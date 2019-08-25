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
        else if (aiUnit.Unit.CanFire && aiUnit.Unit.CanAttack(TargetUnit)) AttackTarget(TargetUnit);
        else Exit();        
    }
    public override void Continue()
    {
      
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
    void AttackTarget(Unit target)
    {
        Core.Controller.Cursor.SetPosition(aiUnit.Unit.Position);
        Core.Controller.SelectedUnit = aiUnit.Unit;
        Debug.Log(aiUnit.Unit + " attacks : " + target);
        aiUnit.Unit.RotateAndAttack(target);
    }
    #region not in use

    #endregion
}
