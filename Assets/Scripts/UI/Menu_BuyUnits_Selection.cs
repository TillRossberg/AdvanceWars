using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Menu_BuyUnits_Selection : MonoBehaviour, ISelectHandler
{
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI unitPrice;
    public Image thumbnail;
    UnitType _unitType;

    public void Init(Data_Unit data)
    {
        unitName.text = data.unitName;
        unitPrice.text = data.cost.ToString();
        thumbnail.sprite = data.GetThumbNail(Core.Controller.ActiveTeam);
        _unitType = data.type;
    }

    public void Buy()
    {
        if (Core.View.BuyMenu.CanAffordUnit(_unitType, Core.Controller.ActiveTeam))
        {
            Core.Controller.ActiveTeam.AddUnit(Core.View.BuyMenu.Buy(_unitType, Core.View.BuyMenu.ProductionPosition, Core.Controller.ActiveTeam));            
            //TODO: make unit face enemy hq        
            //TODO: play buy sound
            Core.Controller.BlockInputFor(0.1f);
            Core.View.BuyMenu.Hide();
        }
        else
        {
            Core.AudioManager.PlayNopeSound();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Core.View.BuyMenu.UpdateDetails(_unitType);
        //TODO: play selection sound
    }
}
