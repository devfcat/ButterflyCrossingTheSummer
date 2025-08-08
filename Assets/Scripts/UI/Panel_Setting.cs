using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Setting : MonoBehaviour
{
    [Header("음량 조절 슬라이더")]
    public Slider BgmSlider;
    public Slider SfxSlider;

    [Header("설정 패널의 버튼들")]
    public GameObject BTN_Save;

    void OnEnable()
    {
        Init();
    }

    void Init()
    {
        Set_UI_Slider();
        DisplaySetting.Instance.Init();

        if (GameManager.Instance.m_State != eState.Main)
        {
            BTN_Save.SetActive(true);
        }
        else
        {
            BTN_Save.SetActive(false);
        }
    }

    /// <summary>
    /// 볼륨 조절 슬라이더 UI 제어 (실제 소리의 볼륨은 조절하지 않음)
    /// </summary>
    public void Set_UI_Slider()
    {
        BgmSlider.value = SoundManager.Instance.BgmVolume;
        SfxSlider.value = SoundManager.Instance.SfxVolume;
    }

    public void Close()
    {
        GameManager.Instance.Control_Popup(false);
    }

    public void OnClick_Save_And_Close()
    {
        SaveManager.Instance.Save_Data();
        GameManager.Instance.Control_Popup(false);
        GameManager.Instance.SetState(eState.Main);
    }
}
