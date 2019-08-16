using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    #region References
    Team team;
    #endregion
    #region Fields
    public List<AI_Unit> AiUnits = new List<AI_Unit>();
    public Tile enemyHQ;
    public List<POI> POIs = new List<POI>();
    public List<AI_UnitPreset> UnitPresets;
    public List<AI_UnitSet> UnitSets = new List<AI_UnitSet>();
    int enemyHQRadius = 4;//In this radius we consider units as near the HQ.
    enum Strategy { FrontalAttack, HoldPOIs, Siege, Guerilla, AttackFromBehind}
    bool decisionPhase = true;
    bool unitPhase = true;
    bool buyPhase = true;
    #endregion
    #region Events

    #endregion

    #region Basic Methods
    public void Init(Team team)
    {
        this.team = team;
        enemyHQ = GetEnemyHQ(team);
        InitUnitSets();
    }   
    void OnDestroy()
    {
    }    
    void ResetPhases()
    {
        decisionPhase = true;
        unitPhase = true;
        buyPhase = true;
    }
    void ResetUnits()        
    {
        foreach (AI_Unit item in AiUnits)item.Reset();       
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
        Debug.Log("AI: " + team + " starts turn!");
        //Analyze situation
        //Change strategy
        ContinueTurn();
    }
    public void ContinueTurn()
    {
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
    #region Decision Phase
    void Decide()
    {
        foreach (AI_Unit aiUnit in AiUnits)
        {
            ApplyTactic(Strategy.FrontalAttack, aiUnit, enemyHQ);
            if (aiUnit.HasNoOrders())
            {
                //Tile unitPos = Core.Model.GetTile(new Vector2Int(5, 3));
                //Tile attackPos = Core.Model.GetTile(new Vector2Int(9, 3));
                //aiUnit.AddOrder(new Move(aiUnit, attackPos));
                //aiUnit.AddOrder(new Attack(aiUnit, unitPos.UnitHere));
            }
            if(aiUnit.Unit.IsInRadius(enemyHQ, enemyHQRadius))
            {
                Debug.Log(aiUnit.Unit + " is in radius of enemy HQ.");
                if(aiUnit.Unit.IsInfantry())
                {
                    if(GetCapturingUnit(enemyHQ) == null)
                    {   
                        //occupy hq
                        aiUnit.ClearOrders();
                        aiUnit.AddOrder(new Move(aiUnit, enemyHQ));
                        aiUnit.AddOrder(new Occupy(aiUnit, enemyHQ));
                    }
                    else
                    {
                        AI_Unit capturingUnit = GetCapturingUnit(enemyHQ);
                        //check for hp of the occuping unit and unite to increase capture speed
                        if(capturingUnit.Unit.data.type == aiUnit.Unit.data.type && capturingUnit.Unit.health < 70)
                        {
                            Debug.Log(aiUnit.Unit + " wants to unite with " + capturingUnit.Unit + " to increase capture speed!");
                        }
                    }
                }
                else
                {
                    //find random position around hq
                    Tile freeTileAroundHQ = aiUnit.GetRandomFreeReachableTile(Core.Model.GetTilesInRadius(enemyHQ, enemyHQRadius), aiUnit.Unit);
                    if(freeTileAroundHQ != null)
                    { 
                        aiUnit.ClearOrders();
                        aiUnit.AddOrder(new Move(aiUnit, freeTileAroundHQ));
                    }
                    else Debug.Log("No free tile around HQ found!");                    
                }
            }
        }
        decisionPhase = false;
        ContinueTurn();
    }

    void ApplyTactic(Strategy tactic, AI_Unit aiUnit, Tile target)
    {
        aiUnit.ClearOrders();
        switch (tactic)
        {
            case Strategy.FrontalAttack:
                List<ValueTarget> valueTargets = GetValueTargets(aiUnit.Unit, aiUnit.GetAttackableEnemies());
                if (valueTargets.Count > 0)
                {
                    Unit highValueTarget = GetMostValuableTarget(valueTargets);
                    List<AI_Unit> attackingUnits = GetAttackingUnits(highValueTarget);
                    if (attackingUnits.Count == 0 || attackingUnits.Count > 0 && CanSurviveAttack(highValueTarget, attackingUnits))
                    {
                        aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
                    }
                    //The enemy wouldnt survive, so just move on to 
                    else aiUnit.AddOrder(new Move(aiUnit, target));
                }
                else
                {
                    aiUnit.AddOrder(new Move(aiUnit, target));
                }
                break;
            case Strategy.HoldPOIs:
                break;
            case Strategy.Siege:
                if (aiUnit.Unit.IsInfantry())
                {

                }
                else throw new System.Exception("Unit is not infantry!");
                break;
            default:
                break;
        }
    }

    #endregion
    #region Unit Phase
    void ActivateNextUnit()
    {
        Core.Controller.Deselect();
        AI_Unit nextUnit = GetNextUnusedUnit(AiUnits);
        if (nextUnit != null)
        {
            Core.Controller.SelectedUnit = nextUnit.Unit;
            Debug.Log("--->" + nextUnit.Unit + " has its turn.");
            nextUnit.ExecuteNextOrder();
        }
        else
        {
            unitPhase = false;
            ContinueTurn();
        }
    }
    #endregion
    #region Buy Phase
    void BuyUnits()
    {
        Debug.Log("-----------------------");
        Debug.Log("AI wants to buy units.");
        //Ground Units
        foreach (AI_UnitSet set in UnitSets)
        {
            List<Tile> productionBuildings = GetFreeProductionBuildings();
            foreach (Tile facility in productionBuildings)
            {
                UnitType newType = set.GetNextAffordableInPreset(team);
                if (newType != UnitType.Null && facility.CanProduce(newType)) 
                {
                    Buy(newType, facility, team, set);
                }
            }
        }                
        buyPhase = false;
        ContinueTurn();
    }
    void Buy(UnitType unitType, Tile tile, Team team, AI_UnitSet set)
    {
        Core.Controller.Cursor.SetPosition(tile.Position);
        Unit newUnit = Core.View.BuyMenu.Buy(unitType, tile.Position, team);
        Debug.Log("AI buys : " + newUnit);
        team.AddUnit(newUnit);
        set.Add(newUnit);
    }
    
    List<Tile> GetFreeProductionBuildings()
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile tile in team.OwnedProperties)
        {
            if (tile.UnitHere == null && (tile.data.type == TileType.Facility || tile.data.type == TileType.Airport || tile.data.type == TileType.Port)) tempList.Add(tile);
        }
        return tempList;
    }

    #endregion
    #region Unit Presets Methods
    void InitUnitSets()
    {
        foreach (AI_UnitPreset item in UnitPresets)
        {
            UnitSets.Add(new AI_UnitSet(item));
        }
    }
    void RemoveFromUnitSets(Unit unit)
    {
        foreach (AI_UnitSet set in UnitSets)if (set.Contains(unit)) set.Remove(unit);      
    }
    #endregion
    #region AI Unit Methods
    public void AddAIUnit(Unit unit)
    {
        AI_Unit newUnit = new AI_Unit(unit, this);
        AiUnits.Add(newUnit);
        newUnit.OnAllOrdersFinished += ActivateNextUnit;
    }    
    public void RemoveAllAIUnits(List<Unit> units)
    {
        foreach (Unit item in units) RemoveAIUnit(item);
    }
    public void RemoveAIUnit(Unit unit)
    {
        AI_Unit unitToRemove = GetUnit(unit);
        unitToRemove.OnAllOrdersFinished -= ActivateNextUnit;
        unitToRemove.ClearOrders();
        AiUnits.Remove(unitToRemove);
        RemoveFromUnitSets(unit);
    }
    #endregion
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
    List<AI_Unit> GetAttackingUnits(Unit unit)
    {
        List<AI_Unit> tempList = new List<AI_Unit>();
        foreach (AI_Unit aiUnit in AiUnits)
        {
            foreach (Order order in aiUnit.Orders)
            {
                if (order.TargetUnit == unit) tempList.Add(aiUnit);
            }
        }
        return tempList;
    }
    bool CanSurviveAttack(Unit defender, List<AI_Unit> attackers)
    {
        int health = defender.health;
        foreach (AI_Unit attacker in attackers)
        {
            health -= Core.Model.BattleCalculations.CalcDamage(attacker.Unit, defender, defender.CurrentTile);
        }
        if (health > 0) return true;
        else return false;
    }
    #endregion    
    #region Occupy Methods
    AI_Unit GetCapturingUnit(Tile tile)
    {
        if (tile.GetComponent<Property>())
        {
            foreach (AI_Unit aiUnit in AiUnits)
            {
                foreach (Order order in aiUnit.Orders)
                {
                    if(order.GetType() == typeof(Occupy))
                    {
                        if (order.TargetTile == tile) return aiUnit;                        
                    }
                }
            }
            return null;
        }
        else throw new System.Exception("Tile is not a property!");
    }
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
        foreach (AI_Unit item in AiUnits)if (item.Unit == unit) return item;       
        throw new System.Exception("AI unit not found!");
    }
    AI_Unit GetNextUnusedUnit(List<AI_Unit> units)
    {
        foreach (AI_Unit aiUnit in units) if (aiUnit.Unit.HasTurn) return aiUnit;        
        return null;
    }
    Tile GetEnemyHQ(Team ownTeam)
    {
        foreach (Tile tile in ownTeam.EnemyTeams[0].OwnedProperties) if (tile.data.type == TileType.HQ) return tile;        
        throw new System.Exception(ownTeam + " :No enemy HQ found!");
    }
    #endregion
}
