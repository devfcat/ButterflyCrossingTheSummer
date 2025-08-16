using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Bgm 오디오 클립 인덱스
/// </summary>
public enum BGM
{
    Title = 0,
    Office_Calm = 1,
    Jazz1 = 2,
    Night1 = 3,
    BGM_Grasses = 4,
    BGM_Garden1 = 5,
    Park_Night = 6,
    Office_Loud = 7,
    BGM_Daily1 = 8,
    BGM_Cute1 = 9,
    BGM_Garden2 = 10,
    BGM_Cute3 = 11,
    BGM_Office_Rain = 12,
    BGM_RainAndUmbrella = 13,
    BGM_LowDaily1 = 14,
    BGM_DrugStoreInside = 15,
    BGM_Bakery = 16,
    BGM_Birthday = 17,
    BGM_Cute2 = 18,
    BGM_Cafe = 19,
    BGM_Cicada = 20,
    BGM_SadSoda = 21,
    BGM_Hospital2 = 22,
    BGM_GardenNight = 23,
    BGM_GardenDawn = 24,
    BGM_Clock = 25,
    BGM_LowDaily3 = 26,
    BGM_LowDaily2 = 27,
    BGM_HarinSad1 = 28,
    BGM_SESad = 29,
    BGM_SESad2 = 30,
    BGM_Ending1 = 31,
    BGM_Ending2 = 32,
    BGM_Ending3 = 33,
    BGM_Train = 34,
    BGM_Fall = 35,
    BGM_FakeEnd = 36,
    BGM_RealEnd = 37,
}

/// <summary>
/// Bgm 재생 상태
/// </summary>
public enum BgmStatus
{
    Play, // 재생 중
    Stop, // 재생 안함
    Pause // 일시 정지
}

/// <summary>
/// 효과음 오디오 클립 인덱스
/// </summary>
public enum SFX
{
    UI = 0,
    SE_KakaoTalk = 1,
    SE_ClickingGlass = 2,
    SE_LighterOn = 3,
    SE_DoorOpen = 4,
    SE_Punch = 5,
    SE_EatSnack = 6,
    SE_Mug = 7,
    SE_OpenBottle = 8,
    SE_Spring = 9,
    SE_Breath = 10,
    SE_Grasses = 11,
    SE_Wind = 12,
    SE_WaterWalk = 13,
    SE_BusEngine = 14,
    SE_Barcode = 15,
    SE_TorchOn = 16,
    SE_Dingdong = 17,
    SE_Shutter = 18,
    SE_Page1 = 19,
    SE_Page2 = 20,
    SE_Step = 21,
    SE_Call = 22,
    SE_CarDoor = 23,
    SE_Knock = 24,
    SE_RainAndUmbrella = 25,
    SE_WalkGarden = 26,
}

public class SoundManager : MonoBehaviour
{
    [Header("AudioClips")] // 인스펙터 창에서 직접 파일 추가
    [SerializeField, Tooltip("Bgm 클립 리스트")] private List<AudioClip> bgmList; 
    [SerializeField, Tooltip("Se 클립 리스트")] private List<AudioClip> sfxList; 

    private AudioSource bgmPlayer; // Bgm AudioSource, SoundManager 첫 번째 자식이어야 함 
    private AudioSource sfxPlayer; // Sfx AudioSource, SoundManager 두 번째 자식이어야 함 

    [Header("Volume")] // 볼륨 상태 디버깅용
    [ReadOnly, SerializeField, Range(0, 1), Tooltip("Bgm 볼륨")] private float Volume1 = 1f;
    [ReadOnly, SerializeField, Range(0, 1), Tooltip("Sfx 볼륨")] private float Volume2 = 1f;

    private float temp_bgm;
    private float temp_sfx;
    
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    /// <summary>
    /// 저장된 볼륨 불러오기 메소드
    /// </summary>
    private void GetVolumePref()
    {
        if (PlayerPrefs.GetFloat("Volume") == 0)
        {
            // Debug.Log("No saved volume");
            PlayerPrefs.SetFloat("BgmVolume", 1f);
            PlayerPrefs.SetFloat("SfxVolume", 1f);
            PlayerPrefs.SetFloat("Volume", 1f);
            BgmVolume = 1f;
            SfxVolume = 1f;
        }
        else
        {
            BgmVolume = PlayerPrefs.GetFloat("BgmVolume");
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume");
            bgmPlayer.volume = BgmVolume;
            sfxPlayer.volume = SfxVolume;
            Debug.Log(string.Format("BgmVolume : {0}, SfxVolume : {1}", BgmVolume, SfxVolume));
        }
    }

    /// <summary>
    /// 볼륨 설정 저장 메소드
    /// </summary>
    public void SetVolumePref()
    {
        // Debug.Log("Save volume");
        PlayerPrefs.SetFloat("BgmVolume", BgmVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.SetFloat("Volume", 1f);
    }
    
    // Bgm AudioSource 볼륨 크기
    private float bgmVolume;
    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            bgmPlayer.volume = bgmVolume;
            BgmStatusCheck();
            Volume1 = bgmVolume;

        }
    }
    // Sfx AudioSource 볼륨 크기
    private float sfxVolume;
    public float SfxVolume
    {
        get
        {
            return sfxVolume;
        }
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            sfxPlayer.volume = sfxVolume;
            Volume2 = sfxVolume;
        }
    }

    private void Awake()
    {
        InstanceCheck();
        Init();
        GetVolumePref();
    }

    /// <summary>
    /// 인스턴스 관리 메소드
    /// </summary>
    private void InstanceCheck()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Control_Audio(bool isOn)
    {
        if (!isOn) // 소리 끄기
        {
            // 임시로 사운드 수치 저장
            temp_bgm = BgmVolume;
            temp_sfx = SfxVolume;

            BgmVolume = 0;
            SfxVolume = 0;
        }
        else // 소리 켜기
        {
            // 임시 저장된 값이 없다면
            if (temp_bgm == 0 || temp_sfx == 0)
            {
                temp_bgm = 1;
                temp_sfx = 1;
            }

            BgmVolume = temp_bgm;
            SfxVolume = temp_sfx;
        }

        bgmPlayer.volume = BgmVolume;
        sfxPlayer.volume = SfxVolume;

        SetVolumePref();
    }

    // BGM 소리 조절
    public void OnValueChange_BgmVolume(Slider s)
    {
        /* 해당 씬에서 사용
        bgmS.value = SoundManager.Instance.BgmVolume;
        sfxS.value = SoundManager.Instance.SfxVolume;
        */
        BgmVolume = s.value;
        SetVolumePref();
    }

    // SE 소리 조절
    public void OnValueChange_SfxVolume(Slider s)
    {
        SfxVolume = s.value;
        SetVolumePref();
    }

    private void Init()
    {
        bgmPlayer = GetComponentsInChildren<AudioSource>()[0];
        sfxPlayer = GetComponentsInChildren<AudioSource>()[1];
        
        // 크레딧 씬에서는 브금을 무한재생하지 않는다
        if (GameManager.Instance.m_State == eState.Credits)
        {
            // 브금을 무한재생하지 않는다
            bgmPlayer.loop = false;
        }
        else bgmPlayer.loop = true;
    }

    /// <summary>
    /// Bgm AudioSource 상태 확인
    /// </summary>
    private void BgmStatusCheck()
    {
        if (bgmPlayer.volume == 0 && bgmPlayer.isPlaying) // 볼륨 0일 때 재생 정지
        {
            // Debug.Log("Muted");
            // BgmControl(BgmStatus.Stop);
        }
        else if (bgmPlayer.volume > 0 && !bgmPlayer.isPlaying) // 볼륨이 0이 아니게 됐을 때 재생
        {
            // Debug.Log("Unmuted");
            BgmControl(BgmStatus.Play);
        }
    }

    /// <summary>
    /// Bgm Clip 선택 후 재생 메소드
    /// </summary>
    /// <param name="bgm">재생할 Bgm 파일 인덱스</param>
    public void PlayBGM(BGM bgm)
    {
        // 현재 재생 중인 BGM과 같은 BGM이면 재생하지 않음
        if (bgmPlayer.clip == bgmList[(int)bgm] && bgmPlayer.isPlaying)
        {
            Debug.Log($"같은 BGM이 재생 중이므로 재생하지 않음: {bgm}");
            return;
        }
        
        Debug.Log($"새로운 BGM 재생: {bgm}");
        bgmPlayer.clip = bgmList[(int)bgm];
        if (bgmPlayer.volume > 0) BgmControl(BgmStatus.Play); // 음소거 시 클립 설정 후 재생하지 않음
    }

    /// <summary>
    /// Sfx Clip 선택 후 재생 메소드
    /// </summary>
    /// <param name="sfx">재생할 Sfx 파일 인덱스</param>
    public void PlaySFX(SFX sfx)
    {
        if (sfxPlayer.volume > 0) // 음소거 시 재생하지 않음
        {
            if (sfxPlayer.isPlaying) sfxPlayer.Stop();

            sfxPlayer.clip = sfxList[(int)sfx];
            sfxPlayer.Play(); // 바로 교체 재생
        }
    }

    public void StopBGM()
    {
        bgmPlayer.clip = null;
        bgmPlayer.Stop();
    }
    public void StopSFX()
    {
        sfxPlayer.clip = null;
        sfxPlayer.Stop();
    }

    /// <summary>
    /// Bgm 상태 변경
    /// </summary>
    /// <param name="status">Play : 재생, Stop : 음소거, Pause : 일시정지</param>
    public void BgmControl(BgmStatus status)
    {
        switch (status)
        {
            case BgmStatus.Play:
                if (bgmPlayer.isPlaying) return;
                bgmPlayer.Play();
                break;
            case BgmStatus.Stop:
                bgmPlayer.Stop();
                break;
            case BgmStatus.Pause:
                bgmPlayer.Pause();
                break;
        }
    }
}
