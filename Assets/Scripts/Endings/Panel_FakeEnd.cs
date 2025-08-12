using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_FakeEnd : MonoBehaviour
{
    void OnEnable()
    {
        QuickMenuManager.Instance.m_mode = Mode.normal;
        SoundManager.Instance.PlayBGM(BGM.BGM_FakeEnd);
    }

    public void Next()
    {
        GameManager.Instance.SetState(eState.RealEnd);
    }
}
