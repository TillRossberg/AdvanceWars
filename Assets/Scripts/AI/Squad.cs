﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Squad
{
    #region References
    AI ai;
    #endregion
    #region Fields
    public bool Finished { get; private set; }
    public UnitPreset Preset;
    public List<AI_Unit> Units = new List<AI_Unit>();
    public Squad TargetSquad;
    public Tile TargetTile;
    public POI POI;
    public int Priority = 0;
    public enum Tactic { AdvancePosition, Rally, HoldPOI, OccupyNeutralProperties, OccupyEnemyProperties, HoldPosition, DefendPosition, WaitForReinforcements, Heal, Flee}
    public Tactic CurrentTactic;

    #endregion
    #region Constructors
    public Squad (AI ai, int priority, UnitPreset preset, Tactic tactic)
    {
        this.ai = ai;
        Priority = priority;
        Preset = preset;
        CurrentTactic = tactic;
    }
    public Squad(AI ai, UnitPreset preset)
    {
        this.ai = ai;
        Priority = preset.Priority;
        Preset = preset;
        CurrentTactic = preset.StartTactic;
    }

    #endregion
    #region Basic Methods
    public void Init()
    {
        if (Preset != null) Units = new List<AI_Unit>(new AI_Unit[Preset.Types.Count]);      
    }
    public void Reset()
    {
        Finished = false;
        ResetUnits();
    }
    void ResetUnits()
    {
        foreach (AI_Unit item in Units)if(item != null) item.Reset();
    }
    #endregion
   
    #region Main Methods
    public void Start()
    {
        Debug.Log("====>" + this.Preset.Type + " starts turn.");
        foreach (AI_Unit aiUnit in Units)
        {
            if(aiUnit != null)ApplyTactic(CurrentTactic, aiUnit);             
        }
        Continue();
    }
    public void Continue()
    {
        ActivateNextUnit();
    }
    public void Exit()
    {
        Finished = true;
        ai.ContinueTurn();
    }
    #endregion
    #region Unit Methods   
    void ActivateNextUnit()
    {
        Core.Controller.Deselect();
        AI_Unit nextUnit = GetNextUnusedUnit();
        if (nextUnit != null)
        {
            Core.Controller.SelectedUnit = nextUnit.Unit;
            Debug.Log("--->" + nextUnit.Unit + " has its turn.");
            nextUnit.ExecuteNextOrder();
        }
        else Exit();
    }
    #endregion

    #region Tactic Methods
    void ApplyTactic(Tactic tactic, AI_Unit aiUnit)
    {
        aiUnit.ClearOrders();
        switch (tactic)
        {
            case Tactic.AdvancePosition:
                AdvancePosition(aiUnit, TargetTile);
                break;
            case Tactic.Rally:
                break;
            case Tactic.HoldPOI:
                HoldPOI(aiUnit, POI);
                break;
            case Tactic.OccupyNeutralProperties:
                OccupyProperties(aiUnit, Core.Model.GetNeutralPorperties());
                break;
            case Tactic.OccupyEnemyProperties:
                OccupyProperties(aiUnit, aiUnit.Unit.enemyTeams[0].OwnedProperties);
                break;
            case Tactic.HoldPosition:
                break;
            case Tactic.DefendPosition:
                break;
            case Tactic.WaitForReinforcements:
                break;
            case Tactic.Heal:
                break;
            case Tactic.Flee:
                break;
        }
    }

    #endregion
    #region Advance
    void AdvancePosition(AI_Unit aiUnit, Tile target)
    {
        Unit highValueTarget = GetHighValueTarget(aiUnit);
        if (highValueTarget != null)
        {
            aiUnit.AddOrder(new Move(aiUnit, highValueTarget));
            aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
        }
        else
        {
            aiUnit.AddOrder(new Move(aiUnit, target));
        }        
    }
    Tile GetAttackTile(AI_Unit aiUnit, Unit target)
    {
        List<Tile> path = Core.Model.AStar.GetPath(aiUnit.Unit, aiUnit.Unit.CurrentTile, target.CurrentTile, false);
        return path[path.Count - 2];
    }
    public Unit GetHighValueTarget(AI_Unit aiUnit)
    {
        List<ValueTarget> valueTargets = GetValueTargets(aiUnit.Unit, aiUnit.GetAttackableEnemies());
        foreach (ValueTarget item in valueTargets)
        {
            List<AI_Unit> attackingUnits = ai.GetAttackingUnits(item.Unit);
            if (attackingUnits.Count == 0 || (attackingUnits.Count > 0 && CanSurviveAttack(item.Unit, attackingUnits)))
            {
                Debug.Log(aiUnit.Unit + " has " + valueTargets.Count + " valuable targets and wants to attack " + item.Unit);
                return item.Unit;
            }            
        }       
        return null;
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
            if (defender.data.directAttack) defenderDamage = Core.Model.BattleCalculations.CalcDamage(defender, defender.health - attackerDamage, attacker, defender.CurrentTile);
            //Debug.Log("defender damage: " + defenderDamage);
            float attackerValue = GetDamageValue(attackerDamage, defender);

            //Debug.Log("attacker value = " + attackerValue);
            float defenderValue = GetDamageValue(defenderDamage, attacker);

            //Debug.Log("defender value = " + defenderValue);
            if (attackerValue > defenderValue)
            {
                valueTargets.Add(new ValueTarget(defender, attackerValue));
            }
            //TODO: if (attackerDamage > defender.health) valueTargets.Add(new ValueTarget(defender, attackerValue));// Also consider if you would kill a unit, it should be a value target.
        }
        //valueTargets = valueTargets.OrderBy(h>=h.Value).ToList();???????
        valueTargets.Sort((x, y) => y.Value.CompareTo(x.Value));
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
    
    bool CanSurviveAttack(Unit defender, List<AI_Unit> attackers)
    {
        int health = defender.health;
        int damage = 0;
        Debug.Log("defender health = " + health);
        foreach (AI_Unit attacker in attackers)
        {
            if (attacker.Unit.CanFire)
            {
                damage += Core.Model.BattleCalculations.CalcDamage(attacker.Unit, defender, defender.CurrentTile);
                health -= Core.Model.BattleCalculations.CalcDamage(attacker.Unit, defender, defender.CurrentTile);
            }
        }
        Debug.Log("attack damage = " + damage);

        if (health > 0) return true;
        else return false;
    }
    #endregion
    #region Hold POI
    void HoldPOI(AI_Unit aiUnit, POI poi)
    {
        if (aiUnit.Unit.IsInRadius(poi.Center, poi.Radius))
        {
            Debug.Log(aiUnit.Unit + " is in radius of " + poi.Center + "!");
            //find random position around POI
            aiUnit.AddOrder(new DefendPosition(aiUnit));
        }
        else
        {
            Tile freeTileAroundPOI = aiUnit.GetRandomFreeReachableTile(Core.Model.GetTilesInRadius(poi.Center, poi.Radius), aiUnit.Unit);
            if (freeTileAroundPOI != null)
            {
                aiUnit.AddOrder(new Move(aiUnit, freeTileAroundPOI));
            }
            else Debug.Log("No free tile around POI found!");            
        }
    }

    #endregion
    #region Occupy Properties    
    void OccupyProperties(AI_Unit aiUnit, List<Tile> properties)
    {
        if (aiUnit.Unit.IsInfantry())
        {
            //Found a neutral property no one is already occupying...
            List<Tile> sortedProperties = ai.SortTilesByDistance(aiUnit.Unit.CurrentTile, properties);
            foreach (Tile tile in sortedProperties)
            {
                if (GetCapturingUnit(tile) == null)
                {
                    aiUnit.AddOrder(new Move(aiUnit, tile));
                    aiUnit.AddOrder(new Occupy(aiUnit, tile));
                    return;
                }
            }
            //...if not, find a unit that may need help...
            foreach (Tile tile in sortedProperties)
            {
                AI_Unit capturingUnit = GetCapturingUnit(tile);
                if (capturingUnit.Unit.data.type == aiUnit.Unit.data.type && capturingUnit.Unit.health < 67)
                {
                    Debug.Log(aiUnit.Unit + " wants to unite with " + capturingUnit.Unit + " to increase capture speed!");
                    aiUnit.AddOrder(new Wait(aiUnit));
                    return;
                }
            }
            //...if no one needs help, move on to hq
            aiUnit.AddOrder(new Move(aiUnit, ai.enemyHQ));
        }
        else throw new System.Exception("Unit is not an infantry unit!");

    }
    AI_Unit GetCapturingUnit(Tile tile)
    {
        if (tile.GetComponent<Property>())
        {
            foreach (AI_Unit aiUnit in ai.GetAllUnits())
            {
                foreach (Order order in aiUnit.Orders)
                {
                    if (order.GetType() == typeof(Occupy))
                    {
                        if (order.TargetTile == tile) return aiUnit;
                    }
                }
            }
            return null;
        }
        else throw new System.Exception("Tile is not a property!");
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
    #region Rally

    #endregion

    #region Unit Methods   
    public void Add(Unit unit)
    {
        AI_Unit newUnit = new AI_Unit(unit, this);
        if(Preset != null)
        {
            for (int i = 0; i < Preset.Types.Count; i++)
            {
                if (unit.data.type == Preset.Types[i] && Units[i] == null)
                {
                    Units[i] = newUnit;
                    return;
                }
            }
            throw new System.Exception(unit + " couldn't be added!");
        }
        else Units.Add(newUnit);        
    }
    public void Remove(AI_Unit unit)
    {
        unit.ClearOrders();
        if (Preset != null)
        {
            int unitIndex = Units.IndexOf(unit);
            Units[unitIndex] = null;
        }
        else Units.Remove(unit);
    }
    AI_Unit GetNextUnusedUnit()
    {
        foreach (AI_Unit aiUnit in Units)
        {
            if (aiUnit != null && aiUnit.Unit.HasTurn) return aiUnit;
        }
        return null;
    }
    public AI_Unit GetAIUnit(Unit unit)
    {
        foreach (AI_Unit item in Units)
        {
            if (item == null) Debug.Log("Unit is null!");
            if (item.Unit == unit) return item;
        }
        return null;
    }
    #endregion
    #region Getter
    public UnitType GetNextAffordableInPreset(Team team)
    {
        for (int i = 0; i < Preset.Types.Count; i++)
        {
            if (Units[i] == null && Core.View.BuyMenu.CanAffordUnit(Preset.Types[i], team)) return Preset.Types[i];
        }
        return UnitType.Null;
    }

    #endregion
    #region Debug
    void LogUnits(List<AI_Unit> units)
    {
        if(units.Count > 0)
        {
            foreach (AI_Unit unit in units)
            {
                if(unit != null) Debug.Log(unit.Unit);
                else Debug.Log("unit is null!");
            }

        }
    }
    void LogValueTargets(List<ValueTarget> valueTargets)
    {
        foreach (var item in valueTargets)
        {
            Debug.Log("Value: " + item.Value + " Unit: " + item.Unit);
        }
    }
    #endregion
}
