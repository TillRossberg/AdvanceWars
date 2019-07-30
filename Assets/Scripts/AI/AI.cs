using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    Team team;
    public List<AI_Unit> aiUnits;
    public Tile enemyHQ;

    event Action OnAllUnitsMoved;
    bool decisionPhase = true;
    public bool unitPhase = true;
    bool buyPhase = true;

    #region Basic Methods
    public void Init(Team team)
    {
        this.team = team;
        aiUnits = InitAIUnits(team.Units);
        enemyHQ = GetEnemyHQ(team);
        OnAllUnitsMoved += ContinueTurn;
    }
    List<AI_Unit> InitAIUnits(List<Unit> units)
    {
        List<AI_Unit> tempList = new List<AI_Unit>();
        foreach (Unit item in units)
        {
            AI_Unit newUnit = new AI_Unit(item, this);
            tempList.Add(newUnit);
            newUnit.OnAllOrdersFinished += ActivateNextUnit;
            newUnit.Unit.AnimationController.OnReachedLastWayPoint += newUnit.MoveFinished;
        }
        return tempList;
    }

    void OnDestroy()
    {
        OnAllUnitsMoved -= ContinueTurn;        
    }
    
    void ResetPhases()
    {
        decisionPhase = true;
        unitPhase = true;
        buyPhase = true;
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
        ContinueTurn();
    }
    public void ContinueTurn()
    {
        Debug.Log("AI continues turn!");
        //Scout before make decisions?
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
        ResetPhases();
        Core.Controller.EndTurnButton();
    }
    #region Decision Methods
    void Decide()
    {
        Debug.Log("-----------------------");
        Debug.Log("AI decides!");
        foreach (AI_Unit aiUnit in aiUnits)
        {
            //Do I protect someone?
            //Can I make a valuable attack?
            aiUnit.Unit.GetAttackableEnemies(aiUnit.Unit.Position);
            List<ValueTarget> valueTargets = GetValueTargets(aiUnit.Unit, aiUnit.Unit.GetAttackableUnits());

            if (valueTargets.Count > 0) GetMostValuableTarget(valueTargets);

            //Keep on moving to enemy HQ.
            aiUnit.moveTarget = GetClosestTileOnPathToEnemyHQ(aiUnit.Unit, enemyHQ);
        }
        decisionPhase = false;
        ContinueTurn();
    }
    List<ValueTarget> GetValueTargets(Unit attacker, List<Unit> units)
    {
        List<ValueTarget> valueTargets = new List<ValueTarget>();
        foreach (Unit defender in units)
        {
            int attackerDamage = Core.Model.BattleCalculations.CalcDamage(attacker, defender, defender.CurrentTile);
            int defenderDamage = Core.Model.BattleCalculations.CalcDamage(defender, defender.health - attackerDamage, attacker, defender.CurrentTile);
            float attackerValue = GetDamageValue(attackerDamage, defender);
            float defenderValue = GetDamageValue(defenderDamage, attacker);
            if (attackerValue > defenderValue)
            {
                valueTargets.Add(new ValueTarget(defender, attackerValue));
            }
            //if (attackerDamage > defender.health) valueTargets.Add(defender);// Also consider if you would kill a unit, it should be a value target.
        }
        return valueTargets;
    }
    float GetDamageValue(int damage, Unit defender)
    {
        return damage / 100 * defender.data.cost;
    }
    Unit GetMostValuableTarget(List<ValueTarget> valueTargets)
    {
        if (valueTargets.Count > 0)
        {
            ValueTarget mostValuableTarget = new ValueTarget();
            float highestValue = 0;
            foreach (ValueTarget item in valueTargets)
            {
                if (item.Value > highestValue)
                {
                    mostValuableTarget = item;
                    highestValue = item.Value;
                }
            }
            return mostValuableTarget.Unit;
        }
        throw new System.Exception("Value targets list is empty!");
    }

    #endregion
    #region Unit Methods
    void ActivateNextUnit()
    {
        AI_Unit nextUnit = GetNextUnusedUnit(aiUnits);
        if (nextUnit != null)
        {
            nextUnit.Start();
        }
        else
        {
            unitPhase = false;
            OnAllUnitsMoved();
        }
    }    
    #endregion
    #region Buy Methods
    void BuyUnits()        
    {
        Debug.Log("-----------------------");
        Debug.Log("AI wants to buy units!");
        buyPhase = false;
        ContinueTurn();
    }
    #endregion
    #region Stuff
    public void RemoveAllAIUnits(List<Unit> units)
    {
        foreach (Unit item in units) RemoveAIUnit(item);       
    }
    public void RemoveAIUnit(Unit unit)
    {
        AI_Unit unitToRemove = GetUnit(unit);
        unitToRemove.OnAllOrdersFinished -= ActivateNextUnit;
        unitToRemove.Unit.AnimationController.OnReachedLastWayPoint -= unitToRemove.MoveFinished;
        aiUnits.Remove(unitToRemove);
    }
    #endregion
    #region Getter
    AI_Unit GetUnit(Unit unit)
    {
        foreach (AI_Unit item in aiUnits)if (item.Unit == unit) return item;       
        throw new System.Exception("AI unit not found!");
    }
    AI_Unit GetNextUnusedUnit(List<AI_Unit> units)
    {
        foreach (AI_Unit aiUnit in units) if (aiUnit.Unit.HasTurn) return aiUnit;        
        return null;
    }
    Tile GetEnemyHQ(Team ownTeam)
    {
        foreach (Tile tile in ownTeam.EnemyTeams[0].ownedProperties) if (tile.data.type == TileType.HQ) return tile;        
        throw new System.Exception(ownTeam + " :No enemy HQ found!");
    }
    #endregion
    #region Calc way to HQ
    Tile GetClosestTileOnPathToEnemyHQ(Unit unit, Tile enemyHQ)
    {
        //find path to hq
        List<Tile> path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, enemyHQ, true);
        if (path.Count == 0) path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, enemyHQ, false);//If all paths to the enemy HQ are blocked by enemies, ignore them.
        return FindReachableTile(path, unit);
    }
    //Find a tile on the path that can be reached with the remaining movement points AND that is not blocked by an enemy.
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
