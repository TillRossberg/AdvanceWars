using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Squad
{
    AI AI;

    public List<AI_Unit> Units;
    public AI_Squad TargetSquad;
    public Tile TargetTile;
    public POI POI;
    public AI_UnitPreset UnitPreset;
    public int Priority = 0;
    public enum Tactic { Advance, Rally, OccupyPOI, HoldPosition, DefendPosition, WaitForSupply, Heal, Flee}
    public Tactic CurrentTactic;

    public AI_Squad (AI ai, int priority, AI_UnitPreset preset, Tactic tactic)
    {
        this.AI = ai;
        Priority = priority;
        UnitPreset = preset;
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


}
