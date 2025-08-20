using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    public TMP_FontAsset font_harin;
    public TMP_FontAsset font_minsieun;
    public TMP_FontAsset font_default;

    void OnEnable()
    {
        SetColor();
    }

    void SetColor()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        if (m_text.text == "시은" || m_text.text == "민시은")
        {
            m_text.font = font_minsieun;
        }
        else if (m_text.text == "민하린")
        {
            m_text.font = font_harin;
        }
        else
        {
            m_text.font = font_default;
        }
    }
}
