using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vueaak : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private Animator m_animator;
    public TMP_FontAsset font_default;
    public TMP_FontAsset font_rainbow;

    void OnEnable()
    {
        if (GameManager.Instance.m_State == eState.Chapter2)
        {
            SetColor();
            SetAnimation();
        }
    }

    void SetColor()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        if (m_text.text.Contains("부에엙"))
        {
            // 컬러 그래디언트 모드를 four color gradient로 변경
            // 컬러 그래디언트 활성화
            // #FF0062FF #E8FF83FF #1CFFF1FF #9200FFFF
            m_text.enableVertexGradient = true;
            m_text.color = Color.white;
            m_text.colorGradient = new VertexGradient(new Color32(255, 0, 98, 255), new Color32(232, 255, 131, 255), new Color32(28, 255, 241, 255), new Color32(146, 0, 255, 255));
            m_text.font = font_rainbow;
            m_text.fontSize = 90;
        }
        else
        {
            m_text.enableVertexGradient = false;
            // 기본으로 변경. 검정색 폰트
            m_text.color = Color.black;
            m_text.font = font_default;
            m_text.fontSize = 35;
        }
    }

    void SetAnimation()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetBool("isDanger", m_text.text.Contains("부에엙"));
    }
}
