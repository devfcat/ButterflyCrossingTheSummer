using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class QuickSaveCell : SaveCell
{
    void OnEnable()
    {
        SetInfo();

        chapterNameText.text = GameManager.Instance.GetChapterName((eState)System.Enum.Parse(typeof(eState), chapter_name));
        dateText.text = date;
        SaveManager.Instance.OnLoad_Data(this as SaveCell);
    }

    public void SetInfo()
    {
        chapter_name = SaveManager.Instance.m_saveData.chapter_name;
        date = SaveManager.Instance.m_saveData.date;
        dialogueIndex = SaveManager.Instance.m_saveData.dialogueIndex;
        isChoiceChapter = SaveManager.Instance.m_saveData.isChoiceChapter;
        choiceFileIndex = SaveManager.Instance.m_saveData.choiceFileIndex;
        choiceDialogueIndex = SaveManager.Instance.m_saveData.choiceDialogueIndex;
    }
    public void OnClick_QuickLoad_Yes()
    {
        GameManager.Instance.Control_Popup(false, this.gameObject, false);
        GameManager.Instance.SetState((eState)System.Enum.Parse(typeof(eState), SaveManager.Instance.m_saveData.chapter_name));
    }

    public void OnClick_QuickLoad_No()
    {
        GameManager.Instance.Control_Popup(false, this.gameObject, false);
    }
}
