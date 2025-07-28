using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 로드 후에 애니메이션이 실행되면 알아서 자신을 끔
/// </summary>
public class FadeOut : MonoBehaviour
{
    public void Off()
    {
        this.gameObject.SetActive(false);
    }
}