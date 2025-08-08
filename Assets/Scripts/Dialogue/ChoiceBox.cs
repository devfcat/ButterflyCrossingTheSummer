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
    public string? choiceResult; //1A#1B#1C 등 선택지 파일 이름 형식
    public string? choiceScore;

    public List<string> contents;

    [Header("UI 컴포넌트")]
    public List<TextMeshProUGUI> tmp_contents;
    public List<GameObject> boxes;

    void OnEnable()
    {
        SetUI();
        SetSound();
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
                boxes[i].SetActive(true);
            }
            else
            {
                tmp_contents[i].gameObject.SetActive(false);
                boxes[i].SetActive(false);
            }
        }
    }

    public void SetSound()
    {
        if (bgm != null && bgm != "")
        {
            if (System.Enum.TryParse<BGM>(bgm, out BGM bgmEnum))
            {
                SoundManager.Instance.PlayBGM(bgmEnum);
            }
        }

        if (se != null && se != "")
        {
            if (System.Enum.TryParse<SFX>(se, out SFX sfxEnum))
            {
                SoundManager.Instance.PlaySFX(sfxEnum);
            }
        }
        else
        {
            SoundManager.Instance.StopSFX();
        }
    }

    public void OnClick(int i = 0) // 선택지 번호
    {
        LogManager.Instance.Add_Log(contents[i]);
        LogManager.Instance.Make_LogUI();
        
        SoundManager.Instance.PlaySFX(SFX.UI);

        // 만약에 선택지 결과가 있으면 그 결과에 따라 다음 대사창으로 넘어감
        if (choiceResult != null && choiceResult != "")
        {
            StartCoroutine(DialogueManager.Instance.Add_Dialogues(i));
        }
        else
        {
            DialogueManager.Instance.NextPage();
        }
    }
}
