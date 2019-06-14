using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_AnimationController : MonoBehaviour
{
    #region References
    Unit unit;
    #endregion
    Vector3 target;
    List<Vector3> wayPointList = new List<Vector3>();
    private int wayPointIndex = 1;

    float movementSpeed = 3;
    float rotationSpeed = 7;

    Quaternion startRotation;
    Quaternion endRotation;
    Vector3 lookingDirection;

    public bool unitWantsToUnite;
    public bool unitWantsToLoad;
    //States

    bool move = false;
    bool rotate = false;
    public bool IsMovingToTarget { get; private set; }

    public void Init()
    {
        wayPointList = Core.Controller.ArrowBuilder.CreateMovementPath();
        wayPointIndex = 1;//Starts at one because the first entry is the current position of the unit.
        target = wayPointList[wayPointIndex];//Set the first target for the movement.
        lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target.
        startRotation = Quaternion.LookRotation(this.transform.position);
        endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target
        IsMovingToTarget = true; //Init the sequencer in the update function...
        rotate = true;//...and start rotating towards the first waypoint.        
        unitWantsToUnite = false;
        unitWantsToLoad = false;
    }
    private void Start()
    {
        IsMovingToTarget = false;
        unit = this.GetComponent<Unit>();
    }
    // Update is called once per frame
    void Update()
    {
        //Rotate the unit towards a point the move it to this point, then get the next point and so on.
        if (IsMovingToTarget)
        {
            if (move)
            {
                //Debug.Log("Moving!");
                float step = movementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(this.transform.position, target, step);
            }

            if (rotate)
            {
                //Debug.Log("Rotating!");
                startRotation = this.transform.rotation;
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, rotationSpeed * Time.deltaTime);//Do the rotation with a lerp.
            }

            if (rotationComplete() && rotate)
            {
                rotate = false;
                move = true;
                //TODO: Stop rotation sound.
                //TODO: Play move sound.
            }

            if (wayPointReached(wayPointList[wayPointIndex]) && move)
            {
                //Debug.Log("Reached waypoint: " + wayPointIndex);
                wayPointIndex++;
                if (wayPointIndex >= wayPointList.Count)
                {
                    IsMovingToTarget = false;
                    wayPointIndex = 1;
                    unit.DisplayHealth(true);
                    if (!unit.IsInterrupted)
                    {
                        //TODO: add event to inform that we reached the last waypoint.
                    }
                    else
                    {
                        unit.Wait();
                        //TODO: Show exclamation mark.
                        //TODO: Stop move sound.
                        //TODO: Play interruption sound
                    }
                }
                target = wayPointList[wayPointIndex];
                lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
                endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target.  
                //TODO: Stop move sound.
                //TODO: Play rotation sound.
                move = false;
                rotate = true;
            }
            //Debug.DrawRay(this.transform.position, this.transform.forward * 2);
            //Debug.DrawLine(this.transform.position, target, Color.green);
        }
    }

    #region Conditions
    //TODO: Move to Animation Controller
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
        if (Vector3.Angle(this.transform.forward, lookingDirection) < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
