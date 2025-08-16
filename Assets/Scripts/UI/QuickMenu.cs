using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀵메뉴의 UI 기능 (관리 기능은 매니저 스크립트에 서술)
/// </summary>
public class QuickMenu : MonoBehaviour
{
    [SerializeField] private bool isOpen; // 퀵메뉴 온인지 판별
    private Animator animator;
    
    [Header("저장 버튼 쿨다운")]
    [SerializeField] private Button saveButton; // 저장 버튼
    [SerializeField] private Image saveButtonImage; // 저장 버튼 이미지
    private bool isSaveCooldown = false; // 저장 쿨다운 상태

    void OnEnable()
    {
        Init();
        QuickMenuManager.Instance.Set_AudioBTNUI();
    }

    void OnDisable()
    {
        animator.SetTrigger("Close");
        Init();
    }

    void Init()
    {
        isOpen = false;
        animator = this.GetComponent<Animator>();
    }

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }

    // 퀵메뉴는 유저가 제어하기 전까지는 움직이지 않는다
    public void OnClick()
    {
        if(isOpen) // 퀵메뉴가 켜져 있는 상태에서 클릭했다면 퀵메뉴를 넣음
        {
            SoundManager.Instance.PlaySFX(SFX.UI);
            // 퀵메뉴 닫기
            animator.SetTrigger("Close");
        }
        else // 퀵메뉴가 꺼져 있는 상태에서 클릭했다면 퀵메뉴를 엶
        {
            SoundManager.Instance.PlaySFX(SFX.UI);
            // 퀵메뉴 열기 
            animator.SetTrigger("Open");
        }
    }

    public void OnClick_Setting()
    {
        QuickMenuManager.Instance.m_mode = Mode.normal; // 만약 다른 모드였다면 기본 모드로 초기화
        QuickMenuManager.Instance.Setting();
    }

    public void OnClick_UIOff()
    {
        if (QuickMenuManager.Instance.m_mode != Mode.normal)
        {
            QuickMenuManager.Instance.m_mode = Mode.normal;
        }
        
        SoundManager.Instance.PlaySFX(SFX.UI);
        QuickMenuManager.Instance.ControlUIImages(false);
    }

    public void OnClick_Main()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        QuickMenuManager.Instance.Home();
    }

    public void OnClick_Skip()
    {
        QuickMenuManager.Instance.Control_Skip();
    }

    public void OnClick_Log()
    {
        LogManager.Instance.Control_Log(true);
    }

    public void OnClick_Auto()
    {
        QuickMenuManager.Instance.Control_Auto();
    }

    public void OnClick_Audio()
    {
        QuickMenuManager.Instance.Control_Audio();
    }

    public void OnClick_Load()
    {
        GameManager.Instance.Control_Load(true);
    }

    public void OnClick_Save()
    {
        // 쿨다운 중이면 저장하지 않음
        if (isSaveCooldown)
        {
            return;
        }
        
        SoundManager.Instance.PlaySFX(SFX.UI);
        SaveManager.Instance.Save_Data();
        GameManager.Instance.Control_Popup(true, GameManager.Instance.m_Popup_Saved);
        
        // 쿨다운 시작
        StartCoroutine(SaveCooldown());
    }
    
    /// <summary>
    /// 저장 버튼 쿨다운 코루틴
    /// </summary>
    private IEnumerator SaveCooldown()
    {
        isSaveCooldown = true;
        
        
        if (saveButtonImage != null)
        {
            saveButtonImage.color = Color.gray;
        }
        
        // 1초 대기
        yield return new WaitForSeconds(1f);
        
        if (saveButtonImage != null)
        {
            saveButtonImage.color = Color.white;
        }
        
        isSaveCooldown = false;
    }
}
