//created by Till Roßberg, 2017-18
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region References
    public Data_Tile data;
    public GameObject Gfx;
    public GameObject fogOfWarGfx;
    public Property Property;
    #endregion
    #region General Fields
    public Vector2Int Position;
    public int Rotation;
    public Unit UnitHere = null;
    public List<Tile> Neighbors;
    #endregion
    #region States
    public bool IsVisible = true;

    #endregion
    #region A*
    public float F = 0;
    public float G = 0;
    public float H = 0;
    public Tile PreviousTile;
    #endregion
    #region Base Methods
    public void Init()
    {
        if (this.GetComponent<Property>())
        {
            Property = GetComponent<Property>();
            Property.Init(data.maxTakeOverPoints);
        }
    }
  

    public void SetNeighbors()
    {
        Vector2Int testPos;
        testPos = new Vector2Int(Position.x - 1, Position.y);
        if (Core.Model.IsOnMap(testPos)) Neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(Position.x + 1, Position.y);
        if (Core.Model.IsOnMap(testPos)) Neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(Position.x, Position.y + 1);
        if (Core.Model.IsOnMap(testPos)) Neighbors.Add(Core.Model.GetTile(testPos));
        testPos = new Vector2Int(Position.x, Position.y - 1);
        if (Core.Model.IsOnMap(testPos)) Neighbors.Add(Core.Model.GetTile(testPos));
    }
    #endregion
    //If the tile is a property, set its color to the occuping team color
    public void SetMaterial(Material material)
    {
        Material[] tempMats = Gfx.GetComponent<MeshRenderer>().materials;
        tempMats[0] = material;
        Gfx.GetComponent<MeshRenderer>().materials = tempMats;
    }
    public void SetMaterial(int index)
    {
        if (index < data.materials.Count && data.materials[index] != null) SetMaterial(data.materials[index]);
        else Debug.Log("Material not found!");
    }
    public void SetColor(Color color)
    {
        Material[] tempMats = Gfx.GetComponent<MeshRenderer>().materials;
        tempMats[0].color = color;
        Gfx.GetComponent<MeshRenderer>().materials = tempMats;
    }



    #region Occupation



    //Check if a unit can occupy this tile
    //TODO: Check if the owning team is part of our alliance. (much later)
    public bool CanBeOccupiedBy(Unit unit)
    {
        if (IsProperty() && (Property.OwningTeam != unit.team)
            && (unit.data.type == UnitType.Infantry || unit.data.type == UnitType.Mech)) return true;
        else return false;
    }

    #endregion
    #region Conditions
    public bool CanProduceUnits()
    {
        switch (data.type)
        {
            case TileType.Facility: return true;
            case TileType.Airport: return true;
            case TileType.Port: return true;
            default: return false;
        }
    }
    public bool CanProduce(UnitType type)
    {
        if (!IsProperty()) return false;
        if (data.type == TileType.Facility)
        {
            switch (type)
            {
                case UnitType.AntiAir: return true;
                case UnitType.APC: return true;
                case UnitType.Tank: return true;
                case UnitType.Artillery: return true;
                case UnitType.Rockets: return true;
                case UnitType.Missiles: return true;
                case UnitType.Titantank: return true;
                case UnitType.Recon: return true;
                case UnitType.Infantry: return true;
                case UnitType.MdTank: return true;
                case UnitType.Mech: return true;
                default: return false;
            }
        }
        else if (data.type == TileType.Airport)
        {
            switch (type)
            {
                case UnitType.TCopter: return true;
                case UnitType.BCopter: return true;
                case UnitType.Bomber: return true;
                case UnitType.Fighter: return true;
                default: return false;
            }
        }
        else if (data.type == TileType.Port)
        {
            switch (type)
            {
                case UnitType.Lander: return true;
                case UnitType.Battleship: return true;
                case UnitType.Cruiser: return true;
                case UnitType.Sub: return true;
                default: return false;
            }
        }
        else return false;
    }
    public bool IsProperty()
    {
        if (Property != null) return true;
        else return false;
    }
    public bool IsAllyHere(Unit unit)
    {
        if (UnitHere != null && !unit.IsMyEnemy(UnitHere)) return true;
        else return false;
    }
    public bool IsEnemyHere(Unit unit)
    {
        if (UnitHere != null && unit.IsMyEnemy(UnitHere)) return true;
        else return false;
    }
    #endregion
    #region A*
    public void ResetAStar()
    {
        PreviousTile = null;
        F = 0;
        G = 0;
        H = 0;
    }

    #endregion
}
