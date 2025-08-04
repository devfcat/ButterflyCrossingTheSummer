using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제작진 패널의 기능
/// </summary>
public class Panel_Teams : MonoBehaviour
{
    public void Close()
    {
        GameManager.Instance.Control_Popup(false);
    }
}
