using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Order
{
    public override AI_Unit aiUnit { get; set; }
    public override Unit TargetUnit { get; set; }
    public override Tile TargetTile { get ; set ; }
    public override bool OrderFinished { get ; set; }
    public Attack(AI_Unit aiUnit, Unit unit)
    {
        this.aiUnit = aiUnit;
        TargetUnit = unit;
        aiUnit.Unit.AnimationController.OnAttackAnimationComplete += Exit;
        aiUnit.Unit.AnimationController.OnReachedLastWayPoint += Continue;
    }

    public override void Start()
    {
        Debug.Log("--> Attack");
        if (TargetUnit == null)
        {
            OrderFinished = true;
            Debug.Log(aiUnit.Unit + " attack target is null.");
            Exit();
        }
        //Can we attack before moving?...
        else if(aiUnit.Unit.CanAttack(TargetUnit) && aiUnit.Unit.CanFire)AttackEm(TargetUnit);   
        //...if not we move and...
        else
        {
            TargetTile = aiUnit.GetClosestFreeTileAround(TargetUnit.CurrentTile, aiUnit.Unit.CurrentTile);
            TargetTile = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, TargetTile);
            //Avoid moving to a tile on wich a friendly unit stands
            if (TargetTile.IsAllyHere(aiUnit.Unit))
            {
                Debug.Log(aiUnit.Unit + " :ally detected!");
                Tile newTarget = aiUnit.GetClosestFreeTileAround(TargetTile, aiUnit.Unit.CurrentTile);
                if (newTarget != null) TargetTile = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, newTarget);
                else TargetTile = aiUnit.Unit.CurrentTile;
            }
            Core.Controller.SelectedTile = Core.Model.GetTile(TargetTile.Position);
            aiUnit.Unit.MoveTo(TargetTile.Position);
            Debug.Log(aiUnit.Unit + " moves to: " + TargetTile + " and wants to attack");
        }            
    }
    public override void Continue()
    {
        aiUnit.Unit.ConfirmPosition(TargetTile.Position);
        //...try again after moving.
        if (aiUnit.Unit.CanAttack(TargetUnit) && aiUnit.Unit.CanFire)AttackEm(TargetUnit);      
        else Exit();
    }

    public override void Exit()
    {
        if(TargetUnit != null && TargetUnit.health <= 0)
        {
            OrderFinished = true;
            Debug.Log(aiUnit.Unit + " attack target successfully destroyed.");
        }
        aiUnit.Unit.Wait();
        aiUnit.ExecuteNextOrder();
    }
    public override void Terminate()
    {
        aiUnit.Unit.AnimationController.OnAttackAnimationComplete -= Exit;
        aiUnit.Unit.AnimationController.OnReachedLastWayPoint -= Continue;
    }
    void AttackEm(Unit target)
    {
        Core.Controller.Cursor.SetPosition(aiUnit.Unit.Position);
        Core.Controller.SelectedUnit = aiUnit.Unit;
        Debug.Log(aiUnit.Unit + " attacks : " + target);
        aiUnit.Unit.RotateAndAttack(target);
    }
    #region not in use

    #endregion
}
