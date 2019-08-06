using System.Collections.Generic;
using UnityEngine;

public class Move: Order
{
    public override Tile targetTile { get ; set; }
    public override AI_Unit aiUnit { get ; set ; }
    public override bool OrderFinished { get ; set ; }
    Tile currentPosition;
    #region Basics
    public Move(AI_Unit aiUnit, Tile tile)
    {
        this.aiUnit = aiUnit;
        this.targetTile = tile;
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
        if (OrderFinished)
        {
            Debug.Log(aiUnit.Unit + " has finished the move order.");
            Exit();
            return;
        }
        Core.Controller.Cursor.SetPosition(aiUnit.Unit.Position);
        Core.Controller.SelectedUnit = aiUnit.Unit;

        currentPosition = GetClosestTileOnPathToTarget(aiUnit.Unit, targetTile);
        if (currentPosition.UnitHere != null && !aiUnit.Unit.IsMyEnemy(currentPosition.UnitHere))
        {
            //Avoid moving to a tile on wich a friendly unit stands
        }
        Core.Controller.SelectedTile = Core.Model.GetTile(currentPosition.Position);
        aiUnit.Unit.MoveTo(currentPosition.Position);

        Debug.Log(aiUnit.Unit + " moves to: " + currentPosition);       
    }
    public override void Continue()
    {

    }
    public override void Exit()
    {
        aiUnit.Unit.ConfirmPosition(currentPosition.Position);
        if (aiUnit.Unit.IsAt(targetTile))OrderFinished = true;
        if (aiUnit.IsLastOrder(this)) aiUnit.Unit.Wait();
        aiUnit.ExecuteNextOrder();
    }
    
    Tile GetClosestTileOnPathToTarget(Unit unit, Tile targetTile)
    {
        List<Tile> path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, true);
        //If all paths to the target are blocked by enemies, ignore the enemies. (The enemies on the path will be taken into account later.)
        if (path.Count == 0) path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, false);
        return FindReachableTile(path, unit);
    }
    //Find a tile on the path that can be reached with the remaining movement points AND that is not blocked by an enemy.
    Tile FindReachableTile(List<Tile> path, Unit unit)
    {
        if (path.Count == 1) return path[0];
        int movementPoints = unit.data.moveDist;
        for (int i = 1; i < path.Count; i++)
        {
            movementPoints -= path[i].data.GetMovementCost(unit.data.moveType);
            if (movementPoints <= 0 || i == path.Count - 1) return path[i];
            if (unit.IsMyEnemy(path[i].UnitHere)) return path[i - 1];
        }
        throw new System.Exception("Error in finding reachable tile on path!");
    }
    


    #region not in use  
    public override Unit AttackTarget { get ; set; }
    #endregion
}
