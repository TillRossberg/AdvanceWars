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
    bool _rotate = true;
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
        attackTarget = null;
        moveTarget = null;
    }
    #endregion
    #region Main function
    public void Start()
    {
        Core.Controller.Cursor.SetPosition(Unit.Position);//Focus camera on the unit.
        Core.Controller.SelectedUnit = Unit;
        Continue();
    }
    public void Continue()
    {
        if (_move) Move();
        else if (_rotate) Rotate();
        else if (_act) Act();
        else Exit();
    }
    public void Exit()
    {        
        Core.Controller.Deselect();
        OnAllOrdersFinished();
    }
    #endregion


    #region Move to position
    void Move()
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
    public void MoveFinished()
    {
        Debug.Log(Unit + " finished movement!");
        _move = false;
        Continue();
    }
    #endregion
    #region Rotate to target
    void Rotate()
    {
        if (attackTarget != null)
        {
            Unit.RotateAndAttack(attackTarget);
        }
        else
        {
            Debug.Log(Unit + " hast no attack target!");
            _rotate = false;
            Continue();
        }
    }
    public void RotateFinished(Unit unit)
    {
        Debug.Log(Unit + " finished rotation!");
        _rotate = false;
        Continue();
    }
    #endregion
    #region Act
    void Act()
    {
        Debug.Log(Unit + " starts to act!");       
        if(attackTarget == null)
        {
            Unit.Wait();
        }       
        _act = false;
        Continue();
    }

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
    public List<Unit> GetAttackableEnemies()
    {
        List<Unit> attackableEnemies = new List<Unit>();
        List<Tile> attackableTiles = new List<Tile>();
        if (Unit.data.directAttack)
        {
            attackableTiles = Unit.GetAttackableTilesDirectAttack2(Unit.Position);

            foreach (Tile tile in attackableTiles)
            {
                if (Unit.IsVisibleEnemyHere(tile))
                {
                    Unit enemy = tile.UnitHere;
                    if (Unit.data.GetDamageAgainst(enemy.data.type) > 0) attackableEnemies.Add(enemy);
                }
            }
        }
        else if (Unit.data.rangeAttack)
        {
            attackableEnemies = Unit.GetAttackableEnemies(Unit.Position);
        }
        return attackableEnemies;
    }
    #endregion
    #region Protect a Unit

    #endregion
    #region Stay out of range (of visible units)

    #endregion
    
}
