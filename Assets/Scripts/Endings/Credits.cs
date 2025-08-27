using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.BGM_Grasses);
    }
    
    public void StartBGM()
    {
        SoundManager.Instance.Init();
        SoundManager.Instance.PlayBGM(BGM.BGM_RealEnd);
    }

    public void Home()
    {
        GameManager.Instance.SetState(eState.Main);
    }
}
