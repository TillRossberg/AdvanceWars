using UnityEngine;

public class Wait : Order
{
    public override AI_Unit aiUnit { get; set ; }
    public override Unit AttackTarget { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override Tile MoveTarget { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override bool OrderFinished { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Wait(AI_Unit aiUnit)
    {
        this.aiUnit = aiUnit;
    }

    public override void Start()
    {
        Debug.Log(aiUnit.Unit + " waits.");
        aiUnit.Unit.Wait();
        Exit();
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

    }
}
