﻿//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_BuyUnits : MonoBehaviour
{
    //Details window
    public Image thumbNail;
    public TextMeshProUGUI movePoints;
    public TextMeshProUGUI visionPoints;
    public TextMeshProUGUI fuelPoints;
    public TextMeshProUGUI range;

    public TextMeshProUGUI primaryWeaponName;
    public TextMeshProUGUI primaryWeaponAmount;
    public TextMeshProUGUI secondaryWeaponName;
    public TextMeshProUGUI secondaryWeaponAmount;
    //Unit selection
    List<Menu_BuyUnits_Selection> _selectors = new List<Menu_BuyUnits_Selection>();
    public Transform selectorParent;
    public Menu_BuyUnits_Selection unitSelectorPrefab;   
    //Fields
    List<UnitType> _availableUnits = new List<UnitType>();
    Vector2Int _productionPosition;
    
    public void DisplayMenu(Tile tile)
    {
        _productionPosition = tile.position;
        SetAvailableUnits(tile);
        CreateUnitSelectors(_availableUnits);
        Core.View.HighlightFirstMenuItem(selectorParent);
    }

    public void Buy(UnitType type)
    {
        //TODO: make unit face enemy hq
        Core.Model.CreateUnit(type, Core.Controller.ActiveTeam, _productionPosition, Direction.North);
        Core.Controller.CurrentMode = Controller.Mode.normal;
        Core.View.DisplayBuyMenu(false);
    }

   
    public void UpdateDetails(UnitType type)
    {
        Data_Unit data = Core.Model.Database.GetUnitPrefab(type).GetComponent<Unit>().data;

        thumbNail.sprite = data.GetThumbNail(Core.Controller.ActiveTeam);
        movePoints.text = data.moveDist.ToString();
        visionPoints.text = data.visionRange.ToString();
        fuelPoints.text = data.maxFuel.ToString() + "/ \n" + data.maxFuel.ToString();

        if (data.maxRange == 1) range.text = "Range: " + data.maxRange.ToString();
        else range.text = "Range: " + data.minRange.ToString() + "-" + data.maxRange.ToString();

        primaryWeaponName.text = data.primaryWeapon.ToString();
        secondaryWeaponName.text = data.secondaryWeapon.ToString();

        if (data.primaryWeapon == Weapons.none) primaryWeaponAmount.text = "";
        else if(data.primaryAmmo == -1) primaryWeaponAmount.text = "-";
        else primaryWeaponAmount.text = data.primaryAmmo.ToString();

        if (data.secondaryWeapon == Weapons.none) secondaryWeaponAmount.text = "";
        else if (data.secondaryAmmo == -1) secondaryWeaponAmount.text = "-";
        else secondaryWeaponAmount.text = data.secondaryAmmo.ToString();      
    }    
    void SetAvailableUnits(Tile tile)
    {
        Team team = tile.owningTeam;
        if (tile.data.type == TileType.Facility) SetAvailableUnits(team.data.availableGroundUnits);
        if (tile.data.type == TileType.Airport) SetAvailableUnits(team.data.availableAirUnits);
        if (tile.data.type == TileType.Port) SetAvailableUnits(team.data.availableNavalUnits);
    }
    void SetAvailableUnits(List<UnitType> unitTypes)
    {
        _availableUnits.Clear();
        _availableUnits = new List<UnitType>(unitTypes);
    }
    void CreateUnitSelectors(List<UnitType> units)
    {
        if (_selectors.Count > 0) ClearSelectors();
        selectorParent.GetComponent<RectTransform>().sizeDelta = new Vector2(unitSelectorPrefab.GetComponent<RectTransform>().sizeDelta.x, (units.Count + 1) * unitSelectorPrefab.GetComponent<RectTransform>().sizeDelta.y);
        foreach (UnitType type in units)
        {
            Menu_BuyUnits_Selection selector = Instantiate(unitSelectorPrefab, selectorParent);
            selector.Init(Core.Model.Database.GetUnitPrefab(type).GetComponent<Unit>().data);
            _selectors.Add(selector);
        }
    }
    void ClearSelectors()
    {
        foreach (Menu_BuyUnits_Selection selector in _selectors) Destroy(selector.gameObject);       
        _selectors.Clear();
    }
}
