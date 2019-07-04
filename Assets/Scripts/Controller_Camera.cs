using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Camera : MonoBehaviour
{
    public bool Active;
    float CameraSpeed = 2;
    float maxTopAngle = 70;
    float maxDownAngle = 20;
    float maxDownY = 5;
    Camera mainCamera;
    Transform _cursor;
    Vector3 _cursorX = new Vector3(1, 0, 0);
    Vector3 _cursorY = new Vector3(0, 1, 0);
    Vector3 _cursorZ = new Vector3(0, 0, 1);

    public void Init()
    {
        mainCamera = Camera.main;
        _cursor = Core.Controller.Cursor.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Active)
        {
            #region Right Stick
            //Right
            if (Input.GetAxis("Right Stick Horizontal") > 0)
            {
                Debug.Log("Right");
                Rotate(_cursor, _cursorY, -CameraSpeed);
            }
            //Left
            else if (Input.GetAxis("Right Stick Horizontal") < 0)
            {
                Debug.Log("Left");
                Rotate(_cursor, _cursorY, CameraSpeed);
            }
            //Up
            else if (Input.GetAxis("Right Stick Vertical") > 0)
            {
                Debug.Log("Up");
                RotateUp(_cursor, _cursorX, CameraSpeed);
            }
            //Down
            else if (Input.GetAxis("Right Stick Vertical") < 0)
            {
                Debug.Log("Down");
                RotateDown(_cursor, _cursorX, -CameraSpeed);
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
        }
    }

    void Rotate(Transform origin, Vector3 axis, float step)
    {
        this.transform.RotateAround(origin.position, axis, step);
    }
    void RotateUp(Transform origin, Vector3 axis, float step)
    {
        if (IsInsideTopBounds()) Rotate(origin, axis, step);
        else mainCamera.transform.eulerAngles = new Vector3(maxTopAngle, 0, 0);
    }
    void RotateDown(Transform origin, Vector3 axis, float step)
    {
        if (IsInsideDownBounds()) Rotate(origin, axis, step);
        else
        {
            mainCamera.transform.eulerAngles = new Vector3(maxDownAngle, 0, 0);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, maxDownY, mainCamera.transform.position.z);
        }
    } 
    bool IsInsideTopBounds()
    {
        if (mainCamera.transform.eulerAngles.x < maxTopAngle) return true;
        else return false;
    }
    bool IsInsideDownBounds()
    {
        if (mainCamera.transform.eulerAngles.x > maxDownAngle && mainCamera.transform.position.y > maxDownY)return true;       
        else return false;
    }
    void CorrectTopBounds()
    {

    }


    //Vector3 CalcXAxis(Vector3 target, Vector3 origin)
    //{
    //    Vector3 vectorToTarget = target - origin;
    //    return Vector3.Cross(vectorToTarget, mainCamera.transform.up);
    //}
    //Vector3 CalcYAxis(Vector3 target, Vector3 origin)
    //{
    //    Vector3 vectorToTarget = target - origin;
    //    Vector3 vector = Vector3.Cross(vectorToTarget, mainCamera.transform.right);
    //    vector = new Vector3(0, 0, 0);
    //    return vector;
    //}
}
