using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스토리 진행 모드
public enum Mode
{
    normal = 0,
    skip = 1,
    auto = 2,
}

/// <summary>
/// 퀵메뉴의 기능들을 서술한 매니저 스크립트
/// 타 스크립트에서 instance를 호출하여 사용
/// </summary>
public class QuickMenuManager : MonoBehaviour
{
    private static QuickMenuManager _instance;
    public static QuickMenuManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(QuickMenuManager)) as QuickMenuManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    public List<Sprite> audioImages; // 소리 켬 끔의 이미지
    public Image audioBTN; // 소리 버튼의 이미지
    public List<GameObject> uis; // UI들 (켜고 끌 게임오브젝트들)

    [SerializeField] private bool isUIOn;

    public Mode m_mode; // 현재 모드

    void Update()
    {
        if (!isUIOn)
        {
            // 키보드 또는 마우스의 아무 키나 누르면 ControlUIImages로 다시 UI를 켬
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                ControlUIImages(true); // UI를 다시 켬
                isUIOn = true;
            }
        }

        // 추후 다른 방법을 찾아볼 예정 (임시)
        if (m_mode != Mode.normal) // 일반 모드가 아닐 시 ESC를 누르면 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_mode = Mode.normal;
            }
        }
    }

    void Start()
    {
        m_mode = Mode.normal; // 초기화
    }

    public void Setting()
    {
        GameManager.Instance.Control_Setting();
    }

    public void Control_Skip()
    {
        if (m_mode == Mode.skip) // 이미 스킵 모드일 경우
        {
            m_mode = Mode.normal; // 모드 끄기기
        }
        else // 기본 모드거나 오토일 경우
        {
            m_mode = Mode.skip;
        }
    }

    public void Control_Auto()
    {
        if (m_mode == Mode.auto) // 이미 오토 모드일 경우
        {
            m_mode = Mode.normal; // 모드 끄기기
        }
        else // 기본 모드거나 스킵일 경우
        {
            m_mode = Mode.auto;
        }
    }

    public void AudioControl()
    {

    }

    public void ControlUIImages(bool isOn)
    {
        isUIOn = isOn;

        for (int i = 0; i < uis.Count; i++)
        {
            uis[i].SetActive(isOn);
        }
    }

    public void Home()
    {
        GameManager.Instance.SetState(eState.Main);
    }
}
