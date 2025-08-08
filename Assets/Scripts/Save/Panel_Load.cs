using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Load : MonoBehaviour
{
    void OnEnable()
    {
        SaveManager.Instance.Load_Data();
        SaveManager.Instance.Set_UI();
    }

    public void Close()
    {
        GameManager.Instance.Control_Load(false);
    }
}
