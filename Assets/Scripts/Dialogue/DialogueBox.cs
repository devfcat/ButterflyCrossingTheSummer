using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public string? speaker;
    public string content;
    public string? scg;
    public string bg;
    public string? ecg;
    public string bgm;
    public string? se;
    public bool fade;
    public bool isChoice;
    public string? choiceResult;
    public string? choiceScore;

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
                StartCoroutine(Skip_OnClick(0.2f));
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
    }

    public void OnClick()
    {
        if (!isTextEnd)
        {
            m_textAnimation.SkipAnimation();
            return;
        }

        // SoundManager.Instance.PlaySFX(SFX.UI);
        DialogueManager.Instance.NextPage();
    }
}
