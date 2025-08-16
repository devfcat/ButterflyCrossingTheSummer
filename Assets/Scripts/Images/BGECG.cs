using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BG
{
    None = 0,
    Office_Night = 1,
    Bar_Inside = 2,
    CityRoad_Night = 3,
    Park_Night = 4,
    Garden_BG = 5,
    Office_Day = 6,
    Station_BG = 7,
    Bus_BG = 8,
    Playground_BG = 9,
    HarinsRoom_BG = 10,
    Office_Rain_BG = 11,
    CityRoad_Rain_BG = 12,
    DrugStoreInside_BG = 13,
    Bakery_BG = 14,
    ForPlayground_BG = 15,
    Office_Dinner_BG = 16,
    FastFood_BG = 17,
    Cafe_BG = 18,
    Office_Light_BG = 19,
    HarinHouseFront_BG = 20,
    HarinVillInside_BG = 21,
    HospitalInside_BG = 22,
    Garden_Night_BG = 23,
    Garden_Sunset_BG = 24,
    GardenDawn_BG = 25,
    SchoolOutside_BG = 26,
    SchoolStair_BG = 27,
    SchoolRooftop_BG = 28,
    HarinsRoomMorning_BG = 29,
    RoadForCompany_BG = 30,
    SchoolGate_BG = 31,
    SchoolGoHome_BG = 32,
    HarinCompany_BG = 33,
    MedicalRoom_BG1 = 34,
    MedicalRoom_BG2 = 35,
    MedicalRoom_BG3 = 36,
    MedicalRoom_BG4 = 37,
    StationNight_BG = 38,
    Fall_BG = 39,
    FirstSnow_BG = 40,
    HarinsRoomNight_BG = 41,
    GoForDoor_BG1 = 42,
    GoForDoor_BG2 = 43,
    GoForDoor_BG3 = 44,
    Garden_DeepNight_BG = 45,
    PlaygroundMorning_BG = 46,
    ForPlaygroundMorning_BG = 47,
    CountryStation_BG = 48,
    CountryBusStation_BG = 49,
    Reeds_BG = 50,
    MHR_ECG5_1 = 51,
    MHR_ECG5_2 = 52,
    SD_MHR1 = 53,
    Comics1_1 = 54,
    Comics1_2 = 55,
    Comics1_3 = 56,
    Comics2_1 = 57,
    Comics2_2 = 58,
    Comics3_1 = 59,
    Comics3_2 = 60,
    Comics4_1 = 61,
    Comics4_2 = 62,
    Comics4_3 = 63,
    Comics5_1 = 64,
    Comics5_2 = 65,
    Comics5_3 = 66,
    Comics9_1 = 67,
    Comics9_2 = 68,
    Comics9_3 = 69,
    AroundCompany_BG = 70
}

public enum ECG
{
    None = 0,
    MHR_ECG1_1 = 1,
    MHR_ECG1_2 = 2,
    MSE_ECG1 = 3,
    MHR_ECG2 = 4,
    MSE_ECG2 = 5,
    MHR_ECG3 = 6,
    MSE_ECG3_1 = 7, 
    MSE_ECG5 = 8,
    MHR_ECG5_1 = 9,
    MHR_ECG5_2 = 10,
    MHR_ECG6 = 11,
    MHR_ECG4 = 12,
    MSE_ECG6_Sunset = 13,
    MSE_ECG6_Morning = 14,
    MSE_ECG6_Night = 15,
}

// 배경, 캐릭터 특별 일러스트를 관리하는 스크립트
public class BGECG : MonoBehaviour
{
    public Image bg_image;
    public Image ecg_animation_image;  // 캐릭터 일러스트는 애니메이션 패널과 bg 패널에 함께 넣음
    public GameObject ecg_panel;
    public ECG m_ecg;
    public BG m_bg;

    public List<Sprite> bg_list;
    public List<Sprite> ecg_list;
    public Animator animator;

    private static BGECG _instance;
    public static BGECG Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(BGECG)) as BGECG;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    void Start()
    {
        //ClearBG(); // 시작할 때 배경을 지우지 않음
        //ClearECG();
    }

    public void SetBG(string bg, bool isChangeSoft = false)
    {
        // 만약 ecg가 있다면 배경을 지우지 않고 ecg를 넣음
        if (m_ecg != ECG.None)
        {
            return;
        }

        Debug.Log($"SetBG 호출됨 - bg: {bg}, isChangeSoft: {isChangeSoft}, 현재 m_bg: {m_bg}");
        
        if (System.Enum.TryParse<BG>(bg, out BG bgEnum))
        {
            // 이전 배경과 동일하면 return
            /*
            if (m_bg == bgEnum)
            {
                Debug.Log($"이전 배경과 동일하므로 SetBG 종료: {bgEnum}");
                return;
            }
            */
            
            m_bg = bgEnum;
            
            Debug.Log($"SetBG: {bgEnum}");
            
            // BG.None이면 배경을 지움
            if (bgEnum == BG.None)
            {
                Debug.Log("BG.None 감지, ClearBG 호출");
                ClearBG();
                return;
            }
            
            animator.SetBool("isChangeSoft", isChangeSoft);

            int index = (int)bgEnum;
            if (index >= 0 && index < bg_list.Count)
            {
                bg_image.sprite = bg_list[index];
                Debug.Log($"배경 스프라이트 설정: index {index}");
            }
            else
            {
                Debug.LogWarning($"배경 인덱스 범위 초과: index {index}, bg_list.Count {bg_list.Count}");
            }
            animator.SetBool("isActive", true);
            Debug.Log("애니메이터 On 트리거 설정");
        }
        else
        {
            Debug.LogWarning($"BG enum 파싱 실패: {bg}");
        }
    }

    public void ClearBG()
    {
        Debug.Log($"ClearBG 호출됨 - 현재 m_bg: {m_bg}");
        m_bg = BG.None;
        Debug.Log("ClearBG 실행");
        bg_image.sprite = bg_list[0];
        animator.SetBool("isActive", false);
        Debug.Log("애니메이터 Off 트리거 설정");
    }

    public void ClearECG()
    {
        m_ecg = ECG.None;
        ecg_animation_image.sprite = null;
        ecg_panel.SetActive(false);
    }

    public void SetECG(string ecg)
    {
        if (System.Enum.TryParse<ECG>(ecg, out ECG ecgEnum))
        {
            bool flash = false;
            if (m_ecg == ECG.None) // 이전 ECG가 없었으면 플래시
            {
                flash = true;
            }

            m_ecg = ecgEnum;

            int index = (int)ecgEnum;
            if (index >= 0 && index < ecg_list.Count)
            {
                bg_image.sprite = ecg_list[index];
                animator.SetBool("isActive", true);
                ecg_animation_image.sprite = ecg_list[index];
            }

            if (flash && QuickMenuManager.Instance.m_mode != Mode.skip)
            ecg_panel.SetActive(true);
        }
    }   
}
