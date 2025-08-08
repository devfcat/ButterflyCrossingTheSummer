using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum SCGs
{
    None = 0,
    MHR_Dialogue1 = 1,
    MHR_Dialogue2 = 2,
    MHR_Normal1 = 3,
    MHR_Normal2 = 4,
    MHR_Enjoy = 5,
    MHR_Aru = 6,
    MSE_Dialogue1 = 7,
    MSE_Dialogue2 = 8,
    MSE_Normal1 = 9,
    MSE_Normal2 = 10,
    MHR_Laugh = 11,
    MHR_EyeLaugh = 12,
    MSE_DialogueLaugh = 13,
    MSE_EyeLaugh = 14,
    MSE_Eyemove = 15,

}

// 캐릭터 스탠딩 일러스트를 적용하는 매니저
public class SCG : MonoBehaviour
{
    public SCGs m_scg;
    public Image scg_image;
    public List<Sprite> scg_list;
    public Animator animator;

    private static SCG _instance;
    public static SCG Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SCG)) as SCG;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // 스탠딩 일러스트 설정
    public void SetSCG(string scg, bool isChangeSoft = false)
    {
        // scg가 null이거나 빈 문자열이면 None으로 처리
        if (string.IsNullOrEmpty(scg))
        {
            m_scg = SCGs.None;
            animator.SetBool("isActive", false);
            Debug.Log("scg가 null이므로 SetSCG 종료");
            return;
        }

        // Enum.TryParse를 사용하여 안전하게 파싱
        if (Enum.TryParse<SCGs>(scg, out SCGs newScg))
        {
            /*
            // 이전 scg와 동일하면 return   
            if (m_scg == newScg)
            {
                return;
            }
            */

            m_scg = newScg;
            
            animator.SetBool("isChangeSoft", isChangeSoft);

            if (m_scg == SCGs.None)
            {
                animator.SetBool("isActive", false);
                return;
            }

            // 인덱스 범위 체크
            if ((int)m_scg < scg_list.Count)
            {
                scg_image.sprite = scg_list[(int)m_scg];
                animator.SetBool("isActive", true);
            }
            else
            {
                Debug.LogWarning($"SCG 인덱스 {(int)m_scg}가 scg_list 범위를 벗어났습니다. 리스트 크기: {scg_list.Count}");
                animator.SetBool("isActive", false);
            }
        }
        else
        {
            Debug.LogWarning($"유효하지 않은 SCG 값: '{scg}'. None으로 설정합니다.");
            m_scg = SCGs.None;
            animator.SetBool("isActive", false);
        }
    }
}
