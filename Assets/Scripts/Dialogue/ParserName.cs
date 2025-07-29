using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
