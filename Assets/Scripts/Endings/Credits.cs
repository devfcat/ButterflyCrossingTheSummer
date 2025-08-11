using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    void OnEnable()
    {
        SoundManager.Instance.PlayBGM(BGM.BGM_RealEnd);
    }
}
