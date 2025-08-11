using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_FakeEnd : MonoBehaviour
{
    void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.BGM_FakeEnd);
    }

    public void Next()
    {
        GameManager.Instance.SetState(eState.RealEnd);
    }
}
