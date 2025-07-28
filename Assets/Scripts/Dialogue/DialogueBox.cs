using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public string? speaker;
    public string content;
    public string? scg;
    public string bg;
    public string? ecg;
    public string bgm;
    public string? se;
    public bool fade;
    public bool isChoice;
    public string? choiceResult;
    public string? choiceScore;

    [Header("UI 컴포넌트")]
    public TextMeshProUGUI tmp_speaker;
    public TextMeshProUGUI tmp_content;

    void OnEnable()
    {
        SetUI();
    }

    public void SetUI()
    {
        if (speaker != null)
        {
            if (speaker == "@")
            {
                tmp_speaker.text = SaveManager.Instance.m_name;
            }
            else
            {
                tmp_speaker.text = speaker;
            }
        }
        else {tmp_speaker.text = "";}

        tmp_content.text = content;
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        DialogueManager.Instance.NextPage();
    }
}
