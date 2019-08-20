﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class AI_Unit
{
    #region References
    Squad squad;
    public Unit Unit;
    #endregion
    #region Fields
    public List<Order> Orders = new List<Order>();
    int orderIndex = 0;  
    #endregion
    

    #region Basic Methods
    public AI_Unit(Unit unit, Squad squad)
    {
        this.Unit = unit;
        this.squad = squad;
    }
    public void Reset()
    {        
        orderIndex = 0;
    }
    #endregion   
    #region Order Methods
    public void ExecuteNextOrder()
    {
        if (orderIndex < Orders.Count)
        {
            Core.Controller.Cursor.SetPosition(Unit.Position);
            Core.Controller.SelectedUnit = Unit;
            Order nextOrder = GetNextOrder();
            nextOrder.Start();
        }
        else
        {
            if (AllOrdersFinished()) ClearOrders();
            squad.Continue();
        }
    }
    Order GetNextOrder()
    {
        Order order = Orders[orderIndex];
        orderIndex++;
        return order;
    }
    public void AddOrder(Order order)
    {
        Orders.Add(order);
    }
    public void ClearOrders()
    {
        foreach (Order item in Orders)item.Terminate();      
        Orders.Clear();
    }
    public bool IsLastOrder(Order order)
    {
        int index = Orders.IndexOf(order);
        if (index == Orders.Count - 1) return true;
        else return false;
    }
    public bool HasNoOrders()
    {
        if (Orders.Count == 0) return true;
        else return false;
    }
    bool AllOrdersFinished()
    {
        foreach (Order item in Orders)if (!item.OrderFinished) return false;        
        return true;
    }
    #endregion
    #region Pathfinding
    public Tile GetClosestTileOnPathToTarget(Unit unit, Tile targetTile)
    {
        Debug.Log(unit + " wants to move to " + targetTile);
        List<Tile> path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, true);
        //If all paths to the target are blocked by enemies, ignore the enemies. (The enemies on the path will be taken into account later.)

        if (path.Count == 0)
        {
            Debug.Log("path count: " + path.Count + ", trying again!");
            path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, false);
        }
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
            if (unit.IsMyEnemy(path[i].UnitHere)) return path[i - 1];
            if (movementPoints <= 0 || i == path.Count - 1) return path[i];
        }       
        throw new System.Exception("Error in finding reachable tile on path!");
    }
    //Since an ally stands on the original target, we need to find a position around that target, where we can move to.
    //BUT this position must not be farther away than the original tile, or it can happen that we cannot move there, because we dont have enough movement points.
    public Tile GetClosestFreeTileAround(Tile target, Tile ownPosition)
    {
        bool finished = false;
        int radius = 1;
        int counter = 0;
        float maxDistance = Vector3.Distance(ownPosition.transform.position, target.transform.position);
        Tile freeTile = null;
        //repeat until a free tile was found, if none is found and you have searched the whole map return null
        while (!finished)
        {
            //get all tiles in the radius around the original
            List<Tile> tilesInRadius = Core.Model.GetTilesInRadius(target, radius);
            //reduce this list to tiles hat are not farther away than our original target
            List<Tile> closerTiles = GetTilesThatAreCloser(ownPosition, maxDistance, tilesInRadius);
            //Sort the tiles by distance to the own position
            //List<Tile> sortedClosestTiles = SortTilesByDistance(ownPosition, closerTiles);
            //finally try to find a free tile 
            freeTile = GetFreeTile(closerTiles);
            radius++;
            counter++;
            if (freeTile != null) finished = true;
            if ((radius > Core.Model.MapMatrix.Count && radius > Core.Model.MapMatrix[0].Count)) finished = true;
            //TODO: test for safe removal
            if (counter > 1000)
            {
                Debug.Log("Reached closest free tile expire counter!");
                finished = true;
            }
        }

        Debug.Log("free tile: " + freeTile);
        return freeTile;
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
    Tile GetFreeTile(List<Tile> tiles)
    {
        foreach (Tile item in tiles) if (item.UnitHere == null && item.data.GetMovementCost(Unit.data.moveType) > 0) return item;
        return null;
    }
    public Tile GetRandomFreeReachableTile(List<Tile> tiles, Unit unit)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in tiles)if (tile.UnitHere == null && tile.data.GetMovementCost(unit.data.moveType) > 0) tempList.Add(tile);
        if (tempList.Count == 0) return null;       
        else return tempList[UnityEngine.Random.Range(0, tempList.Count)];
    }
    
    List<Tile> SortTilesByDistance(Tile tile, List<Tile> tiles)
    {
        Dictionary<Tile, float> TileDistance = new Dictionary<Tile, float>();
        foreach (Tile item in tiles) TileDistance.Add(item, Vector3.Distance(tile.transform.position, item.transform.position));
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
    #endregion     
    #region Get Attackable Enemies    
    public List<Unit> GetAttackableEnemies()
    {
        List<Unit> attackableEnemies = new List<Unit>();
        List<Tile> attackableTiles = new List<Tile>();
        if (Unit.data.directAttack)
        {
            attackableTiles = Unit.GetAttackableTilesDirectAttack2(Unit.Position);

            foreach (Tile tile in attackableTiles)
            {
                if (Unit.IsVisibleEnemyHere(tile))
                {
                    Unit enemy = tile.UnitHere;
                    if (Unit.data.GetDamageAgainst(enemy.data.type) > 0) attackableEnemies.Add(enemy);
                }
            }
        }
        else if (Unit.data.rangeAttack)
        {
            attackableEnemies = Unit.GetAttackableEnemies(Unit.Position);
        }
        return attackableEnemies;
    }
    #endregion
}
