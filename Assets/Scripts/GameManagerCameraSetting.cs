using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCameraSetting : MonoBehaviour
{
    public Canvas targetCanvas;

    void Start()
    {
        AssignCameraToCanvas();
    }

    void OnEnable()
    {
        AssignCameraToCanvas();
    }

    void AssignCameraToCanvas()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponent<Canvas>();
        }

        Camera mainCam = Camera.main;
        if (targetCanvas != null && mainCam != null)
        {
            targetCanvas.worldCamera = mainCam;
        }
    }
}
