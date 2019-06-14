using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_Details_Tile : MonoBehaviour
{
    public TextMeshProUGUI TileName;
    public TextMeshProUGUI Cover;
    public Image TileThumb;
    public TextMeshProUGUI Infantry;
    public TextMeshProUGUI AntiTank;
    public TextMeshProUGUI Tires;
    public TextMeshProUGUI Treads;
    public TextMeshProUGUI Air;
    public TextMeshProUGUI Sea;

    public void Show(Tile tile)
    {
        Core.Controller.CurrentMode = Controller.Mode.ShowTileDetails;
        this.gameObject.SetActive(true);
        UpdateDetails(tile);
    }
    public void Hide()
    {
        Core.Controller.CurrentMode = Controller.Mode.Normal;
        this.gameObject.SetActive(false);
    }
    void UpdateDetails(Tile tile)
    {
        TileName.text = tile.data.tileName;
        Cover.text = "Cover: " + tile.data.cover.ToString();
        TileThumb.sprite = tile.data.thumbNail;
        Infantry.text = "Infantry: " + GetCorrectText(tile.data.footCost);
        AntiTank.text = "Anti Tank: " + GetCorrectText(tile.data.mechCost);
        Tires.text = "Tires: " + GetCorrectText(tile.data.wheelsCost);
        Treads.text = "Treads: " + GetCorrectText(tile.data.treadsCost);
        Air.text = "Air: " + GetCorrectText(tile.data.airCost);
        Sea.text = "Sea: " + GetCorrectText(tile.data.shipCost);
    }

    string GetCorrectText(int value)
    {
        if (value > 0) return value.ToString();
        else return "-";
    }

}
