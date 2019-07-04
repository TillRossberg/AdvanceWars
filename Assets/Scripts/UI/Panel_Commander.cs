using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Panel_Commander : MonoBehaviour
{
    public Image commanderThumbnail;
    public Image commanderFrame;
    public TextMeshProUGUI activeTeam;
    public TextMeshProUGUI money;
    public TextMeshProUGUI roundNr;

    public void Show()
    {
        this.gameObject.SetActive(true);
        UpdateDisplay();
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void UpdateDisplay()
    {
        this.activeTeam.text = "Team: " + Core.Controller.ActiveTeam.data.teamName;
        this.money.text = "$: " + Core.Controller.ActiveTeam.Money.ToString();
        this.roundNr.text = "Round: " + Core.Controller.RoundCounter.ToString();
        this.commanderThumbnail.sprite = Core.Model.Database.GetCommanderThumb(Core.Controller.ActiveTeam.data.commander);
        this.commanderFrame.color = Core.Controller.ActiveTeam.data.color;
    }
}
