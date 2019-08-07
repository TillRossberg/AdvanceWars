using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Move: Order
{
    public override Tile targetTile { get ; set; }
    public override AI_Unit aiUnit { get ; set ; }
    public override bool OrderFinished { get ; set ; }
    Tile currentTarget;
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

        currentTarget = GetClosestTileOnPathToTarget(aiUnit.Unit, targetTile);
        if (currentTarget.IsAllyHere(aiUnit.Unit))
        {
            //Avoid moving to a tile on wich a friendly unit stands
            Tile newTarget = GetClosestFreeTileAround(currentTarget, aiUnit.Unit.CurrentTile);
            if(newTarget != null)currentTarget = GetClosestTileOnPathToTarget(aiUnit.Unit, newTarget);          
        }
        Core.Controller.SelectedTile = Core.Model.GetTile(currentTarget.Position);
        aiUnit.Unit.MoveTo(currentTarget.Position);

        Debug.Log(aiUnit.Unit + " moves to: " + currentTarget);       
    }
    public override void Continue()
    {

    }
    public override void Exit()
    {
        aiUnit.Unit.ConfirmPosition(currentTarget.Position);
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
    //Since an ally stands on the original target, we need to find a position around that target, where we can move to.
    //BUT this position must not be farther away than the original tile, or it can happen that we cannot move there, because we dont have enough movement points.
    Tile GetClosestFreeTileAround(Tile originalTarget, Tile ownPosition)
    {
        bool finished = false;
        int radius = 1;
        int counter = 0;
        float distance = Vector3.Distance(ownPosition.transform.position, originalTarget.transform.position);
        Tile freeTile = null;
        //repeat until a free tile was found, if none is found and you have searched the whole map return null
        while (!finished)
        {
            //get all tiles in the radius around the original
            List<Tile> tilesInRadius = Core.Model.GetTilesInRadius(originalTarget, radius);
            //reduce this list to tiles hat are not farther away than our original target
            List<Tile> closerTiles = GetTilesThatAreCloser(originalTarget, distance, tilesInRadius);
            //Sort the tiles by distance to the own position
            List<Tile> sortedClosestTiles = SortTilesByDistance(ownPosition, closerTiles);
            //finally try to find a free tile 
            freeTile = GetFreeTile(sortedClosestTiles);
            radius++;
            counter++;
            if (freeTile != null) finished = true;
            if ((radius > (Core.Model.MapMatrix.Count / 2) && radius > (Core.Model.MapMatrix[0].Count) / 2)) finished = true;
            //TODO: test for safe removal
            if (counter > 1000)
            {
                Debug.Log("get closest free tile expired counter");
                finished = true;
            }
        }
        return freeTile;
//)
    }   
    //We reduce the list to tiles that are closer to the pivot than the maximum distance.
    List<Tile> GetTilesThatAreCloser(Tile pivot, float maxDistance, List<Tile> tiles)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile item in tiles)
        {
            if (Vector3.Distance(pivot.transform.position, item.transform.position) < maxDistance) tempList.Add(item);
        }        
        return tempList;
    }

    Tile GetFreeTile(List<Tile> tiles )
    {
        foreach (Tile item in tiles) if (item.UnitHere == null) return item;        
        return null;
    }
    List<Tile> SortTilesByDistance(Tile tile, List<Tile> tiles)
    {
        Dictionary<Tile, float> TileDistance = new Dictionary<Tile, float>();
        foreach (Tile item in tiles)TileDistance.Add(item, Vector3.Distance(tile.transform.position, item.transform.position));       
        var tempList = TileDistance.ToList();
        tempList = TileDistance.OrderBy(h => h.Value).ToList();
        List<Tile> tempList2 = new List<Tile>();
        foreach (var item in tempList) tempList2.Add(item.Key);        
        return tempList2;
    }

    float GetDistance(Tile tile1, Tile tile2)
    {
        return Vector3.Distance(tile1.transform.position, tile2.transform.position);
    }


    #region not in use  
    public override Unit AttackTarget { get ; set; }
    #endregion
}
