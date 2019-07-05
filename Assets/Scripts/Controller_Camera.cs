using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Camera : MonoBehaviour
{
    public Transform CameraTarget;
    Vector2Int _cameraTargetPos;
    public bool Active;
    float CameraSpeed = 50;
    float ZoomSpeed = 10;
    //Bounds
    float minHorizontalAngle = 5;
    float maxHorizontalAngle = 175;
    float minVerticalAngle = 5;
    float maxVerticalAngle = 90;
    float minZoomDistance = 1;
    float maxZoomDistance = 12;
    Camera mainCamera;
   
   
    public void Init()
    {
        mainCamera = Camera.main;
        //mainCamera.transform.LookAt(CameraTarget);
        SetTargetPos(Core.Controller.Cursor.Position);
    }

    // Update is called once per frame
    void Update()
    {
        float speed = CameraSpeed * Time.deltaTime;
        if(Active)
        {
            #region Right Stick
            //Right
            if (Input.GetAxis("Right Stick Horizontal") > 0)
            {
                if(IsInHorizontalBounds()) RotateHorizontal(-speed);
                else CorrectHorizontalBounds();
            }
            //Left
            else if (Input.GetAxis("Right Stick Horizontal") < 0)
            {
                if (IsInHorizontalBounds()) RotateHorizontal(speed);
                else CorrectHorizontalBounds();
            }
            //Up
            else if (Input.GetAxis("Right Stick Vertical") > 0)
            {
                if (IsInVerticalBounds()) RotateVertical(speed);
                else CorrectVerticalBounds();
            }
            //Down
            else if (Input.GetAxis("Right Stick Vertical") < 0)
            {
                if (IsInVerticalBounds()) RotateVertical(-speed);
                else CorrectVerticalBounds();
            }
            #endregion
            #region Left Stick
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
            
            }
            else if (Input.GetAxisRaw("Vertical") < 0)           
            {
           
            }       
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
            
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
            
            }
            #endregion
            #region Triggers
            if(Input.GetAxis("RT") > 0)
            {
                if (IsInZoomBounds()) Zoom(ZoomSpeed);
                else CorrectZoomPosition();
            }
            else if(Input.GetAxis("LT") > 0)
            {
                if (IsInZoomBounds()) Zoom(-ZoomSpeed);
                else CorrectZoomPosition();

            }
            #endregion
        }
    }

    #region Rotation
    void RotateVertical(float speed)
    {
        //mainCamera.transform.RotateAround(target.position, axis, step);
        CameraTarget.Rotate(CameraTarget.forward, speed, Space.World);
    }
    void RotateHorizontal(float speed)
    {
        CameraTarget.Rotate(Vector3.up, speed, Space.World);    }

    bool IsInHorizontalBounds()
    {
        if (minHorizontalAngle <= CameraTarget.eulerAngles.y && CameraTarget.eulerAngles.y <= maxHorizontalAngle) return true;
        else return false;
    }
    void CorrectHorizontalBounds()
    {
        if (CameraTarget.eulerAngles.y < minHorizontalAngle)
        {
            CameraTarget.eulerAngles = new Vector3(CameraTarget.eulerAngles.x, minHorizontalAngle, CameraTarget.eulerAngles.z);
        }
        if (CameraTarget.eulerAngles.y > maxHorizontalAngle)
        {
            CameraTarget.eulerAngles = new Vector3(CameraTarget.eulerAngles.x, maxHorizontalAngle, CameraTarget.eulerAngles.z);
        }
    }
    bool IsInVerticalBounds()
    {
        if (minVerticalAngle <= CameraTarget.eulerAngles.z && CameraTarget.eulerAngles.z <= maxVerticalAngle) return true;
        else return false;
    }

    void CorrectVerticalBounds()
    {
        if (CameraTarget.eulerAngles.z < minVerticalAngle)
        {
            CameraTarget.eulerAngles = new Vector3(CameraTarget.eulerAngles.x, CameraTarget.eulerAngles.y, minVerticalAngle);
        }
        if (CameraTarget.eulerAngles.z > maxVerticalAngle)
        {
            CameraTarget.eulerAngles = new Vector3(CameraTarget.eulerAngles.x, CameraTarget.eulerAngles.y, maxVerticalAngle);
        }
    }
    #endregion
    #region Zoom
    void Zoom(float speed)
    {
        mainCamera.transform.Translate(mainCamera.transform.forward * speed * Time.deltaTime, Space.World);
    }
    bool IsInZoomBounds()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, CameraTarget.position);
        if (distance >= minZoomDistance && distance <= maxZoomDistance) return true;
        else return false;
    }
    void CorrectZoomPosition()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, CameraTarget.position);
        if(distance < minZoomDistance) mainCamera.transform.Translate(mainCamera.transform.forward * -0.1f, Space.World);
        if (distance > maxZoomDistance) mainCamera.transform.Translate(mainCamera.transform.forward * 0.1f, Space.World);
    }
    #endregion

    public void SetTargetPos(Vector2Int pos)
    {
        _cameraTargetPos = pos;
        CameraTarget.transform.position = new Vector3(pos.x, 0, pos.y);
    }
    public Vector2Int GetTargetPos()
    {
        return _cameraTargetPos;
    }

}
