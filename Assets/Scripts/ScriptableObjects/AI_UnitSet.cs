using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/AI Unit Set")]
public class AI_UnitSet : ScriptableObject
{
    public List<Unit> Units;
    public List<UnitType> Types;

    private void OnEnable()
    {
        Units = new List<Unit>(new Unit[Types.Count]);
    }
    //go through the list of types, as soon as you find a free spot in the units list for the given unit, insert it
    public void AddUnit(Unit unit)
    {
        for (int i = 0; i < Types.Count; i++)
        {
            if (Types[i] == unit.data.type && Units[i] == null)
            {
                Units[i] = unit;
                return;
            }
        }
    }
    public void Remove(Unit unit)
    {
        Units[Units.IndexOf(unit)] = null;
    }

    public UnitType GetNextType()
    {
        for (int i = 0; i < Types.Count; i++)
        {
            if (Units[i] == null) return Types[i];
        }
        return UnitType.Null;
    }

}
