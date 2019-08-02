using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit_AnimationController : MonoBehaviour
{
    #region References
    Unit unit;
    public ParticleSystem DamageEffect;
    public ParticleSystem DestroyEffect;
    #endregion
    Vector3 target;
    List<Vector3> wayPointList = new List<Vector3>();
    private int wayPointIndex = 1;

    float movementSpeed = 3;
    float rotationSpeed = 7;

    Quaternion startRotation;
    Quaternion endRotation;    
    Vector3 lookingDirection;
    Unit _rotationTarget;

    //States
    bool move = false;
    bool rotate = false;
    public bool IsMovingToTarget { get; private set; }
    public bool IsRotatingToTarget { get; private set; }
    #region Events
    public event Action OnReachedLastWayPoint;
    public event Action<Unit> OnRotationComplete ;
    #endregion

    public void InitMovement()
    {
        wayPointList = Core.Controller.ArrowBuilder.CreateMovementPath();      
        wayPointIndex = 1;//Starts at one because the first entry is the current position of the unit.
        target = wayPointList[wayPointIndex];//Set the first target for the movement.
        lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target.
        startRotation = Quaternion.LookRotation(this.transform.forward);
        endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target

        IsMovingToTarget = true; //Init the sequencer in the update function...
        rotate = true;//...and start rotating towards the first waypoint.            
    }
    public void InitRotation(Unit targetUnit)
    {
        lookingDirection = (targetUnit.transform.position - transform.position).normalized;
        startRotation = Quaternion.LookRotation(this.transform.forward);
        endRotation = Quaternion.LookRotation(lookingDirection);
        IsRotatingToTarget = true;
        _rotationTarget = targetUnit;
    }
    private void Start()
    {
        unit = this.GetComponent<Unit>();
        IsMovingToTarget = false;
        IsRotatingToTarget = false;
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
                wayPointIndex++;
                if (wayPointIndex >= wayPointList.Count)
                //Destination reached
                {
                    IsMovingToTarget = false;
                    wayPointIndex = 1;
                    unit.DisplayHealth(true);
                    OnReachedLastWayPoint();
                }
                else
                //Keep on moving
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
        if (IsRotatingToTarget)
        {
            startRotation = this.transform.rotation;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, rotationSpeed * Time.deltaTime);
            if (RotationComplete() || lookingDirection == Vector3.zero)
            {

                Debug.Log("start rotation: " + startRotation);

                Debug.Log("end rotation: " + endRotation);
                IsRotatingToTarget = false;
                unit.DisplayHealth(true);
                OnRotationComplete(_rotationTarget);
            }
        }
    }

    #region Effects
    public void PlayDestroyEffect()
    {
        DestroyEffect.gameObject.SetActive(true);
        DestroyEffect.Play();
    }
    public void PlayDamageEffect()
    {
        DamageEffect.gameObject.SetActive(true);
        DamageEffect.Play();
        StartCoroutine(StopDamageEffectDelayed(DamageEffect.main.duration));
    }
    IEnumerator StopDamageEffectDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        DamageEffect.Stop();
        DamageEffect.gameObject.SetActive(false);
    }
    #endregion
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
        if (Vector3.Angle(this.transform.forward, lookingDirection) < 0.5) return true;       
        else return false;      
    }
    #endregion
}
