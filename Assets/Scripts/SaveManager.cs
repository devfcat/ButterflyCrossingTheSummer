using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데이터 셀 하나에 담긴 정보
/// </summary>
public class SaveDataCell
{
    public string date;
    public int chapter;
    public int dialogueIndex; // 몇 번째 대사를 보고 있었는지
    public int score; // 호감도 등 엔딩에 영향을 주는 데이터
    
    /// <summary>
    /// 선택지 이후 대사를 로드한 장면인가
    /// </summary>
    public bool isChoiceChapter; // 선택지 이후 대사를 로드한 장면인가
    public int? choiceFileIndex; // 선택지 이후 대사를 로드한 파일의 인덱스
    public int? choiceDialogueIndex; // 선택지 이후 대사를 로드한 파일의 인덱스

}

/// <summary>
/// 스토리 진행 데이터를 불러오고 저장하는 기능이 담긴 매니저 스크립트
/// </summary>
public class SaveManager : MonoBehaviour
{
    public string m_name; // 플레이어의 현재 이름

    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SaveManager)) as SaveManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
