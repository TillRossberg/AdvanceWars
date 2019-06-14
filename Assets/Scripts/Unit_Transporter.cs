using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Transporter : MonoBehaviour
{
    public GameObject loadUnitArrow;
    public Unit loadedUnit;
    public List<Tile> dropOffPositions = new List<Tile>();

    public bool canLoadGroundUnits;
    public bool canLoadCopterUnits;
    public bool canLoadInfantryUnits;

    public void LoadUnit(Unit unit)
    {
        loadedUnit = unit;
        unit.gameObject.SetActive(false);
        Core.Model.GetTile(unit.Position).SetUnitHere(null);
    }
    public void UnloadUnit(Tile tile)
    {
        loadedUnit.gameObject.SetActive(true);
        loadedUnit.SetPosition(tile.Position);
        tile.SetUnitHere(loadedUnit);
        loadedUnit.Wait();
        loadedUnit = null;
    }
    public void SetPossibleDropPositions(Tile targetTile)
    {
        dropOffPositions.Clear();
        foreach (Tile neighbor in targetTile.neighbors)
        {
            if (neighbor.data.GetMovementCost(loadedUnit.data.moveType) > 0 && neighbor.GetUnitHere() == null) dropOffPositions.Add(neighbor);           
        }
    }

    public void DisplayArrow(bool value)
    {
        loadUnitArrow.SetActive(value);
    }

    public bool CanDropUnitsHere(Tile targetTile)
    {
        SetPossibleDropPositions(targetTile);
        if (dropOffPositions.Count > 0)
        {
            dropOffPositions.Clear();
            return true;
        }
        else return false;
    }

    public Tile GetTile(Vector2Int pos)
    {
        Tile tile = null;
        foreach (Tile item in dropOffPositions)
        {
            if (item.Position == pos) tile = item;
        }
        return tile;
    }
}
