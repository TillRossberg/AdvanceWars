using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Context : MonoBehaviour
{
    //References    
    public Image bgPic;
    public Transform buttonParent;
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform rangeButton;
    public RectTransform tileInfoButton;
    public RectTransform occupyButton;
    public RectTransform endTurnButton;
    public RectTransform loadUnitButton;
    public RectTransform unloadUnitButton;
    public RectTransform uniteButton;


    public void Show(Unit unit)
    {
        ActivateMenu();
        TryActivateFireButton(unit);
        TryActivateOccupyButton(unit);
        TryActivateRangeButton(unit);
        TryActivateUnloadButton(unit);
        waitButton.gameObject.SetActive(true);
        //AdaptBGPicSize();
        Core.View.HighlightFirstMenuItem(buttonParent);
    }
    public void Show(Tile tile)
    {
        ActivateMenu();
        tileInfoButton.gameObject.SetActive(true);
        endTurnButton.gameObject.SetActive(true);
        Core.View.HighlightFirstMenuItem(buttonParent);
    }
    public void ShowLoadButton()
    {
        ActivateMenu();
        loadUnitButton.gameObject.SetActive(true);
        Core.View.HighlightFirstMenuItem(buttonParent);
    }
    public void ShowUniteButton()
    {
        ActivateMenu();
        uniteButton.gameObject.SetActive(true);
        Core.View.HighlightFirstMenuItem(buttonParent);
    }
    void ActivateMenu()
    {
        this.gameObject.SetActive(true);
        Core.Controller.CurrentMode = Controller.Mode.ContextMenu;
        DeactivateAllButtons();
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
        Core.Controller.CurrentMode = Controller.Mode.Normal;

    }
    public void Hide(Controller.Mode nextMode)
    {
        this.gameObject.SetActive(false);
        Core.Controller.CurrentMode = nextMode;
    }

    void DeactivateAllButtons()
    {
        for (int i = 0; i < buttonParent.childCount; i++) buttonParent.GetChild(i).gameObject.SetActive(false);
    }
    void TryActivateFireButton(Unit activeUnit)
    {
        if (activeUnit.CanAttack()) fireButton.gameObject.SetActive(true);
    }
    void TryActivateOccupyButton(Unit activeUnit)
    {
        if (Core.Model.GetTile(Core.Controller.Cursor.Position).CanBeOccupiedBy(activeUnit)) occupyButton.gameObject.SetActive(true);
    }
    void TryActivateUnloadButton(Unit activeUnit)
    {
        if (CanDropUnits(activeUnit)) unloadUnitButton.gameObject.SetActive(true);
    }
    bool CanDropUnits(Unit unit)
    {
        Unit_Transporter transporter = unit.GetComponent<Unit_Transporter>();

        if (transporter != null && transporter.loadedUnit != null && transporter.CanDropUnitsHere(Core.Controller.GetSelectedPosition())) return true;
        else return false;
    }
    void TryActivateRangeButton(Unit activeUnit)
    {
        if (activeUnit.data.rangeAttack) rangeButton.gameObject.SetActive(true);
    }
    

    void AdaptBGPicSize()
    {
        int counter = 0;
        float spacing = buttonParent.GetComponent<VerticalLayoutGroup>().spacing;
        float buttonHeight = waitButton.rect.height;
        for (int i = 0; i < buttonParent.childCount; i++)
        {
            if (buttonParent.GetChild(i).gameObject.activeSelf) counter++;
        }
        Debug.Log("active buttons: " + counter);
        Debug.Log("size: " + (spacing + buttonHeight));       
        bgPic.rectTransform.sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.size.x, counter * (buttonHeight + spacing) + buttonHeight + spacing);
    }

}
