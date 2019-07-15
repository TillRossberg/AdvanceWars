//created by Till Roßberg, 2017-19
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_Status : MonoBehaviour
{
    //Fields
    public Image unitThumb;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI health;
    public TextMeshProUGUI ammo;
    public TextMeshProUGUI fuel;
    public TextMeshProUGUI tileName;
    public Image tileThumb;
    public TextMeshProUGUI cover;
    
    public void Show()
    {
        this.gameObject.SetActive(true);
        UpdateDisplay(Core.Model.GetTile(Core.Controller.Cursor.Position));
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateDisplay(Tile tile)
    {       
        this.tileName.text = tile.data.tileName;
        this.tileThumb.sprite = tile.data.thumbNail;
        this.cover.text = tile.data.cover.ToString();
        if(tile.IsVisible && tile.UnitHere != null)
        {
            Unit unit = tile.UnitHere;
            this.unitName.text = unit.data.unitName;
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
        UpdateDisplay(Core.Model.GetTile(unit.Position));
    }


    public void ResetStatus()
    {
        this.unitName.text = "nono";
        this.health.text = "-1";
        this.ammo.text = "-1";
        this.fuel.text = "-1";
        this.tileName.text = "nono";
        this.cover.text = "-1";
    }
   
}
