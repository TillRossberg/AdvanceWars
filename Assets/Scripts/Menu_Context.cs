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
    #region Set Buttons Dynamically
    public void SetButtons()
    {
        Unit activeUnit = Core.Controller.SelectedUnit;
        if (activeUnit != null)
        {
            switch (Core.Controller.CurrentMode)
            {
                case Controller.Mode.normal:
                    TryActivateFireButton(activeUnit);
                    TryActivateOccupyButton(activeUnit);
                    TryActivateUnloadButton(activeUnit);                    
                    tileInfoButton.gameObject.SetActive(true);
                    break;
                case Controller.Mode.move:
                    TryActivateFireButton(activeUnit);
                    TryActivateOccupyButton(activeUnit);
                    TryActivateUnloadButton(activeUnit);

                    //Click on yourself
                    if (Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                    {
                        TryActivateRangeButton(activeUnit);
                        endTurnButton.gameObject.SetActive(true);
                    }
                    break;
                case Controller.Mode.contextMenu:
                    TryActivateFireButton(activeUnit);
                    TryActivateOccupyButton(activeUnit);
                    TryActivateUnloadButton(activeUnit);

                    //Click on yourself
                    if (Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                    {
                        TryActivateRangeButton(activeUnit);
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
    void TryActivateFireButton(Unit activeUnit)
    {
        if (activeUnit.attackableUnits.Count > 0 && activeUnit.CanFire) fireButton.gameObject.SetActive(true);
    }
    void TryActivateOccupyButton(Unit activeUnit)
    {
        if (Core.Model.GetTile(activeUnit.position).IsOccupyableBy(activeUnit)) occupyButton.gameObject.SetActive(true);
    }
    void TryActivateUnloadButton(Unit activeUnit)
    {
        if (CanDropUnits(activeUnit)) unloadUnitButton.gameObject.SetActive(true);
    }
    bool CanDropUnits(Unit unit)
    {
        Unit_Transporter transporter = unit.GetComponent<Unit_Transporter>();
        Tile targetTile;
        if (unit.targetTile != null) targetTile = unit.targetTile;
        else targetTile = Core.Model.GetTile(unit.position);
        if (transporter != null && transporter.loadedUnit != null && transporter.CanDropUnitsHere(targetTile)) return true;
        else return false;
    }
    void TryActivateRangeButton(Unit activeUnit)
    {
        if (activeUnit.data.directAttack || activeUnit.data.rangeAttack) rangeButton.gameObject.SetActive(true);
    }
    #endregion
}
