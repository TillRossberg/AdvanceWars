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
    private void OnMouseDown()
    {
        //myLevelManager.GetComponent<AnimController>().boom(xPos, yPos);
        //Actions are only perfomed, if no menu is opened.

        ////Move mode
        //if(_manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
        //{
        //    Unit selectedUnit = _manager.getGameFunctions().getSelectedUnit().GetComponent<Unit>();
        //    if((isPartOfArrowPath && unitStandingHere == null) || (isPartOfArrowPath && !isVisible))
        //    {
        //        //Move to the position and try to find units that can be attacked.
        //        selectedUnit.moveUnitTo(this.xPos, this.yPos);
        //        selectedUnit.FindAttackableTiles();
        //        selectedUnit.FindAttackableEnemies();
        //        //Delete the reachable tiles and the movement arrow.
        //        _manager.getMapCreator().ResetReachableTiles();
        //        _manager.getArrowBuilder().resetAll();                    
        //    }
        //}       


    }

    private void OnMouseEnter()
    {
        ////Actions are only perfomed, if the menu is not opened.
        //if (!_manager.getContextMenu().isOpened && _manager.getGameFunctions().getCurrentMode() == GameFunctions.mode.move)
        //{
        //    //Draws an Arrow on the tile, if it is reachable
        //    if (isReachable && !isPartOfArrowPath )
        //    {
        //        _manager.getArrowBuilder().createArrowPath(this);
        //    }
        //    //If you go back, make the arrow smaller.
        //    if (isPartOfArrowPath)
        //    {
        //        _manager.getArrowBuilder().tryToGoBack(this);

        //        //Resets the arrowPath if you hover over the unit again. (If this is the tile the unit stands on and an arrow has been drawn.)
        //        if (this == _manager.getArrowBuilder().getArrowPath()[0].getTile() && _manager.getArrowBuilder().getArrowPath().Count > 2)
        //        {
        //            _manager.getArrowBuilder().resetArrowPath();
        //        }
        //    }
        //}
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
