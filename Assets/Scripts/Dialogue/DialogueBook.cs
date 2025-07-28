using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챕터의 스크립트와 세부 셀의 데이터구조
/// </summary>
[System.Serializable]
public class DialogueBook
{
    public List<DialogueCell> dialogues;
}

/// <summary>
/// 한 대사 데이터에 담겨져있는 정보
/// </summary>
[System.Serializable]
public class DialogueCell
{
    public string? speaker;
    public string content;
    public string? scg;
    public string bg;
    public string? ecg;
    public string bgm;
    public string? se;
    public string fade; // JSON에서 문자열로 받음
    public string isChoice; // JSON에서 문자열로 받음
    public string? choiceResult;
    public string? choiceScore;

    // bool 프로퍼티로 변환
    public bool fadeBool
    {
        get { return fade?.ToUpper() == "TRUE"; }
    }

    public bool isChoiceBool
    {
        get { return isChoice?.ToUpper() == "TRUE"; }
    }
}
