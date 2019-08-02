using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    #region References
    Team team;
    public List<AI_Unit> aiUnits;
    public Tile enemyHQ;
    #endregion
    #region Fields
    event Action OnAllUnitsMoved;
    bool decisionPhase = true;
    public bool unitPhase = true;
    bool buyPhase = true;
    #endregion

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
            newUnit.Unit.AnimationController.OnRotationComplete += newUnit.RotateFinished;
        }
        return tempList;
    }
    public void RemoveAllAIUnits(List<Unit> units)
    {
        foreach (Unit item in units) RemoveAIUnit(item);
    }
    public void RemoveAIUnit(Unit unit)
    {
        AI_Unit unitToRemove = GetUnit(unit);
        unitToRemove.OnAllOrdersFinished -= ActivateNextUnit;
        unitToRemove.Unit.AnimationController.OnReachedLastWayPoint -= unitToRemove.MoveFinished;
        unitToRemove.Unit.AnimationController.OnRotationComplete -= unitToRemove.RotateFinished;
        aiUnits.Remove(unitToRemove);
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
    void ResetUnits()        
    {
        foreach (AI_Unit item in aiUnits)item.Reset();       
    }
    #endregion
    #region Main Methods
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
        //Debug.Log("AI continues turn!");
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
        ResetUnits();
        Core.Controller.EndTurnButton();
    }
    #endregion
    #region Decision Methods
    void Decide()
    {
        Debug.Log("-----------------------");
        Debug.Log("AI decides!");
        foreach (AI_Unit aiUnit in aiUnits)
        {
            //Do I protect someone?
            //Can I make a valuable attack?
            if(aiUnit.MoveTarget == null)
            {
                List<ValueTarget> valueTargets = GetValueTargets(aiUnit.Unit, aiUnit.GetAttackableEnemies());

                //Debug.Log("value targets count: " + valueTargets.Count);
                if (valueTargets.Count > 0)
                {
                    aiUnit.AttackTarget = GetMostValuableTarget(valueTargets);
                    Debug.Log(aiUnit.Unit + " wants to attack: " + aiUnit.AttackTarget);
                    aiUnit.MoveTarget = GetAttackTile(aiUnit, aiUnit.AttackTarget);
                }
            }
            if (aiUnit.MoveTarget == null && aiUnit.Unit.IsInfantry())
            {
                if (aiUnit.OccupyTarget == null)
                {
                    Tile targetTile = GetClosestFreeProperty(aiUnit.Unit);
                    if (targetTile != null)
                    {
                        aiUnit.OccupyTarget = targetTile;
                        aiUnit.MoveTarget = GetClosestTileOnPathToTarget(aiUnit.Unit, targetTile);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    if (!aiUnit.Unit.IsAt(aiUnit.OccupyTarget)) aiUnit.MoveTarget = aiUnit.OccupyTarget;
                }

            }
            //Keep on moving to enemy HQ.
            if(aiUnit.MoveTarget == null && aiUnit.Unit.Position != enemyHQ.Position)
            {
                aiUnit.MoveTarget = GetClosestTileOnPathToTarget(aiUnit.Unit, enemyHQ);
            }
        }
        decisionPhase = false;
        ContinueTurn();
    }
    #region Find Attack Target Methods
    Tile GetAttackTile(AI_Unit aiUnit, Unit target)
    {
        List<Tile> path = Core.Model.AStar.GetPath(aiUnit.Unit, aiUnit.Unit.CurrentTile, target.CurrentTile, false);
        return path[path.Count - 2];
    }
    List<ValueTarget> GetValueTargets(Unit attacker, List<Unit> units)
    {
        List<ValueTarget> valueTargets = new List<ValueTarget>();
        foreach (Unit defender in units)
        {
            //Debug.Log("defender: " + defender);
            int attackerDamage = Core.Model.BattleCalculations.CalcDamage(attacker, defender, defender.CurrentTile);
            //Debug.Log("attacker damage: " + attackerDamage);
            int defenderDamage = 0;
            if(defender.data.directAttack) defenderDamage = Core.Model.BattleCalculations.CalcDamage(defender, defender.health - attackerDamage, attacker, defender.CurrentTile);
            //Debug.Log("defender damage: " + defenderDamage);
            float attackerValue = GetDamageValue(attackerDamage, defender);

            //Debug.Log("attacker value = " + attackerValue);
            float defenderValue = GetDamageValue(defenderDamage, attacker);

            //Debug.Log("defender value = " + defenderValue);
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
        return damage / 100f * defender.data.cost;
    }
    Unit GetMostValuableTarget(List<ValueTarget> valueTargets)
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
    #endregion

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
    #region Path Finding Methods
    Tile GetClosestTileOnPathToTarget(Unit unit, Tile targetTile)
    {
        List<Tile> path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, true);
        //If all paths to the enemy HQ are blocked by enemies, ignore them.
        if (path.Count == 0) path = Core.Model.AStar.GetPath(unit, unit.CurrentTile, targetTile, false);
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
    #region Occupy Methods
    Tile GetClosestFreeProperty(Unit unit)
    {
        List<Tile> allProperties = Core.Model.GetProperties();
        List<Tile> freeProperties = new List<Tile>();
        foreach (Tile item in allProperties)if (item.Property.OwningTeam == null) freeProperties.Add(item);
        return GetClosestTile(unit.transform.position, freeProperties);
    }
    Tile GetClosestEnemyProperty(Unit unit)
    {
        List<Tile> allProperties = Core.Model.GetProperties();
        List<Tile> enemyProperties = new List<Tile>();
        foreach (Tile item in allProperties) if (unit.enemyTeams.Contains(item.Property.OwningTeam)) enemyProperties.Add(item);
        return GetClosestTile(unit.transform.position, enemyProperties);
    }
    Tile GetClosestTile(Vector3 position, List<Tile> tiles)
    {
        float shortestDistance = 99999;
        Tile closestTile = null;
        foreach (Tile item in tiles)
        {
            float distance = Vector3.Distance(position, item.transform.position);
            if (distance < shortestDistance)
            {
                closestTile = item;
                shortestDistance = distance;
            }
        }
        return closestTile;
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
}
