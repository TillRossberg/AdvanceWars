using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPosition : Order
{
    public override AI_Unit aiUnit { get; set ; }
    public override Unit TargetUnit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override Tile TargetTile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override bool OrderFinished { get ; set ; }

    public DefendPosition(AI_Unit aiUnit)
    {
        this.aiUnit = aiUnit;
    }

    public override void Start()
    {
        Unit highValueTarget = aiUnit.Squad.GetHighValueTarget(aiUnit);
        if(highValueTarget != null)
        {
            if (aiUnit.Unit.data.directAttack) aiUnit.AddOrder(new Move(aiUnit, highValueTarget));
            aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
            Exit();
        }
        else
        {
            aiUnit.AddOrder(new Wait(aiUnit));
            Exit();
        }
    }
    public override void Continue()
    {
        
    }

    public override void Exit()
    {
        aiUnit.ExecuteNextOrder();
    }


    public override void Terminate()
    {
        
    }
}
