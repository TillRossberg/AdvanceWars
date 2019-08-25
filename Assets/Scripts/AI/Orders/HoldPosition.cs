using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPosition : Order
{
    public override AI_Unit aiUnit { get; set ; }
    public override Unit TargetUnit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override Tile TargetTile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override bool OrderFinished { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public HoldPosition(AI_Unit aiUnit)
    {
        this.aiUnit = aiUnit;
    }
    public override void Start()
    {
        //can we attack someone?
        Unit highValueTarget = aiUnit.Squad.GetHighValueTarget(aiUnit, false);
        if (highValueTarget != null)
        {
            aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
            Exit();
        }
        else
        {
            aiUnit.AddOrder(new Wait(aiUnit));
            Exit();
        }
        //just stay
    }
    public override void Continue()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        aiUnit.ExecuteNextOrder();
    }


    public override void Terminate()
    {
        throw new System.NotImplementedException();
    }
}
