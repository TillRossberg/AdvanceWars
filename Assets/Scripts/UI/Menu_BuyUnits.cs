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
    List<Menu_BuyUnits_Selection> selectors = new List<Menu_BuyUnits_Selection>();
    public Transform selectorParent;
    public Menu_BuyUnits_Selection unitSelectorPrefab;
    //Fields
    List<UnitType> availableUnits = new List<UnitType>();
    public Vector2Int ProductionPosition;

    public void Show(Tile tile)
    {
        this.gameObject.SetActive(true);
        Core.Controller.CurrentMode = Controller.Mode.BuyMenu;
        ProductionPosition = tile.Position;
        SetAvailableUnits(tile);
        CreateUnitSelectors(availableUnits);
        Core.View.HighlightFirstMenuItem(selectorParent);
    }  
    public void Hide()        
    {
        this.gameObject.SetActive(false);
        Core.Controller.CurrentMode = Controller.Mode.Normal;
    }

    public Unit Buy(UnitType type, Vector2Int position, Team team)
    {
        team.SubtractMoney(Core.Model.Database.GetUnitCost(type));
        return Core.Model.CreateUnit(type, position, Direction.North);
    }
    public bool CanAffordUnit(UnitType type, Team team)
    {       
        if (Core.Model.Database.GetUnitCost(type) <= team.Money) return true;
        else return false;
    }

    public void UpdateDetails(UnitType type)
    {
        Data_Unit data = Core.Model.Database.GetUnitPrefab(type).GetComponent<Unit>().data;

        thumbNail.sprite = data.GetThumbNail(Core.Controller.ActiveTeam);
        movePoints.text = data.moveDist.ToString();
        visionPoints.text = data.visionRange.ToString();
        fuelPoints.text = data.maxFuel.ToString() + "/ \n" + data.maxFuel.ToString();

        
        if (data.maxRange == 0 || data.maxRange == 1) range.text = "";
        else range.text = "Range: " + data.minRange.ToString() + "-" + data.maxRange.ToString();

        primaryWeaponName.text = data.primaryWeapon.ToString();
        secondaryWeaponName.text = data.secondaryWeapon.ToString();

        if (data.primaryWeapon == Weapons.none) primaryWeaponAmount.text = "";
        else if(data.primaryAmmo == 0) primaryWeaponAmount.text = "-";
        else primaryWeaponAmount.text = data.primaryAmmo.ToString();

        if (data.secondaryWeapon == Weapons.none) secondaryWeaponAmount.text = "";
        else if (data.secondaryAmmo == 0) secondaryWeaponAmount.text = "-";
        else secondaryWeaponAmount.text = data.secondaryAmmo.ToString();      
    }    
    void SetAvailableUnits(Tile tile)
    {
        Team team = tile.Property.OwningTeam;
        if (tile.data.type == TileType.Facility) SetAvailableUnits(team.Data.availableGroundUnits);
        if (tile.data.type == TileType.Airport) SetAvailableUnits(team.Data.availableAirUnits);
        if (tile.data.type == TileType.Port) SetAvailableUnits(team.Data.availableNavalUnits);
    }
    void SetAvailableUnits(List<UnitType> unitTypes)
    {
        availableUnits.Clear();
        availableUnits = new List<UnitType>(unitTypes);
    }
    void CreateUnitSelectors(List<UnitType> units)
    {
        if (selectors.Count > 0) ClearSelectors();
        selectorParent.GetComponent<RectTransform>().sizeDelta = new Vector2(20, (units.Count + 1) * unitSelectorPrefab.GetComponent<RectTransform>().sizeDelta.y);
        foreach (UnitType type in units)
        {
            Menu_BuyUnits_Selection selector = Instantiate(unitSelectorPrefab, selectorParent);
            selector.Init(Core.Model.Database.GetUnitPrefab(type).GetComponent<Unit>().data);
            selectors.Add(selector);
        }
    }
    void ClearSelectors()
    {
        foreach (Menu_BuyUnits_Selection selector in selectors) Destroy(selector.gameObject);       
        selectors.Clear();
    }
}
