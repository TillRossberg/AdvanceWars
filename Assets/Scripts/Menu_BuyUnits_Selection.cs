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
        Core.View.buyMenu.Buy(_unitType);
        //TODO: play buy sound
    }

    public void OnSelect(BaseEventData eventData)
    {
        Core.View.buyMenu.UpdateDetails(_unitType);
        //TODO: play selection sound
    }
}
