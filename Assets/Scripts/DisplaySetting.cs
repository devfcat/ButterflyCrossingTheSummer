using UnityEngine;
using TMPro;

/// <summary>
/// 창모드와 전체화면 모드 전환 기능
/// </summary>

public enum eResolution
{
    fullscreen = 0,
    big = 1,
    midium = 2,
    small = 3,
    low = 4,
}

public class DisplaySetting : MonoBehaviour
{
    private static DisplaySetting _instance;
    public static DisplaySetting Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(DisplaySetting)) as DisplaySetting;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    [SerializeField] private eResolution m_resolution;
    private eResolution my_resolution;
    public TextMeshProUGUI text_ui;
    public bool SettingMod; // 설정 창 말고 기본 세팅 시

    public void Start()
    {
        Init();
    }

    public void Control_data(bool isSave = false)
    {
        if (isSave)
        {
            PlayerPrefs.SetInt("eResolution", (int)m_resolution);
        }
        else
        {
            int saved = PlayerPrefs.GetInt("eResolution");
            if (saved == 0)
            {
                m_resolution = eResolution.fullscreen;
            }
            else
            {
                m_resolution = (eResolution)saved;
            }
        }
    }

    public void Init()
    {
        Control_data();

        my_resolution = m_resolution;

        Set_UI();
        Set_Resolution();
    }

    // 현재 창 모드에 따라 UI 세팅 (실제 적용은 아님)
    public void Set_UI()
    {
        if (SettingMod)
        {
            return;
        }

        if (my_resolution == eResolution.fullscreen)
        {
            text_ui.text = "전체 화면";
        }
        else
        {
            switch (my_resolution)
            {
                case eResolution.big:
                    text_ui.text = "1920 X 1080";
                    break;
                case eResolution.midium:
                    text_ui.text = "1600 X 900";
                    break;
                case eResolution.small:
                    text_ui.text = "1280 X 720";
                    break;
                case eResolution.low:
                    text_ui.text = "800 X 450";
                    break;
                default:
                    text_ui.text = "Error";
                    break;
            }
        }
    }

    // 실제로 해상도를 적용하는 부분
    void Set_Resolution()
    {
        m_resolution = my_resolution; // 내가 고른 해상도를 가져오고 적용

        switch (m_resolution)
        {
            case eResolution.big:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case eResolution.midium:
                Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
                break;
            case eResolution.small:
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
            case eResolution.low:
                Screen.SetResolution(800, 450, FullScreenMode.Windowed);
                break;
            default:
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
        }
    }

    // 왼쪽 오른쪽 해상도 고르기
    public void OnClick(bool isRight)
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        
        int index = (int)my_resolution;

        if (isRight)
        {
            index++;
        }
        else
        {     
            index--;
        }

        if (index == 5)
        {
            index = 0;
        }
        else if (index == -1)
        {
            index = 4;
        }

        my_resolution = (eResolution)index;
        Set_UI();
    }

    // 적용
    public void OnClick_Setting()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);

        Set_UI();
        Set_Resolution();
        Control_data(true); // 해상도 저장

        Debug.Log(m_resolution + " 적용됨");
    }
}
