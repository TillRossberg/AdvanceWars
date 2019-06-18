﻿using System.Collections;
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
        unit = this.GetComponent<Unit>();
        IsMovingToTarget = false;
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

            if (RotationComplete() && rotate)
            {
                rotate = false;
                move = true;
                //TODO: Stop rotation sound.
                //TODO: Play move sound.
            }

            if (WayPointReached(wayPointList[wayPointIndex]) && move)
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
                        if (unitWantsToLoad) Core.View.ContextMenu.ShowLoadButton();
                        else if (unitWantsToUnite) Debug.Log("Unit wants to unite!");
                        else
                        {
                            unit.FindAttackableEnemies(Core.Controller.SelectedTile.Position);
                            Core.View.ContextMenu.Show(unit);
                        }
                    }
                    else unit.GetInterrupted();
                }
                else
                {
                    target = wayPointList[wayPointIndex];
                    lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
                    endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target.  
                    //TODO: Stop move sound.
                    //TODO: Play rotation sound.
                    move = false;
                    rotate = true;
                }
            }            
        }
    }

    #region Conditions
    //TODO: Move to Animation Controller
    private bool WayPointReached(Vector3 nextWaypoint)
    {
        if (nextWaypoint == this.transform.position)return true;        
        else return false;        
    }
    //If the forward vector of the unit aligns with the vector from the unit to the target, we finished the rotation.
    private bool RotationComplete()
    {
        if (Vector3.Angle(this.transform.forward, lookingDirection) < 1) return true;       
        else return false;      
    }

    #endregion
}