using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleIllustSetting : MonoBehaviour
{
    public Sprite[] illusts;
    public Image image;

    void OnEnable()
    {
        Set_Illust();
    }

    void Set_Illust()
    {
        if (SaveManager.Instance.Check_Ending())
        {
            image.sprite = illusts[1];
        }
        else
        {
            image.sprite = illusts[0];
        }
    }
}
