using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupy : Order
{
    public override AI_Unit aiUnit { get ; set ; }
    public override Tile targetTile { get ; set ; }
    public override bool OrderFinished { get ; set ; }

    public Occupy(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        targetTile = tile;
        targetTile.Property.OnAnimationFinished += Exit;
    }
    public override void Start()
    {
        if(aiUnit.Unit.IsAt(targetTile))
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
        Core.Controller.OccupyAction(aiUnit.Unit, targetTile);
    }
    public override void Exit()
    {
        if(targetTile.Property.OwningTeam == aiUnit.Unit.team)
        {
            OrderFinished = true;
        }
        aiUnit.ExecuteNextOrder();
    }
    public override void Terminate()
    {
        targetTile.Property.OnAnimationFinished -= Exit;
    }
    #region not in use
    public override Unit AttackTarget { get ; set ; }

    #endregion
}
