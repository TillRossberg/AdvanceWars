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

    public void Open()
    {
        DeactivateAllButtons();
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
                    //Wait
                    waitButton.gameObject.SetActive(true);
                    //Info
                    tileInfoButton.gameObject.SetActive(true);
                    break;
                case Controller.Mode.move:
                    //Fire
                    if (unit.attackableUnits.Count > 0) fireButton.gameObject.SetActive(true);
                    //Occupy
                    if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                    //Wait
                    waitButton.gameObject.SetActive(true);

                    if (Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                    {
                        rangeButton.gameObject.SetActive(true);
                        endTurnButton.gameObject.SetActive(true);
                    }
                    break;
                case Controller.Mode.contextMenu:
                    break;
                default: throw new System.Exception("SetButtons doesn't use this mode!");
            }
        }
        else
        {
            tileInfoButton.gameObject.SetActive(true);
            endTurnButton.gameObject.SetActive(true);
        }
        Core.View.HighlightFirstMenuItem(buttonParent);
    }

    void DeactivateAllButtons()
    {
        waitButton.gameObject.SetActive(false);
        fireButton.gameObject.SetActive(false);
        rangeButton.gameObject.SetActive(false);
        tileInfoButton.gameObject.SetActive(false);
        occupyButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(false);
    } 
}
