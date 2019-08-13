using UnityEngine;
public class Move: Order
{
    public override Tile TargetTile { get ; set; }
    public override AI_Unit aiUnit { get ; set ; }
    public override bool OrderFinished { get ; set ; }
    Tile currentTarget;
    #region Basics
    public Move(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        this.TargetTile = tile;
        OrderFinished = false;
        aiUnit.Unit.AnimationController.OnReachedLastWayPoint += Exit;
    }
    public override void Terminate()
    {
        aiUnit.Unit.AnimationController.OnReachedLastWayPoint -= Exit;
    }

    #endregion
    public override void Start()
    {
        Debug.Log("--> Move");
        Core.Controller.Cursor.SetPosition(aiUnit.Unit.Position);
        Core.Controller.SelectedUnit = aiUnit.Unit;
        if (OrderFinished)
        {
            Debug.Log(aiUnit.Unit + " has finished the move order.");
            Exit();
            return;
        }
        currentTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, TargetTile);
        //Avoid moving to a tile on wich a friendly unit stands
        if (currentTarget.IsAllyHere(aiUnit.Unit))
        {
            Debug.Log(aiUnit.Unit + " :ally detected at : " +  currentTarget);
            Tile newTarget = aiUnit.GetClosestFreeTileAround(currentTarget, aiUnit.Unit.CurrentTile);
            if (newTarget != null) currentTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, newTarget);
            else currentTarget = aiUnit.Unit.CurrentTile;

            Debug.Log("Alternativeley moving to: "  + currentTarget);
        }
        Core.Controller.SelectedTile = Core.Model.GetTile(currentTarget.Position);
        Debug.Log(aiUnit.Unit + " moves to: " + currentTarget);       
        aiUnit.Unit.MoveTo(currentTarget.Position);

    }
    public override void Continue()
    {

    }
    public override void Exit()
    {
        aiUnit.Unit.ConfirmPosition(currentTarget.Position);
        if (aiUnit.Unit.IsAt(TargetTile))OrderFinished = true;
        if (aiUnit.IsLastOrder(this)) aiUnit.Unit.Wait();
        aiUnit.ExecuteNextOrder();
    }
    
    


    #region not in use  
    public override Unit TargetUnit { get ; set; }
    #endregion
}
