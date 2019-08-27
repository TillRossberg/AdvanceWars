using UnityEngine;
public class Move: Order
{
    public override Unit TargetUnit { get ; set; }
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
    public Move(AI_Unit aiUnit, Unit unit)
    {
        this.aiUnit = aiUnit;
        this.TargetUnit = unit;
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
        
        //Reached target unit?
        if (TargetUnit != null)
        {
            if(aiUnit.Unit.IsMyEnemy(TargetUnit) && aiUnit.Unit.CanAttack(TargetUnit))
            {
                Debug.Log(aiUnit.Unit + " reached target unit!");
                Exit();
                return;
            }
            TargetTile = TargetUnit.CurrentTile;
        }
        //Reached target tile
        else if (aiUnit.Unit.IsAt(TargetTile))
        {
            Debug.Log(aiUnit.Unit + " reached target tile!");
            Exit();
            return;
        }
        else if(TargetTile == null)
        {

            Debug.Log("Move target is null!");
            Exit();
            return;
        }
        //Find a way
        currentTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, TargetTile);
        //Avoid moving to a tile on wich a friendly unit stands
        if (currentTarget.IsAllyHere(aiUnit.Unit))
        {
            Debug.Log(aiUnit.Unit + " :ally detected at : " +  currentTarget);
            Tile newTarget = aiUnit.GetClosestFreeTileAround(currentTarget, aiUnit.Unit.CurrentTile);

            if (newTarget != null)
            {
                Debug.Log("new target: " + newTarget);
                currentTarget = aiUnit.GetClosestTileOnPathToTarget(aiUnit.Unit, newTarget);
                Debug.Log("current target: " + currentTarget);
            }
            else currentTarget = aiUnit.Unit.CurrentTile;
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
        if(currentTarget != null) aiUnit.Unit.ConfirmPosition(currentTarget.Position);
        if (aiUnit.IsLastOrder(this)) aiUnit.Unit.Wait();
        aiUnit.ExecuteNextOrder();
    }  
}
