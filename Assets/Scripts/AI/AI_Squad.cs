using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Squad
{
    AI AI;

    public AI_UnitPreset Preset;
    public List<AI_Unit> Units;
    public AI_Squad TargetSquad;
    public Tile TargetTile;
    public POI POI;
    public int Priority = 0;
    public enum Tactic { Advance, Rally, OccupyPOI, HoldPosition, DefendPosition, WaitForSupply, Heal, Flee}
    public Tactic CurrentTactic;

    public AI_Squad (AI ai, int priority, AI_UnitPreset preset, Tactic tactic)
    {
        this.AI = ai;
        Priority = priority;
        Preset = preset;
        CurrentTactic = tactic;
    }

    public void Start()
    {

    }
    public void Continue()
    {

    }
    public void End()
    {

    }  
    public void Add(AI_Unit unit)
    {
        for (int i = 0; i < Preset.Types.Count; i++)
        {
            if (unit.Unit.data.type == Preset.Types[i] && Units[i] == null)
            {
                Units[i] = unit;
                return;
            }
        }
        throw new System.Exception(unit + " couldn't be added!");
    }
    public void Remove(AI_Unit unit)
    {
        int unitIndex = Units.IndexOf(unit);
        Units[unitIndex] = null;
    }   
    public UnitType GetNextAffordableInPreset(Team team)
    {
        for (int i = 0; i < Preset.Types.Count; i++)
        {
            if (Units[i] == null && Core.View.BuyMenu.CanAffordUnit(Preset.Types[i], team)) return Preset.Types[i];
        }
        return UnitType.Null;
    }
    public void ResetUnits()
    {
        foreach (AI_Unit item in Units)item.Reset();       
    }
    public AI_Unit GetNextUnusedUnit()
    {
        foreach (AI_Unit aiUnit in Units) if (aiUnit.Unit.HasTurn) return aiUnit;
        return null;
    }
    public AI_Unit GetAIUnit(Unit unit)
    {
        foreach (AI_Unit item in Units) if (item.Unit == unit) return item;
        return null;
    }

}
