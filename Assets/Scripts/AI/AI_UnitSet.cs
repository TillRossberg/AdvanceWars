using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_UnitSet
{
    public AI_UnitPreset Preset;
    List<Unit> setUnits;
    public AI_UnitSet(AI_UnitPreset preset)
    {
        this.Preset = preset;
        setUnits = new List<Unit>(new Unit[Preset.Types.Count]);
    }

    public void Add(Unit unit)
    {
        Debug.Log("trying to add " + unit);
        for (int i = 0; i < Preset.Types.Count; i++)
        {
            if (unit.data.type == Preset.Types[i] && setUnits[i] == null)
            {
                setUnits[i] = unit;
                Debug.Log("Adding " + unit + " to set @ " + i);
                return;
            }
        }
        throw new System.Exception(unit + " couldn't be added!");
    }
    public void Remove(Unit unit)
    {
        int unitIndex = setUnits.IndexOf(unit);
        setUnits[unitIndex] = null;
    }
    public UnitType GetNextInPreset()
    {
        for (int i = 0; i < Preset.Types.Count; i++)
        {
            if (setUnits[i] == null) return Preset.Types[i];
        }
        return UnitType.Null;
    }
    public bool Contains(Unit unit)
    {
        return setUnits.Contains(unit);
    }
    public void LogUnits()
    {
        Debug.Log("--------------------");
        Debug.Log("unit count: " + setUnits.Count);
        foreach (var item in setUnits)
        {
            Debug.Log(item);
        }
    }

}
