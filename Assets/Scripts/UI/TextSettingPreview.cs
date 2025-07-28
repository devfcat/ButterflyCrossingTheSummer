using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

public class TextSettingPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private string contents;
    StringBuilder stringBuilder = new StringBuilder();
    private Coroutine typeCoroutine; // 현재 실행 중인 타이핑 코루틴을 저장

    private float speed;

    void OnEnable()
    {
        tmp = this.GetComponent<TextMeshProUGUI>();
        contents = "스크립트 출력 속도 예시"; // 이 문구를 예시 문구로 사용
        tmp.text = "";

        Play_Preview();
    }

    public void Play_Preview()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }
        
        speed = TextSetting.Instance.Get_Speed();

        tmp.text = "";
        stringBuilder = new StringBuilder();

        typeCoroutine = StartCoroutine(TypeTextEffect(contents));
    }

    IEnumerator TypeTextEffect(string text)
    {
        while (true) // 무한 반복
        {
            for (int i = 0; i < text.Length; i++)
            {
                string m_word = text[i].ToString();
                stringBuilder.Append(text[i]);
                tmp.text = stringBuilder.ToString();
                yield return new WaitForSeconds(speed);
            }
            
            // 텍스트 출력이 끝나면 잠깐 쉬기
            yield return new WaitForSeconds(1f); // 1초 대기
            
            // 다음 반복을 위해 초기화
            tmp.text = "";
            stringBuilder.Clear();
        }
    }
}
