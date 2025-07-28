using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceBox : MonoBehaviour
{
    public string content;
    public string bgm;
    public string? se;
    public bool fade;
    public bool isChoice;
    public string? choiceResult;
    public string? choiceScore;

    public List<string> contents;

    [Header("UI 컴포넌트")]
    public List<TextMeshProUGUI> tmp_contents;

    void OnEnable()
    {
        SetUI();
    }

    public void SetUI()
    {
        // content를 #으로 나눠서 나온 여러 개의 문자열들을 contents에 넣는다.
        contents.Clear();
        
        if (!string.IsNullOrEmpty(content))
        {
            string[] splitContents = content.Split('#');
            foreach (string splitContent in splitContents)
            {
                string trimmedContent = splitContent.Trim();
                if (!string.IsNullOrEmpty(trimmedContent))
                {
                    contents.Add(trimmedContent);
                }
            }
        }
        
        // UI에 내용 표시
        for (int i = 0; i < tmp_contents.Count; i++)
        {
            if (i < contents.Count)
            {
                tmp_contents[i].text = contents[i];
                tmp_contents[i].gameObject.SetActive(true);
            }
            else
            {
                tmp_contents[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);

        // 추후에 선택지 결과 분기에 따라 추가적으로 대사창을 생성하는 기능 등 추가할 것.
        DialogueManager.Instance.NextPage();
    }
}
