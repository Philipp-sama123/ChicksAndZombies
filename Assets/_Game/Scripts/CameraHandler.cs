using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Camera to assign")] public GameObject AimCamera;
    public GameObject AimCanvas;
    public GameObject ThirdPersonCamera;
    public GameObject ThirdPersonCanvas;

    public void HandleAiming(bool isAiming)
    {
        if (isAiming)
        {
            ThirdPersonCamera.SetActive(false);
            ThirdPersonCanvas.SetActive(false);
            AimCanvas.SetActive(true);
            AimCamera.SetActive(true);
        }
        else
        {
            ThirdPersonCamera.SetActive(true);
            ThirdPersonCanvas.SetActive(true);
            AimCanvas.SetActive(false);
            AimCamera.SetActive(false);
        }
    }
}