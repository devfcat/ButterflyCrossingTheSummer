using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    void OnEnable()
    {
        SoundManager.Instance.Init();
        SoundManager.Instance.PlayBGM(BGM.BGM_RealEnd);
    }

    public void Home()
    {
        GameManager.Instance.SetState(eState.Main);
    }
}
