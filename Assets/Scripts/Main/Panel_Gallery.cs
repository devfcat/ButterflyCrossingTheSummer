using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Gallery : MonoBehaviour
{
    public void Close()
    {
        GameManager.Instance.Control_Popup(false);
    }
}
