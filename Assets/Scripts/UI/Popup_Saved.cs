using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Saved : MonoBehaviour
{
    public void Close()
    {
        GameManager.Instance.Control_Popup(false, this.gameObject, false);
    }
}
