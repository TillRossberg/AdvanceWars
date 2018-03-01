using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    private int height = 1;//Heigth above the playground.
    Canvas contextMenu;//Graphics
    public RectTransform waitButton;
    public RectTransform fireButton;
    public RectTransform rangeButton;
    public RectTransform tileInfoButton;
    public RectTransform endTurnButton;
    public bool isOpened = false;
    public int clickedHereX;
    public int clickedHereY;

    public bool showAttackableTiles = false;
    public bool showReachableTiles = false;


	// Use this for initialization
	void Start ()
    {
        contextMenu = GameObject.FindGameObjectWithTag("ContextMenu").GetComponent<Canvas>();
        contextMenu.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //Opens the specific context menu at the given position.
    public void openContextMenu(int x, int y, int menuType)
    {
        contextMenu.gameObject.SetActive(true);
        setMenuType(menuType);
        isOpened = true;
        //Store where the user clicked on, for...whatever purpose...
        clickedHereX = x;
        clickedHereY = y;
    }



    //Postitions the context menu, where it is not visible.
    public void resetContextMenu()
    {
        contextMenu.gameObject.SetActive(false);
        clickedHereX = 0;
        clickedHereY = 0;
    }

    //Close the menu.
    public void closeMenu()
    {
        resetContextMenu();
        isOpened = false;
    }

    
    
    //Activate the fire mode, show the enemies that can be attacked and close the menu.
    public void fireButtonPressed()
    {
        this.GetComponent<MainFunctions>().activateFireMode();
        this.GetComponent<Graph>().showReachableTiles(false);
        this.GetComponent<MainFunctions>().selectedUnit.showAttackableEnemies();
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

    //Switch the visiblity of the attackable tiles of the unit.
    public void rangeButtonPressed()
    {
        if (showAttackableTiles)
        {
            this.GetComponent<Graph>().showAttackableTiles(false);
            showAttackableTiles = false;
        }
        else
        {
            //For direct attack
            if (this.GetComponent<MainFunctions>().selectedUnit.directAttack)
            {
                this.GetComponent<Graph>().showAttackableTiles(true);
                showAttackableTiles = true;
            }
            //For range attack
            if (this.GetComponent<MainFunctions>().selectedUnit.rangeAttack)
            {
                this.GetComponent<Graph>().showAttackableTiles(true);
                showAttackableTiles = true;
            }
        }
    }

    //End the turn for the active player and proceed to the next player in the succession.
    public void endTurnButtonPressed()
    {
        GetComponent<TurnManager>().endTurn();
    }

    //Show the tile info.
    public void tileInfoButtonPressed()
    {

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
                rangeButton.gameObject.SetActive(true);
                rangeButton.anchoredPosition = new Vector3(-418, 140, 0);
                break;

            //Firebutton and waitbutton
            case 1:
                deactivateAllButtons();
                fireButton.gameObject.SetActive(true);
                fireButton.anchoredPosition = new Vector3(-418, 184, 0);
                waitButton.gameObject.SetActive(true);
                waitButton.anchoredPosition = new Vector3(-418, 161, 0);
                rangeButton.gameObject.SetActive(true);
                rangeButton.anchoredPosition = new Vector3(-418, 140, 0);
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
        rangeButton.gameObject.SetActive(false);
        tileInfoButton.gameObject.SetActive(false);
    }
}
