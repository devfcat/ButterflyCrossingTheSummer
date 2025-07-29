using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 모드 표기 UI를 관리하는 스크립트
public class ModeUI : MonoBehaviour
{
    public GameObject ui_auto;
    public GameObject ui_skip;

    public Image btn_auto;
    public Image btn_skip;

    void Update()
    {
        switch (QuickMenuManager.Instance.m_mode)
        {
            case Mode.auto:
                SetModeUI(Color.gray, Color.white, true, false);
                break;
            case Mode.skip:
                SetModeUI(Color.white, Color.gray, false, true);
                break;
            default:
                SetModeUI(Color.white, Color.white, false, false);
                break;
        }
    }

    private void SetModeUI(Color autoColor, Color skipColor, bool autoActive, bool skipActive)
    {
        btn_auto.color = autoColor;
        btn_skip.color = skipColor;
        ui_auto.SetActive(autoActive);
        ui_skip.SetActive(skipActive);
    }
}
