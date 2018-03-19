using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    float height = -0.05f;
    Vector3 target;
    List<Vector3> wayPointList = new List<Vector3>();
    private int wayPointIndex = 0;

    public float speed = 2;
    public float rotationSpeed = 2;

    public Quaternion startRotation;
    public Quaternion endRotation;
    public Vector3 _direction;

    public bool move = false;
    public bool rotate = false;
    public bool stepMove = false;
    // Use this for initialization
    void Start()
    {
        wayPointList.Add(new Vector3(0, height, 4));
        wayPointList.Add(new Vector3(-4, height, 4));
        wayPointList.Add(new Vector3(-4, height, -4));
        wayPointList.Add(new Vector3(0, height, -4));

        target = wayPointList[wayPointIndex];
        _direction = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
        startRotation = Quaternion.LookRotation(this.transform.position);
        endRotation = Quaternion.LookRotation(_direction);//The actual rotation we need to look at the target

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp("1"))
        {
            Debug.Log("Move only!");
            move = move == true ? false : true;

            Debug.Log(move);
        }
        if (Input.GetKeyUp("2"))
        {
            Debug.Log("Rotate only!");
            rotate = rotate == true ? false : true;
        }
        if (Input.GetKeyUp("3"))//Means: first rotate until you look at the target, then move to the target, then set the next target, look at it, move to it and so on.
        {
            Debug.Log("Rotate then move.");
            stepMove = stepMove == true ? false : true;
        }

        if (move)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(this.transform.position, target, step);
        }

        if (rotate)
        {
            startRotation = this.transform.rotation;//Quaternion.LookRotation(this.transform.position);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationSpeed * Time.deltaTime);//Do the rotation with a slerp
        }

        if (stepMove)
        {
            if (rotationComplete())
            {
                Debug.Log("Rotation complete");
                rotate = false;
                move = true;
            }

            if (wayPointReached(wayPointList[wayPointIndex]))
            {
                Debug.Log("Reached waypoint: " + wayPointIndex);
                wayPointIndex++;
                if (wayPointIndex >= wayPointList.Count)//Start from the beginning again.
                {
                    wayPointIndex = 0;
                }
                target = wayPointList[wayPointIndex];
                _direction = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
                endRotation = Quaternion.LookRotation(_direction);//The actual rotation we need to look at the target.  

                move = false;
                rotate = true;
            }
        }
        Debug.DrawRay(this.transform.position, this.transform.forward * 3);
        Debug.DrawLine(this.transform.position, target, Color.green);
    }

    private bool wayPointReached(Vector3 nextWaypoint)
    {
        if (nextWaypoint == this.transform.position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //If the forward vector of the unit aligns with the vector from the unit to the target, we finished the rotation.
    private bool rotationComplete()
    {
        if (this.transform.forward == _direction)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
