//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_BuyUnits : MonoBehaviour
{
    //Required data structures
    private Transform _levelManager;
    //References
    public RectTransform buyMenu;
    public Transform facilityPanel;
    public Transform harborPanel;
    public Transform airPortPanel;
    public RectTransform buyTankButton;
    public RectTransform buyRocketsButton;

    //Details window
    public GameObject detailsWindow;
    public List<Image> detailedThumbs = new List<Image>();
    public Text moveDistance;
    public Text vision;
    public Text fuel;
    public Text ammunition;
    //Fields
    List<Unit> availableUnits = new List<Unit>();
    int xPos;
    int yPos;
    public bool isOpened = false;
        
    public void init()
    {
        //Find required references.
        _levelManager = GameObject.FindGameObjectWithTag("LevelManager").transform;
        facilityPanel = buyMenu.transform.Find("Facility");
        harborPanel = buyMenu.transform.Find("Harbor");
        airPortPanel = buyMenu.transform.Find("Airport");
        //Deactivate all
        facilityPanel.gameObject.SetActive(false);
        harborPanel.gameObject.SetActive(false);
        airPortPanel.gameObject.SetActive(false);
        detailsWindow.gameObject.SetActive(false);
        buyMenu.gameObject.SetActive(false);
    }
	
    //Open the menu either for a facility, a harbor or an airport.
    public void openMenu(int index)
    {
        //Get the position of the tile we selected, so we can place the new unit there.
        Tile selectedTile = _levelManager.GetComponent<GameFunctions>().getSelectedTile();
        setProductionPosition(selectedTile.xPos, selectedTile.yPos);
        //Get the team that has turn now, so we know for whom we create the new unit.

        //Get available units from the team.


        switch(index)
        {
            //Facility
            case 1:
                buyMenu.gameObject.SetActive(true);
                facilityPanel.gameObject.SetActive(true);
                isOpened = true;

                break;
            //Airport
            case 2:

                break;
            //Harbor
            case 3:

                break;
            default:
                Debug.Log("BuyMenu: no such index found!");
                break;
        }
    }

    //Close buttons
    public void closeMenu()
    {
        facilityPanel.gameObject.SetActive(false);
        harborPanel.gameObject.SetActive(false);
        airPortPanel.gameObject.SetActive(false);
        buyMenu.gameObject.SetActive(false);

        isOpened = false;
    }

    public void closeDetailsWindow()
    {
        detailsWindow.gameObject.SetActive(false);
    }

    //The position of the tile we want to create the new unit on.
    public void setProductionPosition(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
    
    //Get the list of the units a team can build. (LATER!)
    public void setAvailableUnits(List<Unit> newUnitList)
    {
        availableUnits.Clear();
        availableUnits = newUnitList;
    }

    //Buy buttons
    //TODO: make the units created face the HQ of the enemy
    public void buyTankButtonPressed()
    {
        this.GetComponent<UnitCreator>().createUnit(Unit.type.Tank, GetComponent<Manager_Turn>().activeTeam, xPos, yPos, Unit.facingDirection.East);
        closeMenu();
    }

    public void buyRocketsButtonPressed()
    {
        this.GetComponent<UnitCreator>().createUnit(Unit.type.Rockets, GetComponent<Manager_Turn>().activeTeam, xPos, yPos, Unit.facingDirection.East);
        closeMenu();
    }

    public void Button_Buy(int index)
    {
        switch (index)
        {
            //Tank
            case 0:
                this.GetComponent<UnitCreator>().createUnit(Unit.type.Tank, GetComponent<Manager_Turn>().activeTeam, xPos, yPos, Unit.facingDirection.East);
                closeMenu();
                break;
            //Rockets
            case 1:
                this.GetComponent<UnitCreator>().createUnit(Unit.type.Rockets, GetComponent<Manager_Turn>().activeTeam, xPos, yPos, Unit.facingDirection.East);
                closeMenu();
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    //Detail buttons
    public void showDetails(string unitName)
    {
        detailsWindow.SetActive(true);
        switch(unitName)
        {
            case "Tank":
                moveDistance.text = "Distance: 6";
                vision.text = "Vision : 3";
                fuel.text = "Fuel: 99/99";
                ammunition.text = "Ammo: 9/9";
                break;

            case "Rockets":
                moveDistance.text = "Distance: 5";
                vision.text = "Vision : 1";
                fuel.text = "Fuel: 99/99";
                ammunition.text = "Ammo: 6/6";
                break;

            default:
                Debug.Log("BuyUnitMenu: No such unittype found!");
                break;
        }
    }

}
