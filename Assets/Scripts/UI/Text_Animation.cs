using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

/// <summary>
/// 이 스크립트가 부착된 TMP는 켜질 때 글자가 하나씩 타이핑됨
/// </summary>
public class Text_Animation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private string contents;
    StringBuilder stringBuilder = new StringBuilder();
    private Coroutine typeCoroutine; // 현재 실행 중인 타이핑 코루틴을 저장

    void Start()
    {
        tmp = this.GetComponent<TextMeshProUGUI>();
        contents = tmp.text;
        tmp.text = "";

        typeCoroutine = StartCoroutine(TypeTextEffect(contents));
    }

    IEnumerator TypeTextEffect(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            string m_word = text[i].ToString();

            stringBuilder.Append(text[i]);
            tmp.text = stringBuilder.ToString();

            if (QuickMenuManager.Instance.m_mode == Mode.skip)
            {
                yield return new WaitForSeconds(0.0005f);
            }
            else yield return new WaitForSeconds(TextSetting.Instance.speed);
        }
    }

    public void SkipAnimation()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            tmp.text = contents; // 전체 텍스트를 즉시 표시
            typeCoroutine = null;
        }
    }
}
