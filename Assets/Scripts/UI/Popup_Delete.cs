using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Delete : MonoBehaviour
{
    public void OnClick_DeleteYes()
    {
        SaveManager.Instance.Delete_Data();
        GameManager.Instance.Control_Popup(false);
    }

    public void OnClick_DeleteNo()
    {
        GameManager.Instance.Control_Popup(false);
    }
}
