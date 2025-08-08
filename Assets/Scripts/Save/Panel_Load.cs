using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Load : MonoBehaviour
{
    public GameObject label_notexist;
    public GameObject savePage;

    void OnEnable()
    {
        SaveManager.Instance.Load_Data();
        SaveManager.Instance.Set_UI();

        if (SaveManager.Instance.saveDatas.Count == 0)
        {
            label_notexist.SetActive(true);
            savePage.SetActive(false);
        }
        else
        {
            label_notexist.SetActive(false);
            savePage.SetActive(true);
        }
    }

    public void Close()
    {
        GameManager.Instance.Control_Load(false);
    }
}
