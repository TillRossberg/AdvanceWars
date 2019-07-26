using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    Team team;
    public Tile enemyHQ;
    event Action AllUnitsMoved;
    bool decisionPhase = false;
    public bool unitPhase = true;
    bool buyPhase = false;

    #region Basic Methods
    public void Init(Team team)
    {
        this.team = team;
        AllUnitsMoved += ExecuteTurn;
        SubscribeForUnitEvents();
        enemyHQ = FindEnemyHQ(team);
    }
    void OnDestroy()
    {
        AllUnitsMoved -= ExecuteTurn;
        UnSubscribeForUnitEvents();
    }
    void SubscribeForUnitEvents()
    {
        foreach (Unit unit in team.Units)
        {
            unit.AnimationController.OnReachedLastWayPoint += MoveFinished;
        }
    }
    void UnSubscribeForUnitEvents()
    {
        foreach (Unit unit in team.Units)
        {
            unit.AnimationController.OnReachedLastWayPoint -= MoveFinished;
        }
    }
    #endregion
    
    public IEnumerator StartTurnDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTurn();
    }
    public void StartTurn()
    {
        Debug.Log("========================");
        Debug.Log("AI starts turn!");
        //Analyze situation
        unitPhase = true;
        ExecuteTurn();
    }
    public void ExecuteTurn()
    {
        Debug.Log("AI executes turn!");
        //Decide what to do
        if (decisionPhase) Decide();
        //Make moves for units
        else if (unitPhase) ActivateNextUnit();
        //Buy new units
        else if (buyPhase) BuyUnits();
        //Nothing more to do?
        else EndTurn();
    }
    public void EndTurn()
    {
        Debug.Log("AI ends turn!");
        Core.Controller.EndTurnButton();
    }
    #region Decision Methods
    void Decide()       
    {
        Debug.Log("AI decides!");
    }
    #endregion
    #region Unit Methods
    void ActivateNextUnit()
    {
        Core.Controller.SelectedUnit = GetNextUnusedUnit(team.Units);
        if (Core.Controller.SelectedUnit != null)
        {
            MoveToHQ(Core.Controller.SelectedUnit);
        }
        else
        {
            unitPhase = false;
            AllUnitsMoved();
        }
    }
    void MoveToHQ(Unit unit)        
    {
        Debug.Log("--------------------");
        Debug.Log(unit + " moves to enemy HQ.");
        //find path to hq
        Core.Model.AStar.CalcPath(unit.data.moveType, unit.CurrentTile, enemyHQ, false);
        //if path is blocked by enemy or ally OR movement cost to the HQ are not enough, find the closest reachable tile
        Tile tile = FindReachableTile(Core.Model.AStar.FinalPath, unit);
        Debug.Log("path length: " + Core.Model.AStar.FinalPath.Count);
        Debug.Log("target tile: " + tile);

        Vector2Int targetPos = tile.Position;
        //focus on unit
        Core.Controller.Cursor.SetPosition(unit.Position);
        //move action        
        Core.Controller.SelectedTile = Core.Model.GetTile(targetPos);
        unit.MoveTo(targetPos);
    }
    void MoveFinished()
    {
        Debug.Log("Movement finished!");
        Action(Core.Controller.SelectedUnit);
    }       
    void Action(Unit unit)        
    {
        Debug.Log(unit + " acts.");
        unit.Wait();
        ActionFinished();
    }
    void ActionFinished()
    {
        Core.Controller.SelectedUnit = null;
        Core.Controller.SelectedTile = null;
        ActivateNextUnit();
    }
    
    Unit GetNextUnusedUnit(List<Unit> units)
    {
        foreach (Unit unit in units) if (unit.HasTurn) return unit;        
        return null;
    }
    #endregion
    #region Buy Methods
    void BuyUnits()        
    {
        
    }
    #endregion
    #region Stuff
    Tile FindEnemyHQ(Team ownTeam)
    {
        foreach (Tile tile in ownTeam.EnemyTeams[0].ownedProperties) if (tile.data.type == TileType.HQ) return tile;        
        throw new System.Exception(ownTeam + " :No enemy HQ found!");
    }
    Tile FindReachableTile(List<Tile> path, Unit unit)
    {
        int movementPoints = unit.data.moveDist;
        for (int i = 1; i < path.Count; i++)
        {
            movementPoints -= path[i].data.GetMovementCost(unit.data.moveType);
            if (movementPoints <= 0 || i == path.Count - 1) return path[i];
            if (unit.IsMyEnemy(path[i].UnitHere)) return path[i - 1];
        }       
        throw new System.Exception("Error in finding reachable tile on path!");
    }
    #endregion

}
