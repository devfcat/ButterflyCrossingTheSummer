using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Main : MonoBehaviour
{
    [Header("메인 화면의 패널들")]
    public GameObject Panel_Gallery;
    public GameObject Panel_Teams;

    public void OnClick_Setting()
    {
        GameManager.Instance.Control_Setting();
    }

    public void OnClick_Gallery()
    {
        GameManager.Instance.Control_Popup(true, Panel_Gallery);
    }

    public void OnClick_Teams()
    {
        GameManager.Instance.Control_Popup(true, Panel_Teams);
    }

    public void OnClick_New()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        GameManager.Instance.SetState(eState.Chapter1);
    }

    public void OnClick_Quit()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}
