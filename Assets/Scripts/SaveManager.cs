using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 데이터 셀 하나에 담긴 정보
/// </summary>
public class SaveDataCell
{
    public string date; // 저장한 날짜
    public string chapter_name; // 저장한 씬의 이름
    public int dialogueIndex; // 몇 번째 대사를 보고 있었는지
    public int score; // 호감도 등 엔딩에 영향을 주는 데이터 (동기화)
    
    /// <summary>
    /// 선택지 이후 대사를 로드한 장면인가 (챕터 1에만 선택지 분기 있음)
    /// </summary>
    public bool isChoiceChapter; // 선택지 이후 대사를 로드한 장면인가
    public int? choiceFileIndex; // 선택지 이후 대사를 로드한 파일의 인덱스
    public int? choiceDialogueIndex; // 선택지 이후 로드된 추가 대사의 인덱스
}

/// <summary>
/// 스토리 진행 데이터를 불러오고 저장하는 기능이 담긴 매니저 스크립트
/// </summary>
public class SaveManager : MonoBehaviour
{
    public string m_name; // 플레이어의 현재 이름
    public List<SaveDataCell> saveDatas; // 세이브 데이터 셀 리스트

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

    // 세이브 데이터 셀 구조로 데이터 셀을 만들고 세이브파일.json 데이터에 추가함
    public void Save_Data()
    {
        SaveDataCell m_data = new SaveDataCell();
        m_data.date = DateTime.Now.ToString("yyyy년 MM월 dd일\nHH시 mm분");
        m_data.chapter_name = GameManager.Instance.m_State.ToString();

        m_data.dialogueIndex = DialogueManager.Instance.m_index;
        m_data.score = 0;
        m_data.isChoiceChapter = DialogueManager.Instance.isReadAddedDialogue;
        m_data.choiceFileIndex = DialogueManager.Instance.m_index;
        m_data.choiceDialogueIndex = DialogueManager.Instance.m_addedIndex;

        saveDatas.Add(m_data);

        Put_File();
    }

    // 선택한 셀의 데이터를 덮어씀 그리고 파일을 저장함
    public void Overwrite_Data()
    {
        
    }

    public void Put_File()
    {
        string json = JsonUtility.ToJson(saveDatas);
        File.WriteAllText(Application.persistentDataPath + "/userData.json", json);
    }

    // 선택한 데이터 셀을 세이브파일.json에서 삭제함
    public void Delete_Data(SaveDataCell target)
    {

    }
}
