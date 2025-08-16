using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// 씬 이름과 상태 이름이 동일하도록 적을 것
public enum eState
{
    Main = 0,
    Chapter1 = 1, // 초반부
    Chapter2 = 2, // 민하린1
    Chapter3 = 3, // 시은1
    Chapter4 = 4, // 민하린2
    Chapter5 = 5, // 시은2
    Chapter6 = 6, // 시은2-1
    Chapter7 = 7, // 시은2-2
    Chapter8 = 8, // 시은2-3
    Chapter9 = 9, // 민하린3
    Chapter10 = 10, // 주인공시점 1년
    Chapter11 = 11, // 민하린 시점
    Chapter12 = 12, // 주인공시점 2년
    StoryEnd = 13, // 스토리 종료 후 결말 (엔딩 포함)
    AfterStory = 14, // 후일담
    RealEnd = 15, // 진짜 엔딩
    Credits = 16, // 엔딩 크레딧
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
    public bool isWorking; // 씬 변경 중인지 또는 로딩 중인지
    public bool isPopupOn; // 팝업이 열려있는가
    public bool isCurtainOn; // 페이드 인 중인가
    public GameObject m_Popup;
    public GameObject m_Popup_Saved;
    public GameObject m_Popup_Delete;
    public GameObject m_Popup_QuickLoad;

    public GameObject Panel_Load;
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
    /// 챕터의 소제목을 반환해주는 메서드
    /// </summary>
    /// <param name="state">해당 챕터</param>
    /// <returns></returns>
    public string GetChapterName(eState state)
    {
        string chapter_name = "";
        switch(state)
        {
            case eState.Chapter1:
                chapter_name = "CH1. 나는 잘하고 있는 걸까?";
                break;
            case eState.Chapter2:
                chapter_name = "CH2. 주량을 잘 지킵시다.";
                break;
            case eState.Chapter3:
                chapter_name = "CH3. 아름다운 소녀에게 겁을 주는 것은 재밌습니다.";
                break;
            case eState.Chapter4:
                chapter_name = "CH4. 분명 비는 아침부터 내렸을텐데.";
                break;
            case eState.Chapter5:   
                chapter_name = "CH5. 사실 몰래 초를 하나 더 꽂았지만.";
                break;
            case eState.Chapter6:
                chapter_name = "CH6. 햄버거에 진심인 의외의 그녀.";
                break;
            case eState.Chapter7:
                chapter_name = "CH7. 나비는 바다를 건널 수 있을까?";
                break;
            case eState.Chapter8:
                chapter_name = "CH8. 가끔은 이유없이 잡담을 나누고 싶은 날이 있죠.";
                break;
            case eState.Chapter9:
                chapter_name = "CH9. 너와 어울리지 않는 배경";
                break;
            case eState.Chapter10:
                chapter_name = "CH10. 지극히 평범한 결심";
                break;
            case eState.Chapter11:
                chapter_name = "CH11. 지극히 평범한 일상으로,";
                break;
            case eState.Chapter12:
                chapter_name = "CH12. 그러나 하나의 약속과 함께.";
                break;
            case eState.AfterStory:
                chapter_name = "여름을 건너 이어지기를.";
                break;
            case eState.RealEnd:
                chapter_name = "부디 그러기를.";
                break;
            case eState.StoryEnd:
                chapter_name = "세상을 떠난 두 사람의 이야기가,";
                break;
            default:
                chapter_name = m_State.ToString();
                break;
        }

        return chapter_name;
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
        isCurtainOn = true;
        // 페이드 인 시작
        Panel_FadeIn.SetActive(true);
        Panel_FadeOut.SetActive(false);

        // 페이드 인 완료까지 대기
        yield return new WaitForSeconds(time);

        // 페이드 아웃 시작
        Panel_FadeIn.SetActive(false);
        Panel_FadeOut.SetActive(true);

        isCurtainOn = false;
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
    
    // 세이브 패널 제어
    public void Control_Load(bool on)
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        Panel_Load.SetActive(on);
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
    public void Control_Popup(bool on, GameObject b = null, bool sfx = true)
    {
        if (sfx) SoundManager.Instance.PlaySFX(SFX.UI);
        if(m_Popup != null && m_Popup.activeSelf) // 이전에 켜진 팝업이 있는데 다른 팝업을 켤 경우 끔
        {
            m_Popup.SetActive(false);
            isPopupOn = false;
            return;
        }

        isPopupOn = on;
        if (on)
        {
            m_Popup = b;
            m_Popup.SetActive(true);
        }
        else if (m_Popup != null)
        {
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
        if (m_State == eState.RealEnd) // 후일담에서 넘어갈 때
        {
            StartCoroutine(ChangeScene_FakeEnd(m_State.ToString()));
        }
        else
        {
            StartCoroutine(Change_Scene(m_State.ToString()));
        }
    }

    IEnumerator ChangeScene_FakeEnd(string scenename)
    {
        Control_Popup(false, m_Popup, false); // 팝업을 닫음
        SoundManager.Instance.StopBGM(); 

        //yield return StartCoroutine(Fade()); // 창 어둡게

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenename);

        while (!asyncOperation.isDone)
        {
            isWorking = true;
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
        //StartCoroutine(Fade(true));

        isWorking = false;
    }

    /// <summary>
    /// 일정 시간 뒤 씬 변경과 일부 변수를 초기화함
    /// </summary>
    /// <param name="scenename">변경할 씬의 이름</param>
    /// <returns></returns>
    IEnumerator Change_Scene(string scenename)
    {
        Control_Popup(false, m_Popup, false); // 팝업을 닫음
        SoundManager.Instance.StopBGM(); 

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
    }
}