using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AI_Unit 
{
    AI ai;
    public Unit Unit;
    public Unit attackTarget;
    public Tile moveTarget;

    bool _move = true;
    bool _act = true;

    public event Action OnAllOrdersFinished;

    #region Basic Methods
    public AI_Unit(Unit unit, AI ai)
    {
        this.Unit = unit;
        this.ai = ai;
    }   
    public void Reset()
    {
        _move = true;
        _act = true;
    }


    #endregion
    public void Start()
    {
        Core.Controller.Cursor.SetPosition(Unit.Position);//Focus camera on the unit.
        Core.Controller.SelectedUnit = Unit;
        Continue();
    }
    public void Continue()
    {

        if (_move)
        {
            //move action   
            if (moveTarget != null)
            {
                Debug.Log(Unit + " starts to move!");

                Core.Controller.SelectedTile = Core.Model.GetTile(moveTarget.Position);
                Unit.MoveTo(moveTarget.Position);
            }
            else
            {
                Debug.Log(Unit + " has no move target!");
                _move = false;
                Continue();
            }
        }
        else if (_act)
        {
            Debug.Log(Unit + " starts to act!");
            Unit.Wait();
            _act = false;
            Continue();
        }
        else Exit();
    }
    public void Exit()
    {        
        Core.Controller.Deselect();
        OnAllOrdersFinished();
    }


    #region Move to position
    public void MoveFinished()
    {
        Debug.Log(Unit + " finished movement!");
        _move = false;
        Continue();
    }
    #endregion
    #region Act
    public void ActFinished()
    {
        Debug.Log(Unit + " finished acting!");
        Continue();
    }
    #endregion
    #region Move to enmey HQ
    
    
    #endregion
    #region Act
    public void Attack()
    {

    }
    #endregion
    #region Protect a Unit

    #endregion
    #region Stay out of range (of visible units)

    #endregion
    
}
