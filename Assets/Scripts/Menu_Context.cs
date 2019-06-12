using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Context : MonoBehaviour
{
    //References    

    public Transform buttonParent;
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform rangeButton;
    public RectTransform tileInfoButton;
    public RectTransform occupyButton;
    public RectTransform endTurnButton;
    public RectTransform loadUnitButton;
    public RectTransform unloadUnitButton;
       
    public void Open(int index)
    {
        DeactivateAllButtons();
        switch (index)
        {
            case 0:
                waitButton.gameObject.SetActive(true);
                break;
            case 1:
                waitButton.gameObject.SetActive(true);
                fireButton.gameObject.SetActive(true);
                break;
            case 2:
                waitButton.gameObject.SetActive(true);
                occupyButton.gameObject.SetActive(true);
                break;
            case 3:
                waitButton.gameObject.SetActive(true);
                fireButton.gameObject.SetActive(true);
                occupyButton.gameObject.SetActive(true);
                break;
            case 4:
                break;
            case 5:
                loadUnitButton.gameObject.SetActive(true);
                break;
            case 6:
                //unload units
                unloadUnitButton.gameObject.SetActive(true);
                waitButton.gameObject.SetActive(true);
                break;
            case 7:
                //statistics button
                tileInfoButton.gameObject.SetActive(true);
                endTurnButton.gameObject.SetActive(true);
                break;
            case 8:
                Unit SelectedUnit = Core.Controller.SelectedUnit;
                if (SelectedUnit.targetTile != null) SelectedUnit.CalcAttackableArea(SelectedUnit.targetTile.position);//You clicked on a target position.
                if (SelectedUnit.targetTile == null) SelectedUnit.CalcAttackableArea(SelectedUnit.position);//You clicked on yourself.
                SelectedUnit.FindAttackableEnemies();
                SetButtons();
                waitButton.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        Core.Controller.CurrentMode = Controller.Mode.contextMenu;
        Core.View.HighlightFirstMenuItem(buttonParent);
    }
    
    public void SetButtons()
    {       
        Unit unit = Core.Controller.SelectedUnit;
        if(unit != null)
        {
            switch (Core.Controller.CurrentMode)
            {
                case Controller.Mode.normal:
                    //Fire
                    if (unit.attackableUnits.Count > 0 && unit.CanFire) fireButton.gameObject.SetActive(true);
                    //Occupy
                    if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                    //Unload unit
                    if (unit.GetComponent<Unit_Transporter>() != null && unit.GetComponent<Unit_Transporter>().loadedUnit != null) unloadUnitButton.gameObject.SetActive(true);              
                    //Info
                    tileInfoButton.gameObject.SetActive(true);
                    break;
                case Controller.Mode.move:
                    //Fire
                    if (unit.attackableUnits.Count > 0) fireButton.gameObject.SetActive(true);
                    //Occupy
                    if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                    //Unload unit
                    if (unit.GetComponent<Unit_Transporter>() != null && unit.GetComponent<Unit_Transporter>().loadedUnit != null) unloadUnitButton.gameObject.SetActive(true);
                    //Click on yourself
                    if (Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                    {
                        if(unit.data.directAttack || unit.data.rangeAttack) rangeButton.gameObject.SetActive(true);
                        endTurnButton.gameObject.SetActive(true);
                    }
                    break;
                case Controller.Mode.contextMenu:
                    if (unit.attackableUnits.Count > 0) fireButton.gameObject.SetActive(true);
                    //Occupy
                    if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                    //Unload unit
                    if (unit.GetComponent<Unit_Transporter>() != null && unit.GetComponent<Unit_Transporter>().loadedUnit != null) unloadUnitButton.gameObject.SetActive(true);
                    //Click on yourself
                    if (Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                    {
                        if (unit.data.directAttack || unit.data.rangeAttack) rangeButton.gameObject.SetActive(true);
                        endTurnButton.gameObject.SetActive(true);
                    }
                    break;
                default: throw new System.Exception("SetButtons doesn't use this mode!");
            }
        }        
    }

    void DeactivateAllButtons()
    {
        for (int i = 0; i < buttonParent.childCount; i++) buttonParent.GetChild(i).gameObject.SetActive(false);
    } 
}
