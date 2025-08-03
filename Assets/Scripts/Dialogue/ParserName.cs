using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대사 박스에 사용되는 parser
/// </summary>
public class ParserName : MonoBehaviour
{
    public DialogueBox box;
    private string result;

    public void Parse()
    {
        string content = box.content;
        // 만약 내용 중 @가 있다면 @을 SaveManager.Instance.m_name으로 치환해서 result에 저장
        result = content.Replace("@", SaveManager.Instance.m_name);
        box.content = result;
    }
}
