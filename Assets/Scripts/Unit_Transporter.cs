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
    }
    public void UnloadUnit(Tile tile)
    {

        Debug.Log("pos:" + tile.position);
        loadedUnit.gameObject.SetActive(true);
        loadedUnit.position = tile.position;
        loadedUnit.transform.position = new Vector3(tile.position.x, 0, tile.position.y);
        tile.SetUnitHere(loadedUnit);
        loadedUnit.Wait();
        loadedUnit = null;
    }
    public void SetPossibleDropPositions(Tile targetTile)
    {
        dropOffPositions.Clear();
        foreach (Tile neighbor in targetTile.neighbors)
        {
            if (neighbor.data.GetMovementCost(loadedUnit.data.moveType) > 0 && neighbor.GetUnitHere() == null)
            {
                dropOffPositions.Add(neighbor);

                //Debug.Log(neighbor.position);

            }
        }
    }

    public void DisplayArrow(bool value)
    {
        loadUnitArrow.SetActive(value);
    }
}
