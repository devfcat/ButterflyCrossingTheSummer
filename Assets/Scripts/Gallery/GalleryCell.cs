using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryCell : MonoBehaviour
{
    public ECG id;

    public void OnClick_Cell()
    {
        SoundManager.Instance.PlaySFX(SFX.UI);
        Panel_Gallery.Instance.OnClick_Cell(id);
    }
}
