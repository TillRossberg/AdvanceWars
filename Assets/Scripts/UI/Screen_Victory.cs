using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Screen_Victory : MonoBehaviour
{
    public TextMeshProUGUI victoryText;

    public Transform ButtonParent;
    public Button restartButton;
   
    public void Show(Team winner)
    {
        Core.View.HideAll();
        this.gameObject.SetActive(true);
        SetVictoryText(winner);
        Core.View.HighlightFirstMenuItem(ButtonParent);
        Core.Controller.BlockInput(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
        Core.Controller.BlockInput(false);
    }
    void SetVictoryText(Team team)
    {
        victoryText.color = team.Data.color;
        victoryText.text = team.Data.teamName + " wins!";
    }
}
