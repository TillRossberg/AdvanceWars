//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_BuyUnits : MonoBehaviour
{
    //References       
    public RectTransform buyButton;

    //Details window
    public GameObject detailsWindow;
    public Image unitDetailThumb;
    public Text moveDistance;
    public Text vision;
    public Text fuel;
    public Text ammunition;
    //Fields
    public Unit SelectedUnit { get; private set; }
    List<UnitType> availableUnits;
    public Vector2Int ProductionPosition { get; private set; }
    public bool isOpened = false;
    
    public void DisplayMenu(Tile tile)
    {
        ProductionPosition = tile.position;
        if (tile.data.type == TileType.Facility) DisplayFacility(tile.owningTeam);
        if (tile.data.type == TileType.Airport) DisplayAirport(tile.owningTeam);
        if (tile.data.type == TileType.Port) DisplayPort(tile.owningTeam);
    }

    void DisplayFacility(Team team)
    {
        availableUnits = team.data.GetAvailableGroundUnits();
        Debug.Log("Showing ground units!");
    }
    void DisplayAirport(Team team)
    {
        availableUnits = team.data.GetAvailableAirUnits();
        Debug.Log("Showing air units!");
    }
    void DisplayPort(Team team)
    {
        availableUnits = team.data.GetAvailableNavalUnits();
        Debug.Log("Showing naval units!");
    }
	
    public void UpdateDetails()
    {
        moveDistance.text = SelectedUnit.data.moveDist.ToString();
        vision.text = SelectedUnit.data.visionRange.ToString();
        fuel.text = SelectedUnit.data.maxFuel.ToString();
        ammunition.text = SelectedUnit.data.maxAmmo.ToString();
    }    
    
}
