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
     
     // 이미지 관련 필드 추가
     public string? scg;
     public string bg;
     public string? ecg;
     public bool isChangeSoft; // scg와 bg가 부드럽게 바뀌는가 (서서히 나타남)

    public List<string> contents;

    [Header("UI 컴포넌트")]
    public List<TextMeshProUGUI> tmp_contents;
    public List<GameObject> boxes;

    [Header("내부 기능 요소")]
    private bool isInitialized = false; // 초기화 완료 플래그
    public bool isDataSet = false; // 데이터가 설정되었는지 확인하는 플래그

    void OnEnable()
    {
        // 데이터가 설정되지 않았으면 초기화하지 않음
        if (!isDataSet)
        {
            return;
        }
        
        // 이미 초기화되어 있으면 중복 호출 방지
        if (isInitialized)
        {
            return;
        }
        
        SetUI();
        SetSound();
        
        isInitialized = true;
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

        // scg가 null이거나 빈 문자열이면 빈 문자열로 전달
        SCG.Instance.SetSCG(scg ?? "", isChangeSoft);

        if (ecg != null && ecg != "")
        {
            BGECG.Instance.SetECG(ecg);
            return;
        }
        else
        {
            BGECG.Instance.ClearECG();
        }

        if (bg == "" || bg == null)
        {
            Debug.Log("bg가 빈 문자열이므로 ClearBG 호출");
            if (ecg != "" && ecg != null) {}
            else {BGECG.Instance.ClearBG();}
        }
        else
        {
            Debug.Log($"bg가 '{bg}'이므로 SetBG 호출");
            BGECG.Instance.SetBG(bg, isChangeSoft);
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

    void OnDisable()
    {
        // 비활성화될 때 플래그 리셋
        isInitialized = false;
        // isDataSet은 리셋하지 않음 (데이터는 유지)
    }
}
