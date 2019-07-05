using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Camera mainCamera;
    private void OnEnable()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(mainCamera.transform);
    }
}
