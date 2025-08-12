using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChapterBar : MonoBehaviour
{
    public TextMeshProUGUI tmp_chapter;

    void OnEnable()
    {
        SetChapterUI();
    }

    void SetChapterUI()
    {
        tmp_chapter.text = GameManager.Instance.GetChapterName(GameManager.Instance.m_State);
    }
}
