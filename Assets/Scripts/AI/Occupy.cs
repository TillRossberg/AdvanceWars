using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupy : Order
{
    public override AI_Unit aiUnit { get ; set ; }
    public override Tile MoveTarget { get ; set ; }
    public override bool OrderFinished { get ; set ; }

    public Occupy(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        MoveTarget = tile;
        MoveTarget.Property.OnAnimationFinished += Exit;
    }
    public override void Start()
    {
        if(aiUnit.Unit.IsAt(MoveTarget))
        {
            Continue();

            Debug.Log(aiUnit.Unit + " captures property.");
        }
        else
        {
            aiUnit.Unit.Wait();
            Exit();
        }
    }
    public override void Continue()
    {
        Core.Controller.OccupyAction(aiUnit.Unit, MoveTarget);
    }
    public override void Exit()
    {
        if(MoveTarget.Property.OwningTeam == aiUnit.Unit.team)
        {
            OrderFinished = true;
        }
        aiUnit.ExecuteNextOrder();
    }
    public override void Terminate()
    {
        MoveTarget.Property.OnAnimationFinished -= Exit;
    }
    #region not in use
    public override Unit AttackTarget { get ; set ; }

    #endregion
}
