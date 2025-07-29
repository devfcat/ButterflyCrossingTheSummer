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
    [SerializeField] private List<GameObject> my_dialogues; // 내 대사들 오브젝트
    public GameObject dialoguePrefab; // 대사 프리팹
    public GameObject choicePrefab; // 선택지 프리팹
    public int m_index; // 현재 열람 중인 대사의 인덱스
    public int length;


    [Header("스토리 데이터 파일 처리")]
    public TextAsset m_file; // 현재 스토리 파일
    public List<TextAsset> m_choiceFile; // 선택지 이후의 대사 파일 리스트(선택)
    private DialogueBook bookData; // 이번 챕터의 대화 전체
    public int value_dialogue; // 대화의 개수
    public List<DialogueCell> list_dialogue; // 대화 데이터들을 이곳에 순서대로 저장됨

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
        m_index = 0; // 추후 로드 시스템 생기면 여기에 로드한 인덱스 값 넣기

        Load_Dialogue();
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

                script.SetUI();

                // 만약 choiceResult에 값이 있다면 (선택 이후 스크립트가 개별 존재한다면)
            }
            else // 선택지 데이터가 아닌가
            {
                my_dialogues[i] = Instantiate(dialoguePrefab, this.transform);
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

                script.SetUI();
            }
        }

        length = my_dialogues.Count;

        GameManager.Instance.isWorking = false;

        // 모든 대사창을 다 복제하면 첫 대사를 띄움
        StartCoroutine(Open_Dialogue());

        yield return null;
    }

    // 누르면 다음 대사를 로드하도록
    public void NextPage()
    {
        m_index++;
        StartCoroutine(Open_Dialogue());
    }

    // 대사창 흐름에 맞게 대사창을 띄움
    public IEnumerator Open_Dialogue()
    {
        if (m_index >= length) // 마지막 대사창을 이미 로드한 상태라면
        {
            Paging(); // 마지막 대사창을 처리하는 메서드 호출
        }
        else if (m_index > 0)
        {
            my_dialogues[m_index-1].SetActive(false);
            my_dialogues[m_index].SetActive(true);
        }
        else {my_dialogues[m_index].SetActive(true);} // 첫 시작
        yield return null;
    }

    // 마지막 대사창을 본 후의 처리
    public void Paging()
    {

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
}
