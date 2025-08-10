using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Gallery : MonoBehaviour
{
    [Header("갤러리의 이미지들")]
    public List<Sprite> ecg_images; // ECG와 순서 동일해야 함
    public List<GameObject> ecg_list;
    public Sprite default_image;

    [Header("갤러리 관련 인터페이스")]
    public GameObject Popup_Gallery_Full;
    public Image full_image;

    public int m_index;

    private static Panel_Gallery instance;
    public static Panel_Gallery Instance
    {
        get {
            if(!instance)
            {
                instance = FindObjectOfType(typeof(Panel_Gallery)) as Panel_Gallery;

                if (instance == null)
                    Debug.Log("no Singleton obj");
            }
            return instance;
        }
    }

    void OnEnable()
    {
        Popup_Gallery_Full.SetActive(false);

        Set_Gallery();
    }

    public void Close()
    {
        GameManager.Instance.Control_Popup(false);
    }

    // 각 갤러리 아이디를 인식해 해금된 이미지를 설정
    public void Set_Gallery()
    {
        for (int i = 0; i < ecg_list.Count; i++)
        {
            // 현재 셀이 가진 아이디가 해금되었는지 여부 판별
            ECG m_cellID = ecg_list[i].GetComponent<GalleryCell>().id;
            if (SaveManager.Instance.Check_ECGData(m_cellID))
            {
                ecg_list[i].GetComponent<Image>().sprite = ecg_images[i+1];
                ecg_list[i].GetComponent<Button>().enabled = true;
            }
            else
            {
                ecg_list[i].GetComponent<Image>().sprite = default_image;
                ecg_list[i].GetComponent<Button>().enabled = false;
            }
        }
    }

    public void OnClick_Cell(ECG id)
    {
        full_image.sprite = ecg_images[(int)id];
        Popup_Gallery_Full.SetActive(true);
    }

    public void OnClick_CloseCell()
    {
        Popup_Gallery_Full.SetActive(false);
    }

}
