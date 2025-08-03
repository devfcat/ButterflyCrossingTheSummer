using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// 씬 이름과 상태 이름이 동일하도록 적을 것
public enum eState
{
    Main = 0,
    Chapter1 = 1,
}

public enum gameState
{
    Default = 0,
    Setting, // 설정 창창
    Log, // 로그 창
    Load, // 파일 로드 창
    Save, // 파일 세이브 창
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    [Header("상태 및 정보")]
    public eState m_State; // 현재 씬 상태
    public gameState g_State; // 현재 일시정지인가 게임 중인가
    public bool isWorking; // 씬 변경 중인지 또는 로딩 중인지
    public bool isPopupOn; // 팝업이 열려있는가
    public GameObject m_Popup;

    public GameObject Panel_Loading;

    [Header("설정 패널들")]
    public GameObject Panel_Setting;

    [Header("페이드 패널")]
    public GameObject Panel_FadeIn;
    public GameObject Panel_FadeOut;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        Application.targetFrameRate = 60;

        isWorking = false;
        isPopupOn = false;

        SetState(eState.Main);
    }

    /// <summary>
    /// 씬 처음 시작일 경우 isOn = true
    /// 다음 씬으로 넘어가려고 하는 경우 isOn = false
    /// </summary>
    /// <param name="isOn"></param>
    public IEnumerator Fade(bool isOn = false)
    {
        isWorking = true;

        // 씬 처음 시작일 경우
        if (isOn)
        {
            Panel_FadeIn.SetActive(false);
            Panel_FadeOut.SetActive(true);
        }
        else // 다음 씬으로 넘어가려고 하는 경우
        {
            Panel_FadeIn.SetActive(true);
            Panel_FadeOut.SetActive(false);
        }

        yield return new WaitForSeconds(3f);

        isWorking = false;
    }

    /// <summary>
    /// 잠깐 어두워졌다가 다시 밝아지는 화면 효과
    /// 기본 시간은 2초
    /// StartCoroutine(GameManager.Instance.Curtain()); 로 사용.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Curtain(float time = 2f)
    {
        isWorking = true;

        Panel_FadeIn.SetActive(true);
        Panel_FadeOut.SetActive(false);

        yield return new WaitForSeconds(time);

        Panel_FadeIn.SetActive(false);
        Panel_FadeOut.SetActive(true);

        isWorking = false;
    }

    public void Load()
    {
        
    }

    public void Save()
    {

    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("저장된 정보 삭제");
        }
#endif
        // 모든 씬에서 ESC 누를 시 설정 팝업이 뜨도록 제어
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPopupOn)
            {
                Control_Popup(false);
            }
            else
            {
                Control_Setting();
            }
        }

        if (isWorking)
        {
            if (!Panel_Loading.activeSelf) Panel_Loading.SetActive(true);
        }
        else
        {
            if (Panel_Loading.activeSelf) Panel_Loading.SetActive(false);
        }
    }

    // 설정창 제어
    public void Control_Setting()
    {
        if (Panel_Setting.activeSelf)
        {
            Control_Popup(false);
        }
        else
        {
            Control_Popup(true, Panel_Setting);
        }
    }

    // 팝업을 관리하는 메서드
    public void Control_Popup(bool on, GameObject b = null)
    {
        if(m_Popup != null && m_Popup.activeSelf) // 이전에 켜진 팝업이 있는데 다른 팝업을 켤 경우 끔
        {
            SoundManager.Instance.PlaySFX(SFX.UI);
            m_Popup.SetActive(false);
            return;
        }

        isPopupOn = on;
        if (on)
        {
            SoundManager.Instance.PlaySFX(SFX.UI);

            m_Popup = b;
            m_Popup.SetActive(true);
        }
        else if (m_Popup != null)
        {
            SoundManager.Instance.PlaySFX(SFX.UI);
            m_Popup.SetActive(false);
        }
    }
    
    // 상태 머신 함수
    public void SetState(eState state)
    {
        if (state == eState.Main && m_State == eState.Main)
        {
            return;
        }

        // 상태 관리 메서드가 작동중인가
        // 프롤로그 스킵은 예외
        if (isWorking)
        {
            Debug.Log("씬 변경중");
            return;
        }
        else
        {
            isWorking = true;
        }

        m_State = state;
        StartCoroutine(Change_Scene(m_State.ToString()));
    }

    /// <summary>
    /// 일정 시간 뒤 씬 변경과 일부 변수를 초기화함
    /// </summary>
    /// <param name="scenename">변경할 씬의 이름</param>
    /// <returns></returns>
    IEnumerator Change_Scene(string scenename)
    {
        g_State = gameState.Default;

        Control_Popup(false); // 팝업을 닫음
        SoundManager.Instance.BgmControl(BgmStatus.Pause);
        // SoundManager.Instance.PlaySFX(SFX.SceneChange);

        yield return StartCoroutine(Fade()); // 창 어둡게

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenename);

        while (!asyncOperation.isDone)
        {
            isWorking = true;
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
        StartCoroutine(Fade(true));

        isWorking = false;

        SoundManager.Instance.BgmControl(BgmStatus.Play);
    }
}