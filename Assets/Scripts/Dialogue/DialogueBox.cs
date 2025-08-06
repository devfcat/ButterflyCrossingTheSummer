using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 대사창의 데이터구조를 정의한 클래스
/// 대사창이 뜰 때의 기능도 포함되어 있다.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    public string? speaker;
    public string content;
    public string? scg;
    public string bg;
    public string? ecg;
    public string? bgm;
    public string? se;
    public bool fade;
    public float? fadeTime; // 커튼 페이드 시간(fade가 true일 때만 사용)
    public bool isChoice;
    public string? choiceResult;
    public string? choiceScore;
    public bool isChangeSoft; // scg와 bg가 부드럽게 바뀌는가 (서서히 나타남)

    [Header("UI 컴포넌트")]
    public TextMeshProUGUI tmp_speaker;
    public TextMeshProUGUI tmp_content;
    public GameObject arrow;

    [Header("내부 기능 요소")]
    public Text_Animation m_textAnimation;
    public ParserName parser;
    private bool isTextEnd; // 모든 텍스트가 출력되었는가?
    private bool isAutoCoroutineRunning = false;

    void OnEnable()
    {
        SetUI();
        // 해당하는 scg, bg, ecg, bgm, se 등을 세팅
        SetSound();
    }

    void Update()
    {
        if (content == tmp_content.text) // 모든 대사를 띄웠다면
        {
            isTextEnd = true;

            if (!arrow.activeSelf)
            {
                arrow.SetActive(true);
            }

            // 오토 모드 코루틴 중복 방지
            if (QuickMenuManager.Instance.m_mode == Mode.auto && !isAutoCoroutineRunning)
            {
                StartCoroutine(Delay_OnClick(2f));
                isAutoCoroutineRunning = true;
            }

            if (QuickMenuManager.Instance.m_mode == Mode.skip && !isAutoCoroutineRunning)
            {
                StartCoroutine(Delay_OnClick(0.1f));
                isAutoCoroutineRunning = true;
            }
        }
        else // 대사가 나올 때
        {
            isTextEnd = false;

            if (QuickMenuManager.Instance.m_mode == Mode.skip)
            {
                StartCoroutine(Skip_OnClick(0.15f));
            }
            isAutoCoroutineRunning = false; // 텍스트가 바뀌면 코루틴 플래그 해제
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClick();
        }
    }

    public IEnumerator Skip_OnClick(float time)
    {
        yield return new WaitForSeconds(time);

        OnClick();
        isAutoCoroutineRunning = false; // 코루틴 끝나면 플래그 해제
    }

    public IEnumerator Delay_OnClick(float time)
    {
        yield return new WaitForSeconds(time);

        OnClick();
        isAutoCoroutineRunning = false; // 코루틴 끝나면 플래그 해제
    }

    /// <summary>
    /// 현재 대사의 데이터를 화면에 표시하며 관련 이벤트를 실행시킴
    /// </summary>
    public void SetUI()
    {
        parser.Parse();
        
        arrow.SetActive(false);

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

        Debug.Log($"DialogueBox.SetUI - bg: '{bg}', speaker: '{speaker}'");

        if (ecg != null && ecg != "")
        {
            BGECG.Instance.SetECG(ecg);
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

        // scg가 null이거나 빈 문자열이면 빈 문자열로 전달
        SCG.Instance.SetSCG(scg ?? "", isChangeSoft);
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
        else
        {
            SoundManager.Instance.StopBGM();
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

    public void OnClick()
    {
        if (!isTextEnd)
        {
            m_textAnimation.SkipAnimation();
            return;
        }

        /// 만약 현재 대사창의 fade가 true라면 이 대사를 다 보고 다음 대사창으로 넘어가기 전에 커튼을 페이드 시킴
        if (fade && QuickMenuManager.Instance.m_mode == Mode.normal) // 만약 스킵이나 오토라면 넘길 때 암전효과를 끔   
        {
            // gameManager의 커튼을 일정 시간 세팅하고 완료 후 다음 페이지로
            StartCoroutine(CurtainAndNextPage(fadeTime ?? 2f));
        }
        else
        {
            // SoundManager.Instance.PlaySFX(SFX.UI);
            DialogueManager.Instance.NextPage();
        }
    }

    private IEnumerator CurtainAndNextPage(float time)
    {
        yield return StartCoroutine(GameManager.Instance.Curtain(time));
        // SoundManager.Instance.PlaySFX(SFX.UI);
        DialogueManager.Instance.NextPage();
    }
}
