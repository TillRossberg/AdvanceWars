//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{
    //References
    private Manager _manager;
    public Image commanderThumbnail;
    public Image commanderFrame;
    public Text activeTeam;
    public Text money;
    public Text roundNr;

    //Fields
    Image unitThumb;
    Text unitName;
    Text health;
    Text ammo;
    Text fuel;
    Text tileName;
    Image tileThumb;
    Text cover;
    
    public void init()
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        //Get the graphic elements for the unit status window
        unitThumb = this.transform.Find("Unit_Status/Thumbnail_Unit").GetComponent<Image>();
        unitName = this.transform.Find("Unit_Status/Text_Unit_Name").GetComponent<Text>();
        health = this.transform.Find("Unit_Status/Text_Health").GetComponent<Text>();
        ammo = this.transform.Find("Unit_Status/Text_Ammo").GetComponent<Text>();
        fuel = this.transform.Find("Unit_Status/Text_Fuel").GetComponent<Text>();
        tileName = this.transform.Find("Unit_Status/Text_tileName").GetComponent<Text>();
        tileThumb = this.transform.Find("Unit_Status/Thumbnail_Tile").GetComponent<Image>();
        cover = this.transform.Find("Unit_Status/Text_Cover").GetComponent<Text>();
        
        this.gameObject.SetActive(false);        
    }

    //De-/activates the status window for the selected unit.
    public void showStatus(bool value)
    {
        if(value)
        {
            resetStatus();
            this.gameObject.SetActive(value);
            Unit myUnit = _manager.getGameFunctions().getSelectedUnit();
            int myCover = _manager.getMapCreator().getTile(myUnit.xPos, myUnit.yPos).cover;
            string tileName = _manager.getMapCreator().getTile(myUnit.xPos, myUnit.yPos).terrainName;
            Sprite tileThumb = _manager.getMapCreator().getTile(myUnit.xPos, myUnit.yPos).thumbnail;
            changeStatus(myUnit.unitName, myUnit.thumbNail, myUnit.health, myUnit.ammo, myUnit.fuel, tileName, tileThumb, myCover);
        }
        else
        {
            resetStatus();
            this.gameObject.SetActive(value);
        } 
    }

    //Displays the active team, its money, the round number and update the thumbnail for the active commander.
    public void displayGeneralInfo()
    {
        TurnManager turnManager = _manager.getTurnManager();
        this.activeTeam.text = "Team: " + turnManager.activeTeam.name;
        this.money.text = "$: " + turnManager.activeTeam.money.ToString();
        this.roundNr.text = "Round: " + turnManager.roundCounter.ToString();
        this.commanderThumbnail.sprite = _manager.getDatabase().getCommanderThumb(turnManager.activeTeam.getTeamCommander());
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
        Tile _tile = _manager.getMapCreator().getTile(x, y);
        int newCover = _tile.cover;
        string newTileName = _tile.terrainName;
        Sprite newThumb = _tile.thumbnail;

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
