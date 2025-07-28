using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Setting : MonoBehaviour
{
    [Header("음량 조절 슬라이더")]
    public Slider BgmSlider;
    public Slider SfxSlider;

    // [Header("설정 패널의 버튼들")]

    void OnEnable()
    {
        Init();
    }

    void Init()
    {
        Set_UI_Slider();
        DisplaySetting.Instance.Init();
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
}
