using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupy : Order
{
    public override AI_Unit aiUnit { get ; set ; }
    public override Tile TargetTile { get ; set ; }
    public override bool OrderFinished { get ; set ; }

    public Occupy(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        TargetTile = tile;
        TargetTile.Property.OnAnimationFinished += Exit;
    }
    public override void Start()
    {
        if(aiUnit.Unit.IsAt(TargetTile))
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
        Core.Controller.OccupyAction(aiUnit.Unit, TargetTile);
    }
    public override void Exit()
    {
        if(TargetTile.Property.OwningTeam == aiUnit.Unit.team)
        {
            OrderFinished = true;
        }
        aiUnit.ExecuteNextOrder();
    }
    public override void Terminate()
    {
        TargetTile.Property.OnAnimationFinished -= Exit;
    }
    #region not in use
    public override Unit TargetUnit { get ; set ; }

    #endregion
}
