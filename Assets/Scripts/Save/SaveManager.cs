using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// JSON 배열 파싱을 위한 래퍼 클래스
/// </summary>
[System.Serializable]
public class SaveDataWrapper
{
    public List<SaveData> saveDatas;
}

/// <summary>
/// 세이브 데이터 구조체 (JSON 직렬화용)
/// </summary>
[System.Serializable]
public class SaveData
{
    public string date;
    public string chapter_name;
    public int dialogueIndex;
    public bool isChoiceChapter;
    public int choiceFileIndex;
    public int choiceDialogueIndex;
}

/// <summary>
/// 스토리 진행 데이터를 불러오고 저장하는 기능이 담긴 매니저 스크립트
/// </summary>
public class SaveManager : MonoBehaviour
{
    public string m_name; // 플레이어의 현재 이름
    public List<SaveCell> saveDatas; // 세이브 데이터 셀 리스트
    public SaveCell m_saveData; // 현재 세이브 데이터 셀
    public SaveCell selectedCell; // 선택한 셀

    [Header("세이브 페이지네이션용 프리팹")]
    public GameObject savePagePrefebs;
    public Transform savePageContent;
    public RectTransform savePageRect;

    public List<GameObject> pages; // 페이지네이션 셀 리스트


    [Header("세이브 파일 경로")]
    [SerializeField] private string saveFilePath;

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

        saveFilePath = Application.persistentDataPath + "/userData.json";
        Load_Data();
    }

    // 씬이 변경될때마다 SaveManager를 다시 가동
    public void OnSceneChanged()
    {
        Load_Data();
    }

    public void Set_Panel()
    {
        // 만약 데이터 개수가 0이라면 패널을 닫음
        if (saveDatas.Count == 0)
        {
            GameManager.Instance.Control_Load(false);
        }
        else
        {
            Set_UI();
        }
    }

    // 현재 상태를 기반으로 데이터 셀을 만들어서 반환함
    public SaveCell Get_Data()
    {
        SaveCell m_data = new SaveCell();

        m_data.date = DateTime.Now.ToString("yyyy년 MM월 dd일\nHH시 mm분 ss초");

        m_data.chapter_name = GameManager.Instance.m_State.ToString();
        m_data.dialogueIndex = DialogueManager.Instance.m_index;

        m_data.isChoiceChapter = DialogueManager.Instance.isReadAddedDialogue; // 선택지 대사 볼 때 true
        m_data.choiceFileIndex = DialogueManager.Instance.m_choiceFileIndex; // 로드해야 하는 선택지 파일의 인덱스
        m_data.choiceDialogueIndex = DialogueManager.Instance.m_addedIndex; // 로드해야 하는 선택지 대사의 인덱스

        return m_data;
    }

    // 유저 데이터 파일을 불러옴
    public void Load_Data()
    {
        // 유저 데이터 파일이 있는지 확인하여 가져옴
        if(File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                Debug.Log($"로드된 JSON: {json}");
                
                // JSON 배열을 파싱하기 위해 래퍼 클래스 사용
                if (!string.IsNullOrEmpty(json))
                {
                    // JSON이 배열 형태인지 확인
                    if (json.Trim().StartsWith("[") && json.Trim().EndsWith("]"))
                    {
                        string wrappedJson = "{\"saveDatas\":" + json + "}";
                        SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(wrappedJson);
                        saveDatas.Clear();
                        if (wrapper.saveDatas != null)
                        {
                            foreach (var data in wrapper.saveDatas)
                            {
                                SaveCell cell = new SaveCell();
                                cell.date = data.date;
                                cell.chapter_name = data.chapter_name;
                                cell.dialogueIndex = data.dialogueIndex;
                                cell.isChoiceChapter = data.isChoiceChapter;
                                cell.choiceFileIndex = data.choiceFileIndex;
                                cell.choiceDialogueIndex = data.choiceDialogueIndex;
                                saveDatas.Add(cell);
                            }
                        }
                    }
                    else
                    {
                        // 기존 방식으로 시도
                        saveDatas = JsonUtility.FromJson<List<SaveCell>>(json) ?? new List<SaveCell>();
                    }
                }
                else
                {
                    saveDatas.Clear();
                }
                
                Debug.Log($"로드된 세이브 데이터 개수: {saveDatas?.Count ?? 0}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"세이브 데이터 로드 오류: {e.Message}");
            }
        }
    }

    // 가장 최근 세이브 데이터를 로드함
    public void OnLoad_RecentData()
    {
        Load_Data();
        
        if (saveDatas.Count > 0)
        {
            m_saveData = saveDatas[saveDatas.Count - 1] as SaveCell;
        }
    }

    // 게임 시작 로드
    public void OnLoad_Data(SaveCell target)
    {
        m_saveData = target;
    }

    // 세이브 데이터 셀 구조로 데이터 셀을 만들고 세이브파일.json 데이터에 추가함
    public void Save_Data()
    { 
        Load_Data();

        m_saveData = Get_Data();
        saveDatas.Add(m_saveData);

        Put_File();
    }

    // 선택한 셀(target)의 데이터를 덮어씀 그리고 파일을 저장함 (사용하지 않음)
    /*
    public void Overwrite_Data(SaveCell target)
    {
        // 세이브파일.json애서 target의 데이터를 찾아서 덮어씀
        string json = File.ReadAllText(saveFilePath);

        for (int i = 0; i < saveDatas.Count; i++)
        {
            if (saveDatas[i].date == target.date)
            {
                saveDatas[i] = Get_Data();
                break;
            }
        }

        Put_File();
    }
    */

    private void Put_File()
    {
        try
        {
            // SaveCell을 SaveData로 변환해서 저장
            List<SaveData> saveDataList = new List<SaveData>();
            foreach (var cell in saveDatas)
            {
                SaveData data = new SaveData();
                data.date = cell.date;
                data.chapter_name = cell.chapter_name;
                data.dialogueIndex = cell.dialogueIndex;
                data.isChoiceChapter = cell.isChoiceChapter;
                data.choiceFileIndex = cell.choiceFileIndex;
                data.choiceDialogueIndex = cell.choiceDialogueIndex;
                saveDataList.Add(data);
            }
            
            // JSON 배열 형태로 직접 생성
            string json = "[";
            for (int i = 0; i < saveDataList.Count; i++)
            {
                string itemJson = JsonUtility.ToJson(saveDataList[i]);
                json += itemJson;
                if (i < saveDataList.Count - 1)
                {
                    json += ",";
                }
            }
            json += "]";
            
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"세이브 데이터 저장 완료: {saveDatas.Count}개, JSON: {json}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"세이브 데이터 저장 오류: {e.Message}");
        }

        Load_Data(); // 데이터 로드
    }

    // 선택한 데이터 셀을 세이브파일.json에서 삭제함
    public void Delete_Data()
    {
        if (selectedCell != null)
        {
            // selectedCell의 데이터와 일치하는 항목을 saveDatas에서 찾아서 삭제
            for (int i = 0; i < saveDatas.Count; i++)
            {
                if (saveDatas[i].date == selectedCell.date && 
                    saveDatas[i].chapter_name == selectedCell.chapter_name &&
                    saveDatas[i].dialogueIndex == selectedCell.dialogueIndex)
                {
                    saveDatas.RemoveAt(i);
                    Debug.Log($"세이브 데이터 삭제 완료: {selectedCell.date}");
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("삭제할 셀이 선택되지 않았습니다.");
        }

        Put_File(); 
        Set_Panel();
    }

    public void Set_UI()
    {
        // 모든 페이지를 삭제하고 다시 구성
        foreach (var p in pages)
        {
            Destroy(p);
        }
        pages.Clear();

        GameManager.Instance.isWorking = true;

        // 현재 데이터 셀 개수를 셈
        int count = saveDatas.Count;
        // 한 페이지에 10개의 셀이 들어감 (올림해야 함)
        int page = (int)Mathf.Ceil(count / 10f);

        // 페이지네이션 셀 리스트 초기화
        pages = new List<GameObject>();

        // 페이지네이션 셀 리스트에 페이지네이션 셀 추가
        for (int i = 0; i < page; i++)
        {
            GameObject pageObject = Instantiate(savePagePrefebs, savePageContent);
            pages.Add(pageObject);
        }
    
        // i 페이지의 0~10개의 셀에 데이터를 적용함
        for (int i = 0; i < page; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                // 만약 i*10+j가 데이터 셀 개수보다 크면 종료
                if (i * 10 + j >= count)
                {
                    pages[i].GetComponent<SavePage>().saveCells[j].SetActive(false);
                }
                else
                {
                    pages[i].GetComponent<SavePage>().saveCells[j].SetActive(false);

                    var script = pages[i].GetComponent<SavePage>().saveCells[j].GetComponent<SaveCell>();
                    
                    script.date = saveDatas[i * 10 + j].date;
                    script.chapter_name = saveDatas[i * 10 + j].chapter_name;

                    script.dialogueIndex = saveDatas[i * 10 + j].dialogueIndex;
                    script.isChoiceChapter = saveDatas[i * 10 + j].isChoiceChapter;
                    script.choiceFileIndex = saveDatas[i * 10 + j].choiceFileIndex;
                    script.choiceDialogueIndex = saveDatas[i * 10 + j].choiceDialogueIndex;

                    pages[i].GetComponent<SavePage>().saveCells[j].SetActive(true);

                    script.Set_UI();
                }
            }
        }

        // 페이지네이션 컨텐츠의 사이즈를 조정 (셀 높이 1520)
        savePageRect.sizeDelta = new Vector2(1520 * page, savePageRect.sizeDelta.y);

        GameManager.Instance.isWorking = false;
    }


    public void Save_ECGData(ECG name)
    {
        // 해당 ECG를 열람했을 때 이 아이디로 저장 (1이 해금했다는 의미)
        PlayerPrefs.SetInt(name.ToString(), 1);
    }

    public bool Check_ECGData(ECG name)
    {
        return PlayerPrefs.GetInt(name.ToString()) == 1;
    }

    /// <summary>
    /// 엔딩을 모두 보았는지 확인
    /// </summary>
    /// <returns></returns>
    public bool Check_Ending()
    {
        return PlayerPrefs.GetInt("isEnding") == 1;
    }

    public void Save_Ending()
    {
        PlayerPrefs.SetInt("isEnding", 1);
    }
}
