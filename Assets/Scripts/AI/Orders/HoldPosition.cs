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

        Debug.Log(aiUnit.Unit + " holds its position.");
        //can we attack someone?
        Unit highValueTarget = aiUnit.Squad.GetHighValueTarget(aiUnit, false);

        Debug.Log("high value target " + highValueTarget);
        if (highValueTarget != null)
        {
            aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
            Exit();
        }
        //just stay
        else
        {
            Debug.Log("No high value target found.");
            aiUnit.AddOrder(new Wait(aiUnit));
            Exit();
        }
    }
    public override void Continue()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        if (aiUnit.IsLastOrder(this)) aiUnit.Unit.Wait();
        aiUnit.ExecuteNextOrder();
    }


    public override void Terminate()
    {

    }
}
