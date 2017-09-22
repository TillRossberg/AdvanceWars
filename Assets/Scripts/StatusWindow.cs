using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{
    public Canvas statusWindow;

    GameObject unitStatus;
    Image unitThumb;
    Text unitName;
    Text health;
    Text ammo;
    Text fuel;
    Text tileName;
    Image tileThumb;
    Text cover;
    
	// Use this for initialization
	void Start ()
    {
        unitStatus = GameObject.Find("Unit_Status");
        unitThumb = GameObject.Find("Unit_Status/Thumbnail_Unit").GetComponent<Image>();
        unitName = GameObject.Find("Unit_Status/Text_Unit_Name").GetComponent<Text>();
        health = GameObject.Find("Unit_Status/Text_Health").GetComponent<Text>();
        ammo = GameObject.Find("Unit_Status/Text_Ammo").GetComponent<Text>();
        fuel = GameObject.Find("Unit_Status/Text_Fuel").GetComponent<Text>();
        tileName = GameObject.Find("Unit_Status/Text_tileName").GetComponent<Text>();
        tileThumb = GameObject.Find("Unit_Status/Thumbnail_Tile").GetComponent<Image>();
        cover = GameObject.Find("Unit_Status/Text_Cover").GetComponent<Text>();
        unitStatus.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void showStatus(bool value)
    {
        resetStatus();
        unitStatus.SetActive(value);
    }

    public void changeStatus(string unitName, Sprite unitThumb, int health, int ammo, int fuel, string terrainName, Sprite tileThumb, int cover)
    {
        this.unitName.text = unitName;
        this.unitThumb.sprite = unitThumb;
        this.health.text = health.ToString();
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
        int newCover = this.GetComponent<Graph>().getTile(x, y).cover;
        string newTileName = this.GetComponent<Graph>().getTile(x, y).terrainName;
        Sprite newThumb = this.GetComponent<Graph>().getTile(x, y).thumbnail;
        this.cover.text = newCover.ToString();
        this.tileName.text = newTileName;
        this.tileThumb.sprite = newThumb;
    }
}
