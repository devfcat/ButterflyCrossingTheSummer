using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 텍스트 속도를 제어하기 위한 매니저 스크립트
/// </summary>

public enum textStep
{
    normal = 0,
    fast = 1,
    veryfast = 2
}

public class TextSetting : MonoBehaviour
{
    [SerializeField] private float _speed;
    public float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }
    [SerializeField] private textStep m_textStep;
    private textStep my_textStep;
    public TextMeshProUGUI text_ui;

    public TextSettingPreview m_preview;


    private static TextSetting _instance;
    public static TextSetting Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(TextSetting)) as TextSetting;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    public void Start()
    {
        Init();
    }

    public void Control_data(bool isSave = false)
    {
        if (isSave)
        {
            PlayerPrefs.SetInt("textStep", (int)m_textStep);
        }
        else
        {
            int saved = PlayerPrefs.GetInt("textStep");
            if (saved == 0)
            {
                m_textStep = textStep.normal;
            }
            else
            {
                m_textStep = (textStep)saved;
            }
        }
    }

    public void Init()
    {
        Control_data();

        my_textStep = m_textStep;

        Set_UI();
        Set_TextSpeed();
    }

    // 현재 창 모드에 따라 UI 세팅 (실제 적용은 아님)
    public void Set_UI()
    {
        if (my_textStep == textStep.normal)
        {
            text_ui.text = "1단계";
        }
        else
        {
            switch (my_textStep)
            {
                case textStep.fast:
                    text_ui.text = "2단계";
                    break;
                case textStep.veryfast:
                    text_ui.text = "3단계";
                    break;
                default:
                    text_ui.text = "1단계";
                    break;
            }
        }
    }
    void Set_TextSpeed()
    {
        m_textStep = my_textStep;

        speed = Get_Speed();

        Debug.Log(m_textStep + " 적용됨");
    }

    // 왼쪽 오른쪽 해상도 고르기
    public void OnClick(bool isRight)
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        
        int index = (int)my_textStep;

        if (isRight)
        {
            index++;
        }
        else
        {     
            index--;
        }

        if (index == 3)
        {
            index = 0;
        }
        else if (index == -1)
        {
            index = 2;
        }

        my_textStep = (textStep)index;
        Set_UI();

        m_preview.Play_Preview();
    }

    // 적용
    public void OnClick_Setting()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);

        Set_UI();
        Set_TextSpeed();
        Control_data(true); // 해상도 저장
    }

    public float Get_Speed()
    {
        float speed = 0.04f;

        switch (my_textStep)
        {
            case textStep.fast:
                speed = 0.02f;
                break;
            case textStep.veryfast:
                speed = 0.005f;
                break;
            default:
                speed = 0.04f;
                break;
        }

        return speed;
    }
}
