using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Order
{
    public override AI_Unit aiUnit { get; set; }
    public override Unit AttackTarget { get; set; }
    public override Tile MoveTarget { get ; set ; }
    public override bool OrderFinished { get ; set; }
    public Attack(AI_Unit aiUnit, Unit unit)
    {
        this.aiUnit = aiUnit;
        AttackTarget = unit;
        aiUnit.Unit.AnimationController.OnAttackAnimationComplete += Exit;
        aiUnit.Unit.AnimationController.OnReachedLastWayPoint += Continue;
    }

    public override void Start()
    {        
        if (AttackTarget == null)
        {
            OrderFinished = true;
            Debug.Log(aiUnit.Unit + " attack target is null.");
            Exit();
        }
        //Can we attack before moving?...
        else if(aiUnit.Unit.CanAttack(AttackTarget) && aiUnit.Unit.CanFire)AttackEm(AttackTarget);   
        //...if not we move and...
        else
        {
            MoveTarget = aiUnit.GetClosestFreeTileAround(AttackTarget.CurrentTile, aiUnit.Unit.CurrentTile);
            MoveTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, MoveTarget);
            //Avoid moving to a tile on wich a friendly unit stands
            if (MoveTarget.IsAllyHere(aiUnit.Unit))
            {
                Debug.Log(aiUnit.Unit + " :ally detected!");
                Tile newTarget = aiUnit.GetClosestFreeTileAround(MoveTarget, aiUnit.Unit.CurrentTile);
                if (newTarget != null) MoveTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, newTarget);
                else MoveTarget = aiUnit.Unit.CurrentTile;
            }
            Core.Controller.SelectedTile = Core.Model.GetTile(MoveTarget.Position);
            aiUnit.Unit.MoveTo(MoveTarget.Position);
            Debug.Log(aiUnit.Unit + " moves to: " + MoveTarget + " and wants to attack");
        }            
    }
    public override void Continue()
    {
        aiUnit.Unit.ConfirmPosition(MoveTarget.Position);
        //...try again after moving.
        if (aiUnit.Unit.CanAttack(AttackTarget) && aiUnit.Unit.CanFire)AttackEm(AttackTarget);      
        else Exit();
    }

    public override void Exit()
    {
        if(AttackTarget != null && AttackTarget.health <= 0)
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
