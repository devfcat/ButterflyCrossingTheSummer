using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 대화창의 전체 흐름을 관리하는 스크립트
/// 대화창이 있는 씬에 부착 (BookMaker 생성 후 자식 오브젝트에 넣기)
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("다음 챕터 씬 이름")]
    public eState n_scene;
    [SerializeField] private List<GameObject> my_dialogues; // 내 대사들 오브젝트
    [SerializeField] private List<GameObject> my_addedDialogues; // 내 선택지 이후 오브젝트
    public GameObject dialoguePrefab; // 대사 프리팹
    public GameObject choicePrefab; // 선택지 프리팹
    public int m_index; // 현재 열람 중인 대사의 인덱스
    public int m_addedIndex; // 선택지 이후의 대사 인덱스
    public int m_choiceFileIndex; // 선택지 파일 인덱스
    public int length;
    public int length_added;

    [Header("선택지 이후 대사를 열람중인가")]
    public bool isReadAddedDialogue;

    [Header("스토리 데이터 파일 처리")]
    public TextAsset m_file; // 현재 스토리 파일
    public List<TextAsset> m_choiceFile; // 선택지 이후의 대사 파일 리스트(선택)
    private DialogueBook bookData; // 이번 챕터의 대화 전체
    private DialogueBook choiceBookData; // 선택지 이후의 대화 전체
    public int value_dialogue; // 대화의 개수
    public List<DialogueCell> list_dialogue; // 대화 데이터들을 이곳에 순서대로 저장됨
    public List<DialogueCell> list_addedDialogue; // 선택지 이후의 대사 데이터들을 이곳에 순서대로 저장됨

    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get {
            if(!instance)
            {
                instance = FindObjectOfType(typeof(DialogueManager)) as DialogueManager;

                if (instance == null)
                    Debug.Log("no Singleton obj");
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 세이브 데이터가 있으면 로드, 없으면 기본값 설정
        if (SaveManager.Instance.m_saveData != null)
        {
            var saveData = SaveManager.Instance.m_saveData;
            
            // 대화 인덱스 로드 (0이 아닐 때만)
            m_index = saveData.dialogueIndex != 0 ? saveData.dialogueIndex : 0;
            
            // 선택지 대화 인덱스 로드
            m_addedIndex = saveData.choiceDialogueIndex ?? 0;
            
            // 선택지 파일 인덱스 로드
            m_choiceFileIndex = saveData.choiceFileIndex ?? 0;
            
            // 선택지 챕터 여부 설정
            isReadAddedDialogue = saveData.isChoiceChapter;
        }
        else
        {
            // 기본값 설정
            m_index = 0;
            m_addedIndex = 0;
            m_choiceFileIndex = 0;
            isReadAddedDialogue = false;
        }

        Load_Dialogue(); // 기본 대화 파일을 모두 로드한다.
    }

    /// <summary>
    /// 대화 파일을 로드하는 함수
    /// </summary>
    public void Load_Dialogue()
    {
        GameManager.Instance.isWorking = true;

        // m_file이 null인지 확인
        if (m_file == null)
        {
            Debug.LogError("스토리 파일이 할당되지 않았습니다. Inspector에서 m_file을 설정해주세요.");
            GameManager.Instance.isWorking = false;
            return;
        }

        Debug.Log($"JSON 파일 내용 길이: {m_file.text.Length}");
        Debug.Log($"JSON 파일 시작 부분: {m_file.text.Substring(0, Mathf.Min(100, m_file.text.Length))}");

        // JSON 파일이 배열 형태이므로 직접 DialogueCell 배열로 파싱
        try
        {
            // JSON 배열을 파싱하기 위해 임시 래퍼 클래스 사용
            string jsonText = "{\"dialogues\":" + m_file.text + "}";
            Debug.Log($"래핑된 JSON 시작 부분: {jsonText.Substring(0, Mathf.Min(150, jsonText.Length))}");
            
            // JSON 유효성 검사
            if (string.IsNullOrEmpty(m_file.text))
            {
                Debug.LogError("JSON 파일이 비어있습니다.");
                GameManager.Instance.isWorking = false;
                return;
            }

            // JSON 배열이 올바른 형식인지 확인
            if (!m_file.text.Trim().StartsWith("[") || !m_file.text.Trim().EndsWith("]"))
            {
                Debug.LogError("JSON 파일이 올바른 배열 형식이 아닙니다.");
                GameManager.Instance.isWorking = false;
                return;
            }
            
            bookData = JsonUtility.FromJson<DialogueBook>(jsonText);
            
            if (bookData == null)
            {
                Debug.LogError("bookData가 null입니다. JSON 파싱에 실패했습니다.");
                GameManager.Instance.isWorking = false;
                return;
            }
            
            if (bookData.dialogues == null)
            {
                Debug.LogError("bookData.dialogues가 null입니다. JSON 구조를 확인해주세요.");
                Debug.LogError($"JSON 텍스트: {jsonText.Substring(0, Mathf.Min(200, jsonText.Length))}");
                GameManager.Instance.isWorking = false;
                return;
            }

            Debug.Log($"파싱된 대화 개수: {bookData.dialogues.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 오류: {e.Message}");
            Debug.LogError($"스택 트레이스: {e.StackTrace}");
            GameManager.Instance.isWorking = false;
            return;
        }

        value_dialogue = bookData.dialogues.Count;
        list_dialogue = new List<DialogueCell>(bookData.dialogues);

        Debug.Log($"대화 로드 완료: {value_dialogue}개의 대화");

        GameManager.Instance.isWorking = false;

        StartCoroutine(Get_Dialogues());
    }

    public IEnumerator Get_Dialogues()
    {
        GameManager.Instance.isWorking = true;

        my_dialogues.Clear();
        length = value_dialogue;

        for (int i = 0; i < length; i++)
        {
            my_dialogues.Add(null);
        }
        for (int i = 0; i < length; i++)
        {
            bool isChoice = bookData.dialogues[i].isChoiceBool;
            if (isChoice) // 선택지 데이터인가
            {
                my_dialogues[i] = Instantiate(choicePrefab, this.transform);
                my_dialogues[i].SetActive(false);

                var script = my_dialogues[i].GetComponent<ChoiceBox>();
                script.content = bookData.dialogues[i].content;
                script.bgm = bookData.dialogues[i].bgm;
                script.se = bookData.dialogues[i].se;
                script.fade = bookData.dialogues[i].fadeBool;
                script.isChoice = bookData.dialogues[i].isChoiceBool;
                script.choiceResult = bookData.dialogues[i].choiceResult;
                script.choiceScore = bookData.dialogues[i].choiceScore;
                
                // 이미지 데이터 설정
                script.scg = bookData.dialogues[i].scg;
                script.bg = bookData.dialogues[i].bg;
                script.ecg = bookData.dialogues[i].ecg;
                script.isChangeSoft = bookData.dialogues[i].isChangeSoftBool;

                                 // 디버그: ChoiceBox 데이터 설정 확인 (로그 제거)

                // 데이터 설정 완료 플래그 설정
                script.isDataSet = true;

                // script.SetUI();
            }
            else // 선택지 데이터가 아닌가
            {
                my_dialogues[i] = Instantiate(dialoguePrefab, this.transform);
                // setactive되면 안됨.
                my_dialogues[i].SetActive(false);

                var script = my_dialogues[i].GetComponent<DialogueBox>();

                script.speaker = bookData.dialogues[i].speaker;
                script.content = bookData.dialogues[i].content;
                script.scg = bookData.dialogues[i].scg;
                script.bg = bookData.dialogues[i].bg;
                script.ecg = bookData.dialogues[i].ecg;
                script.bgm = bookData.dialogues[i].bgm;
                script.se = bookData.dialogues[i].se;
                script.fade = bookData.dialogues[i].fadeBool;
                script.isChoice = bookData.dialogues[i].isChoiceBool;
                script.choiceResult = bookData.dialogues[i].choiceResult;
                script.choiceScore = bookData.dialogues[i].choiceScore;
                // 만약 데이터에 fade가 true라면 fadeTime을 세팅
                if (bookData.dialogues[i].fadeBool)
                {
                    script.fadeTime = bookData.dialogues[i].fadeTimeFloat;
                }
                script.isChangeSoft = bookData.dialogues[i].isChangeSoftBool;

                // 데이터 설정 완료 플래그 설정
                script.isDataSet = true;

                // script.SetUI();
            }
        }

        length = my_dialogues.Count;

        GameManager.Instance.isWorking = false;

        // 모든 대사창을 다 복제하면 첫 대사를 띄움
        // 만약 선택지를 읽고 있었던 상황의 세이브파일을 로드할 시 선택지 이후 대사창을 띄움
        if (isReadAddedDialogue)
        {
            StartCoroutine(Add_Dialogues(m_choiceFileIndex));
        }
        else
        {
            StartCoroutine(Open_Dialogue());
        }

        yield return null;
    }

    // 누르면 다음 대사를 로드하도록
    public void NextPage()
    {
        if (isReadAddedDialogue)
        {
            m_addedIndex++;
            StartCoroutine(Open_AddedDialogue());
        }
        else
        {
            m_index++;
            StartCoroutine(Open_Dialogue());
        }
    }

    // 대사창 흐름에 맞게 대사창을 띄움
    public IEnumerator Open_Dialogue()
    {
        if (m_index >= length) // 마지막 대사창을 이미 로드한 상태라면
        {
            Paging(); // 마지막 대사창을 처리하는 메서드 호출
            yield return null;
        }
        else if (m_index > 0)
        {
            if (m_index - 1 < my_dialogues.Count)
            {
                my_dialogues[m_index-1].SetActive(false);
            }
            if (m_index < my_dialogues.Count)
            {
                // 이미 활성화되어 있는지 확인
                if (!my_dialogues[m_index].activeSelf)
                {
                    my_dialogues[m_index].SetActive(true);
                }
            }
        }
        else 
        {
            if (m_index < my_dialogues.Count)
            {
                // 이미 활성화되어 있는지 확인
                if (!my_dialogues[m_index].activeSelf)
                {
                    my_dialogues[m_index].SetActive(true); // 첫 시작
                }
            }
        }
        
                 // 이번 대사창이 선택지 창이 아니라면 로그 박스에 바로 추가함  
         if (m_index < list_dialogue.Count && !list_dialogue[m_index].isChoiceBool)
         {
             LogManager.Instance.Add_Log(list_dialogue[m_index].content, list_dialogue[m_index].speaker);
             LogManager.Instance.Make_LogUI();
         }

         // 이어하기를 위해 이미지와 사운드 설정
         SetCurrentDialogueUI();

         yield return null;
    }

    // 대사창 흐름에 맞게 선택지 이후 대사창을 띄움
    public IEnumerator Open_AddedDialogue()
    {
        Debug.Log($"Open_AddedDialogue 호출됨 - m_addedIndex: {m_addedIndex}, length_added: {length_added}");
        
        if (m_addedIndex >= length_added) // 마지막 대사창을 이미 로드한 상태라면
        {
            Debug.Log("선택지 이후 대사 마지막에 도달");
            // 마지막 대사창을 로그 박스에 추가함
            int lastIndex = Mathf.Min(length_added - 1, list_addedDialogue.Count - 1);
            
            if (my_addedDialogues.Count > 0)
            {
                my_addedDialogues[my_addedDialogues.Count - 1].SetActive(false);
            }
            
            Paging(); // 마지막 대사창을 처리하는 메서드 호출
            yield return null;
        }
        else if (m_addedIndex > 0)
        {
            Debug.Log($"선택지 이후 대사 {m_addedIndex}번째로 이동");
            my_addedDialogues[m_addedIndex-1].SetActive(false);
            // 이미 활성화되어 있는지 확인
            if (!my_addedDialogues[m_addedIndex].activeSelf)
            {
                my_addedDialogues[m_addedIndex].SetActive(true);
            }
        }
        else 
        {
            Debug.Log("선택지 이후 대사 첫 번째 시작");
            // 이미 활성화되어 있는지 확인
            if (!my_addedDialogues[m_addedIndex].activeSelf)
            {
                my_addedDialogues[m_addedIndex].SetActive(true); // 첫 시작
            }
        }
        
                 // 이번 대사창이 선택지 창이 아니라면 로그 박스에 바로 추가함  
         if (m_addedIndex < list_addedDialogue.Count && !list_addedDialogue[m_addedIndex].isChoiceBool)
         {
             LogManager.Instance.Add_Log(list_addedDialogue[m_addedIndex].content, list_addedDialogue[m_addedIndex].speaker);
             LogManager.Instance.Make_LogUI();
         }

         // 이어하기를 위해 이미지와 사운드 설정
         SetCurrentAddedDialogueUI();

         yield return null;
    }

    // 마지막 대사창을 본 후의 처리 (이번 씬에서 전체 리스트를 다 봤을 때)
    public void Paging()
    {
        Debug.Log($"Paging 호출됨 - isReadAddedDialogue: {isReadAddedDialogue}, m_index: {m_index}, length: {length}");
        
        // 다음 씬으로
        if (isReadAddedDialogue)
        {
            Debug.Log("선택지 이후 대사 완료, 원래 대사로 돌아감");
            isReadAddedDialogue = false; // 먼저 상태 변경
            // 선택지 이후 대사가 끝나면 원래 대사로 돌아가기
            m_index++; // 원래 대사의 다음 인덱스로
            StartCoroutine(Open_Dialogue());
        }
        else
        {
            Debug.Log("일반 대사 완료, 다음 챕터로 이동");
            // 다음 챕터로 이동
            GameManager.Instance.SetState(n_scene);
        }
    }

    /// <summary>
    /// 특정 인덱스의 대화로 이동하는 함수
    /// </summary>
    public bool Jump_ToDialogue(int index)
    {
        if (index >= 0 && index < list_dialogue.Count)
        {
            m_index = index;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 전체 대화 개수를 가져오는 함수
    /// </summary>
    public int Get_TotalCount()
    {
        return value_dialogue;
    }

    // Load_Dialogue() 함수와 Get_Dialogues() 함수를 참고해서 작성함
    // 선택지 이후 대사창을을 생성하고 띄움
    public IEnumerator Add_Dialogues(int choiceFileIndex)
    {
        isReadAddedDialogue = true;
        
        // 선택지 인덱스에 해당하는 텍스트 파일 사용
        var file = m_choiceFile[choiceFileIndex];

        // JSON 파일이 배열 형태이므로 직접 DialogueCell 배열로 파싱
        try
        {
            // JSON 배열을 파싱하기 위해 임시 래퍼 클래스 사용
            string jsonText = "{\"dialogues\":" + file.text + "}";
            Debug.Log($"래핑된 JSON 시작 부분: {jsonText.Substring(0, Mathf.Min(150, jsonText.Length))}");           
            // JSON 유효성 검사
            if (string.IsNullOrEmpty(file.text))
            {
                Debug.LogError("JSON 파일이 비어있습니다.");
            }

            // 선택지에 해당하는 파일을 불러옴
            choiceBookData = JsonUtility.FromJson<DialogueBook>(jsonText);
            
            if (choiceBookData == null)
            {
                Debug.LogError("bookData가 null입니다. JSON 파싱에 실패했습니다.");
            }      
            if (choiceBookData.dialogues == null)
            {
                Debug.LogError("bookData.dialogues가 null입니다. JSON 구조를 확인해주세요.");
                Debug.LogError($"JSON 텍스트: {jsonText.Substring(0, Mathf.Min(200, jsonText.Length))}");
            }
            Debug.Log($"파싱된 대화 개수: {choiceBookData.dialogues.Count}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 오류: {e.Message}");
            Debug.LogError($"스택 트레이스: {e.StackTrace}");;
        }

        //list_addedDialogue에 선택지 이후의 대사들을 순서대로 넣음
        for (int i = 0; i < choiceBookData.dialogues.Count; i++)
        {
            list_addedDialogue.Add(choiceBookData.dialogues[i]);
        }

        my_addedDialogues.Clear();
        length_added = list_addedDialogue.Count;

        // 선택지 이후의 대사들 my_addedDialogues에 순서대로 넣음   
        for (int i = 0; i < choiceBookData.dialogues.Count; i++)
        {
            my_addedDialogues.Add(Instantiate(dialoguePrefab, this.transform));
            my_addedDialogues[i].SetActive(false);

            var script = my_addedDialogues[i].GetComponent<DialogueBox>();

            script.speaker = choiceBookData.dialogues[i].speaker;
            script.content = choiceBookData.dialogues[i].content;
            script.scg = choiceBookData.dialogues[i].scg;
            script.bg = choiceBookData.dialogues[i].bg;
            script.ecg = choiceBookData.dialogues[i].ecg;
            script.bgm = choiceBookData.dialogues[i].bgm;
            script.se = choiceBookData.dialogues[i].se;
            script.fade = choiceBookData.dialogues[i].fadeBool;
            script.isChoice = choiceBookData.dialogues[i].isChoiceBool;
            script.choiceResult = choiceBookData.dialogues[i].choiceResult;
            script.choiceScore = choiceBookData.dialogues[i].choiceScore;
            // 만약 데이터에 fade가 true라면 fadeTime을 세팅
            if (choiceBookData.dialogues[i].fadeBool)
            {
                script.fadeTime = choiceBookData.dialogues[i].fadeTimeFloat;
            }
            script.isChangeSoft = choiceBookData.dialogues[i].isChangeSoftBool;

            // 데이터 설정 완료 플래그 설정
            script.isDataSet = true;

            // script.SetUI();
        }

        // 현재 선택지를 닫는다
        my_dialogues[m_index].SetActive(false);
        
                 // 그리고 선택지에 의해 생성된 대사의 첫 창을 띄움
         StartCoroutine(Open_AddedDialogue());

         yield return null;
     }

     /// <summary>
     /// 현재 대화창의 UI를 설정하는 메서드 (이어하기용)
     /// </summary>
     private void SetCurrentDialogueUI()
     {
         if (m_index >= list_dialogue.Count) return;
         
         // 선택지 타입인지 확인
         if (list_dialogue[m_index].isChoiceBool)
         {
             var choiceBox = my_dialogues[m_index].GetComponent<ChoiceBox>();
             if (choiceBox != null)
             {
                 choiceBox.SetUI();
                 choiceBox.SetSound();
             }
         }
         else
         {
             var dialogueBox = my_dialogues[m_index].GetComponent<DialogueBox>();
             if (dialogueBox != null)
             {
                 dialogueBox.SetUI();
                 dialogueBox.SetSound();
             }
         }
     }

     /// <summary>
     /// 현재 선택지 이후 대화창의 UI를 설정하는 메서드 (이어하기용)
     /// </summary>
     private void SetCurrentAddedDialogueUI()
     {
         if (m_addedIndex < my_addedDialogues.Count && my_addedDialogues[m_addedIndex] != null)
         {
             var dialogueBox = my_addedDialogues[m_addedIndex].GetComponent<DialogueBox>();
             if (dialogueBox != null)
             {
                 dialogueBox.SetUI();
                 dialogueBox.SetSound();
             }
         }
     }
 }
