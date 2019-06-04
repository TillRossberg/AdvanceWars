//created by Till Roßberg, 2017-19
using UnityEngine;
using UnityEngine.UI;

public class Panel_Status : MonoBehaviour
{
    //Fields
    public Image unitThumb;
    public Text unitName;
    public Text health;
    public Text ammo;
    public Text fuel;
    public Text tileName;
    public Image tileThumb;
    public Text cover;
    
    public void UpdateDisplay(Tile tile)
    {       
        this.tileName.text = tile.data.tileName;
        this.tileThumb.sprite = tile.data.thumbNail;
        this.cover.text = tile.data.cover.ToString();
        if(tile.isVisible && tile.unitStandingHere != null)
        {
            Unit unit = tile.unitStandingHere;
            this.unitName.text = unit.name;
            this.unitThumb.sprite = unit.data.redThumbNail;
            this.health.text = unit.GetCorrectedHealth().ToString();
            this.ammo.text = unit.ammo.ToString();
            this.fuel.text = unit.fuel.ToString();
        }
        else
        {
            //Hide the part of the status window where the unit is updated
            this.unitName.text = "no name";
            this.unitThumb.sprite = null;
            this.health.text = "-1";
            this.ammo.text = "-1";
            this.fuel.text = "-1";
        }
    }    

    public void UpdateDisplay(Unit unit)
    {
        UpdateDisplay(Core.Model.GetTile(unit.position));
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
   
}
