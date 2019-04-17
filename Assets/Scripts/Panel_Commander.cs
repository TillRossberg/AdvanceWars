using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Panel_Commander : MonoBehaviour
{
    public Image commanderThumbnail;
    public Image commanderFrame;
    public Text activeTeam;
    public Text money;
    public Text roundNr;

    public void UpdateDisplay()
    {
        this.activeTeam.text = "Team: " + Core.Controller.ActiveTeam.data.teamName;
        this.money.text = "$: " + Core.Controller.ActiveTeam.Money.ToString();
        this.roundNr.text = "Round: " + Core.Controller.RoundCounter.ToString();
        this.commanderThumbnail.sprite = Core.Model.Database.GetCommanderThumb(Core.Controller.ActiveTeam.data.commander);
        this.commanderFrame.color = Core.Controller.ActiveTeam.data.teamColor;
    }
}
