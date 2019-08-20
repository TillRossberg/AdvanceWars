using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public enum Tactic { AdvancePosition, Rally, OccupyPOI, OccupyFreeProperties, OccupyEnemyProperties, HoldPosition, DefendPosition, WaitForReinforcements, Heal, Flee}
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
        foreach (AI_Unit item in Units) item.Reset();
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
                Advance(aiUnit);
                break;
            case Tactic.Rally:
                break;
            case Tactic.OccupyPOI:
                OccupyPOI(aiUnit, POI.Center, POI.Radius);
                break;
            case Tactic.OccupyFreeProperties:
                break;
            case Tactic.OccupyEnemyProperties:
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
    void Advance(AI_Unit aiUnit)
    {
        List<ValueTarget> valueTargets = GetValueTargets(aiUnit.Unit, aiUnit.GetAttackableEnemies());
        if (valueTargets.Count > 0)
        {
            Unit highValueTarget = GetMostValuableTarget(valueTargets);
            List<AI_Unit> attackingUnits = ai.GetAttackingUnits(highValueTarget);
            if (attackingUnits.Count == 0 || attackingUnits.Count > 0 && CanSurviveAttack(highValueTarget, attackingUnits))
            {
                aiUnit.AddOrder(new Attack(aiUnit, highValueTarget));
            }
            //The enemy wouldnt survive, so just move on to 
            else aiUnit.AddOrder(new Move(aiUnit, TargetTile));
        }
        else
        {
            aiUnit.AddOrder(new Move(aiUnit, TargetTile));
        }
    }
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
    #region Occupy POI
    void OccupyPOI(AI_Unit aiUnit, Tile target, int radius)
    {
        if (aiUnit.Unit.IsInRadius(target, radius))
        {
            Debug.Log(aiUnit.Unit + " is in radius of " + target + "!");
            if (aiUnit.Unit.IsInfantry())
            {
                if (ai.GetCapturingUnit(target) == null)
                {
                    //occupy hq
                    aiUnit.ClearOrders();
                    aiUnit.AddOrder(new Move(aiUnit, target));
                    aiUnit.AddOrder(new Occupy(aiUnit, target));
                }
                else
                {
                    AI_Unit capturingUnit = ai.GetCapturingUnit(target);
                    //check for hp of the occuping unit and unite to increase capture speed
                    if (capturingUnit.Unit.data.type == aiUnit.Unit.data.type && capturingUnit.Unit.health < 70)
                    {
                        Debug.Log(aiUnit.Unit + " wants to unite with " + capturingUnit.Unit + " to increase capture speed!");
                    }
                }
            }
            else
            {
                //find random position around hq
                Tile freeTileAroundHQ = aiUnit.GetRandomFreeReachableTile(Core.Model.GetTilesInRadius(target, radius), aiUnit.Unit);
                if (freeTileAroundHQ != null)
                {
                    aiUnit.ClearOrders();
                    aiUnit.AddOrder(new Move(aiUnit, freeTileAroundHQ));
                }
                else Debug.Log("No free tile around HQ found!");
            }
        }
    }

    #endregion
    #region Occupy
    
    Tile GetClosestFreeProperty(Unit unit)
    {
        List<Tile> allProperties = Core.Model.GetProperties();
        List<Tile> freeProperties = new List<Tile>();
        foreach (Tile item in allProperties) if (item.Property.OwningTeam == null) freeProperties.Add(item);
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
        foreach (AI_Unit item in Units) if (item.Unit == unit) return item;
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
    #endregion
}
