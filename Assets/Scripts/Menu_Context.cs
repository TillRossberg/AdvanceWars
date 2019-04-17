using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu_Context : MonoBehaviour
{
    //References    
    public EventSystem eventSystem;
          
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform rangeButton;
    public RectTransform tileInfoButton;
    public RectTransform occupyButton;
    public RectTransform endTurnButton;
   
    public void SetButtons(Unit unit)
    {
        DeactivateAllButtons();
        switch (Core.Controller.CurrentMode)
        {
            case Controller.Mode.normal:
                if (unit.attackableUnits.Count > 0 && unit.CanFire) fireButton.gameObject.SetActive(true);
                if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                waitButton.gameObject.SetActive(true);
                tileInfoButton.gameObject.SetActive(true);
                break;            
            case Controller.Mode.move:
                if (unit.attackableUnits.Count > 0) fireButton.gameObject.SetActive(true);
                if (Core.Model.GetTile(unit.position).IsOccupyableBy(unit)) occupyButton.gameObject.SetActive(true);
                waitButton.gameObject.SetActive(true);
                if(Core.Controller.SelectedUnit.position == Core.Controller.Cursor.Position)
                {
                    rangeButton.gameObject.SetActive(true);
                    endTurnButton.gameObject.SetActive(true);

                }
                break;            
            default: throw new System.Exception("SetButtons doesn't use this mode!");                
        }
        eventSystem.SetSelectedGameObject(null);
        Invoke("highlightFirstMenuButton", 0.01f);
    }
    public void SetButtons(int index)
    {
        DeactivateAllButtons();
        if (index == 0)
        {
            tileInfoButton.gameObject.SetActive(true);
            endTurnButton.gameObject.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(null);
        Invoke("highlightFirstMenuButton", 0.01f);
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
   
    //Can go somewhere else... view?
    #region Highlight First Button
    public Transform getFirstActiveButton()
    {
        Transform buttons = GameObject.Find("Buttons").transform;
        foreach (Transform child in buttons)
        {
            if(child.gameObject.activeSelf)
            {
                return child;                
            }            
        }
        Debug.Log("menu cotext: getfirstactivebutton: no active button found!");
        return null;
    }
    private void highlightFirstMenuButton()
    {
        eventSystem.SetSelectedGameObject(getFirstActiveButton().gameObject);
    }
    #endregion

}
