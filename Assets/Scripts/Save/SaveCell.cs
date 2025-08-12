using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 데이터 셀 하나에 담긴 정보
/// </summary>
/// 그리고 셀의 기능 처리를 담당하는 스크립트
[System.Serializable]
public class SaveCell : MonoBehaviour
{
    public string date; // 저장한 날짜
    public string chapter_name; // 저장한 씬의 이름
    public int dialogueIndex; // 몇 번째 대사를 보고 있었는지
    /// <summary>
    /// 선택지 이후 대사를 로드한 장면인가 (챕터 1에만 선택지 분기 있음)
    /// </summary>
    public bool isChoiceChapter; // 선택지 이후 대사를 로드한 장면인가
    public int choiceFileIndex; // 선택지 이후 대사를 로드한 파일의 인덱스
    public int choiceDialogueIndex; // 선택지 이후 로드된 추가 대사의 인덱스

    public TextMeshProUGUI dateText;
    public TextMeshProUGUI chapterNameText;

    void OnEnable()
    {
        Set_UI();
    }

    public void Set_UI()
    {
        dateText.text = date;
        
        // 안전한 열거형 파싱
        if (System.Enum.TryParse<eState>(chapter_name, out eState state))
        {
            chapterNameText.text = GameManager.Instance.GetChapterName(state);
        }
        else
        {
            // 파싱에 실패한 경우 기본값 또는 오류 메시지 표시
            chapterNameText.text = "알 수 없는 챕터";
            Debug.LogWarning($"SaveCell: 유효하지 않은 챕터 이름 '{chapter_name}'");
        }
    }

    public void OnClick_Load()
    {
        SaveManager.Instance.OnLoad_Data(this);
        
        // 안전한 열거형 파싱
        if (System.Enum.TryParse<eState>(chapter_name, out eState state))
        {
            GameManager.Instance.SetState(state);
        }
        else
        {
            Debug.LogError($"SaveCell: 로드할 수 없는 챕터 이름 '{chapter_name}'");
            return;
        }
        
        GameManager.Instance.Control_Load(false);
    }

    public void OnClick_DeletePopup()
    {
        SaveManager.Instance.selectedCell = this;
        GameManager.Instance.Control_Popup(true, GameManager.Instance.m_Popup_Delete);
    }
}
