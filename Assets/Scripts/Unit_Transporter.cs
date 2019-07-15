using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Transporter : MonoBehaviour
{
    public GameObject loadUnitArrow;
    public Unit loadedUnit;

    public bool canLoadGroundUnits;
    public bool canLoadCopterUnits;
    public bool canLoadInfantryUnits;

    public void LoadUnit(Unit unit)
    {
        loadedUnit = unit;
        unit.gameObject.SetActive(false);
        unit.GetLoaded();
    }
    public void UnloadUnit(Tile tile)
    {
        loadedUnit.gameObject.SetActive(true);
        loadedUnit.GetUnloaded(tile);
        loadedUnit = null;
    }
    
    public List<Tile> GetPossibleDropOffPositions(Vector2Int pos)
    {
        List<Tile> tempList = new List<Tile>();
        foreach (Tile neighbor in Core.Model.GetTile(pos).Neighbors)
        {
            if (neighbor.data.GetMovementCost(loadedUnit.data.moveType) > 0 && neighbor.UnitHere == null) tempList.Add(neighbor);
        }
        return tempList;
    }


    public void DisplayArrow(bool value)
    {
        loadUnitArrow.SetActive(value);
    }

    public bool CanDropUnitsHere(Vector2Int pos)
    {
        List<Tile> tempList = GetPossibleDropOffPositions(pos);
        if (tempList.Count > 0) return true;       
        else return false;
    }
}
