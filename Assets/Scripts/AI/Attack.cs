using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Order
{
    public override AI_Unit aiUnit { get; set; }
    public override Unit AttackTarget { get; set; }
    public override bool OrderFinished { get ; set; }

    public Attack(AI_Unit aiUnit, Unit unit)
    {
        this.aiUnit = aiUnit;
        AttackTarget = unit;
        aiUnit.Unit.AnimationController.OnAttackAnimationComplete += Exit;
    }

    public override void Start()
    {
        if(AttackTarget == null)
        {
            OrderFinished = true;
            Debug.Log(aiUnit.Unit + " attack target already destroyed.");
            Exit();
        }
        else if(aiUnit.Unit.CanAttack(AttackTarget) && aiUnit.Unit.CanFire)
        {
            Core.Controller.Cursor.SetPosition(aiUnit.Unit.Position);
            Core.Controller.SelectedUnit = aiUnit.Unit;
            Debug.Log(aiUnit.Unit + " attacks : " + AttackTarget);
            aiUnit.Unit.RotateAndAttack(AttackTarget);
        }
        else Exit();       
    }
    public override void Continue()
    {
        
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
    }
    #region not in use
    public override Tile targetTile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    #endregion
}
