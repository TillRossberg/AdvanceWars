//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{
    public Canvas statusWindow;

    GameObject unitStatusWindow;
    Image unitThumb;
    Text unitName;
    Text health;
    Text ammo;
    Text fuel;
    Text tileName;
    Image tileThumb;
    Text cover;

    public Image commanderThumbnail;
    public Image commanderFrame;
    public Text activeTeam;
    public Text money;
    public Text roundNr;
    
	// Use this for initialization
	void Start ()
    {
        //Get the graphic elements for the unit status window
        unitStatusWindow = GameObject.Find("Unit_Status");
        unitThumb = GameObject.Find("Unit_Status/Thumbnail_Unit").GetComponent<Image>();
        unitName = GameObject.Find("Unit_Status/Text_Unit_Name").GetComponent<Text>();
        health = GameObject.Find("Unit_Status/Text_Health").GetComponent<Text>();
        ammo = GameObject.Find("Unit_Status/Text_Ammo").GetComponent<Text>();
        fuel = GameObject.Find("Unit_Status/Text_Fuel").GetComponent<Text>();
        tileName = GameObject.Find("Unit_Status/Text_tileName").GetComponent<Text>();
        tileThumb = GameObject.Find("Unit_Status/Thumbnail_Tile").GetComponent<Image>();
        cover = GameObject.Find("Unit_Status/Text_Cover").GetComponent<Text>();


        //(DOESNT WORK!!)Get the graphic elements for the general info 
        //commanderThumbnail = GameObject.Find("CommanderThumbnail").GetComponent<Image>();
        //team = GameObject.Find("ActiveTeam_Text").GetComponent<Text>();
        //money = GameObject.Find("Money_Text").GetComponent<Text>();
        //roundNr = GameObject.Find("RoundNr_Text").GetComponent<Text>();

        unitStatusWindow.SetActive(false);
    }
	
    //De-/activates the status window for the selected unit.
    public void showStatus(bool value)
    {
        if(value)
        {
            resetStatus();
            unitStatusWindow.SetActive(value);
            Unit myUnit = this.GetComponent<MainFunctions>().selectedUnit;
            int myCover = this.GetComponent<MapCreator>().getTile(myUnit.xPos, myUnit.yPos).cover;
            string tileName = this.GetComponent<MapCreator>().getTile(myUnit.xPos, myUnit.yPos).terrainName;
            Sprite tileThumb = this.GetComponent<MapCreator>().getTile(myUnit.xPos, myUnit.yPos).thumbnail;
            changeStatus(myUnit.unitName, myUnit.thumbNail, myUnit.health, myUnit.ammo, myUnit.fuel, tileName, tileThumb, myCover);
        }
        else
        {
            resetStatus();
            unitStatusWindow.SetActive(value);
        } 
    }

    //Displays the active team, its money, the round number and update the thumbnail for the active commander.
    public void displayGeneralInfo()
    {
        TurnManager turnManager = GetComponent<TurnManager>();
        this.activeTeam.text = "Team: " + turnManager.activeTeam.name;
        this.money.text = "$: " + turnManager.activeTeam.money.ToString();
        this.roundNr.text = "Round: " + turnManager.roundCounter.ToString();
        this.commanderThumbnail.sprite = GetComponent<Database>().getCommanderThumb(turnManager.activeTeam.getTeamCommander());
        this.commanderFrame.color = turnManager.activeTeam.teamColor;
    }

    public void changeStatus(string unitName, Sprite unitThumb, int health, int ammo, int fuel, string terrainName, Sprite tileThumb, int cover)
    {
        this.unitName.text = unitName;
        this.unitThumb.sprite = unitThumb;
        this.health.text = getRoundedHealth(health).ToString();
        this.ammo.text = ammo.ToString();
        this.fuel.text = fuel.ToString();
        this.tileName.text = terrainName;
        this.tileThumb.sprite = tileThumb;
        this.cover.text = cover.ToString();
    }

    public void resetStatus()
    {
        this.unitName.text = "nono";
        this.health.text = "-1";
        this.ammo.text = "-1";
        this.fuel.text = "-1";
        this.tileName.text = "nono";
        this.cover.text = "-1";
    }

    //Updates the cover value to the given coordinates.
    public void updateCover(int x, int y)
    {
        int newCover = this.GetComponent<MapCreator>().getTile(x, y).cover;
        string newTileName = this.GetComponent<MapCreator>().getTile(x, y).terrainName;
        Sprite newThumb = this.GetComponent<MapCreator>().getTile(x, y).thumbnail;

        this.cover.text = newCover.ToString();
        this.tileName.text = newTileName;
        this.tileThumb.sprite = newThumb;
    }

    //Just cut the decimal of the healh, so the units look a bit weaker than they really are.
    int getRoundedHealth(int oldHealth)
    {
        int newHealth;
        if (oldHealth > 10)
        {
            newHealth = (int)(oldHealth / 10);
        }
        else
        {
            newHealth = 1;
        }
        if (oldHealth <= 0)
        {
            newHealth = 0;
        }

        return newHealth;
    }

    //Sets the thumbnail for the commander to the given one.
    public void setCommanderThumb(Sprite newThumb)
    {
        commanderThumbnail.sprite = newThumb;
    }
}
