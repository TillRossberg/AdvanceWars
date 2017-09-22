using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    private int height = 1;//Heigth above the playground.
    Canvas UserInterface;//Graphics
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform tileInfoButton;
    public bool isOpened = false;
    public int clickedHereX;
    public int clickedHereY;

	// Use this for initialization
	void Start ()
    {
        UserInterface = GameObject.FindGameObjectWithTag("ContextMenu").GetComponent<Canvas>();
        UserInterface.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Opens the specific context menu at the given position.
    public void openContextMenu(int x, int y, int menuType)
    {
        UserInterface.gameObject.SetActive(true);
        setMenuType(menuType);
        isOpened = true;
        //Store where the user clicked on, for...whatever purpose...
        clickedHereX = x;
        clickedHereY = y;
    }



    //Postitions the context menu, where it is not visible.
    public void resetContextMenu()
    {
        UserInterface.gameObject.SetActive(false);
        clickedHereX = 0;
        clickedHereY = 0;
    }

    //Close the menu.
    public void closeMenu()
    {
        
        resetContextMenu();
        isOpened = false;
    }

    public void closeMenuAndReset()
    {
        resetContextMenu();
        isOpened = false;
    }
    
    //Activate the fire mode.
    public void fireButtonPressed()
    {
        this.GetComponent<MainFunctions>().activateFireMode();
        this.GetComponent<Graph>().drawAttackableTilesForDirectAttack();
        closeMenu();
    }

    //Wait here and perform no actions.
    public void waitButtonPressed()
    {
        this.GetComponent<MainFunctions>().selectedUnit.moveUnitTo(clickedHereX, clickedHereY);
        this.GetComponent<MainFunctions>().selectedUnit.canFire = false;
        this.GetComponent<MainFunctions>().selectedUnit.hasTurn = false;
        this.GetComponent<MainFunctions>().deselectObject();            
    }

    //Activates the different menu types.
    public void setMenuType(int menuType)
    {
        switch(menuType)
        {
            //Waitbutton only
            case 0:
                deactivateAllButtons();
                waitButton.gameObject.SetActive(true);
                waitButton.anchoredPosition = new Vector3(-418, 184, 0);
                break;

            //Firebutton and waitbutton
            case 1:
                deactivateAllButtons();
                fireButton.gameObject.SetActive(true);
                fireButton.anchoredPosition = new Vector3(-418, 184, 0);
                waitButton.gameObject.SetActive(true);
                waitButton.anchoredPosition = new Vector3(-418, 161, 0);
                break;

            //Infobutton about a tile
            case 2:
                deactivateAllButtons();
                tileInfoButton.anchoredPosition = new Vector3(-418, 184, 0);
                tileInfoButton.gameObject.SetActive(true);

                break;

            default:
                Debug.Log("Invalid menu type chosen!");
                break;
        }
    }

    //Sets all buttons to inactive.
    public void deactivateAllButtons()
    {
        fireButton.gameObject.SetActive(false);
        waitButton.gameObject.SetActive(false);
        tileInfoButton.gameObject.SetActive(false);
    }
}
