using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class View : MonoBehaviour
{
    #region References
    public Canvas canvas;
    public EventSystem eventSystem;
    public Menu_BuyUnits BuyMenu;
    public Menu_Context ContextMenu;
    public Panel_Status StatusPanel;
    public Panel_Commander CommanderPanel;
    public Menu_Details_Tile TileDetails;
    #endregion
    #region Parent Object Fields
    public Transform reachableTilesParent;
    public Transform attackablTilesParent;
    #endregion
    public List<string> commanderNames { get; private set; }
    public List<string> weatherNames { get; private set; }//List to hold the available weather types for creating the dropdown menu to chose from them.

    #region Tile Fields
    List<GameObject> reachableTilesGfx = new List<GameObject>();
    List<GameObject> attackableTilesGfx = new List<GameObject>();
    #endregion

    #region Base Methods
    public void Init()
    {
        weatherNames = System.Enum.GetNames(typeof(Weather)).ToList<string>();
        commanderNames = System.Enum.GetNames(typeof(CommanderType)).ToList<string>();
        if (!canvas.gameObject.activeSelf) canvas.gameObject.SetActive(true);
        BuyMenu.gameObject.SetActive(false);
        ContextMenu.gameObject.SetActive(false);
        TileDetails.gameObject.SetActive(false);
    }

    #endregion  
    public void ShowStandardPanels()
    {
        CommanderPanel.Show();
        StatusPanel.Show();
    }
    public void HideAllMenus()
    {
        BuyMenu.Hide();
        ContextMenu.Hide();
        CommanderPanel.Hide();
        StatusPanel.Hide();
    }      
    #region Fog of War
    
    //Resets the visiblity value of each tile to invisible and calculates the visibility for the given team.
    public void UpdateFogOfWar(Team team)
    {        
        if(Core.Model.MapSettings.fogOfWar)
        {
            for (int i = 0; i < Core.Model.MapMatrix.Count; i++)
            {
                for (int j = 0; j < Core.Model.MapMatrix[i].Count; j++)
                {
                    Core.Model.SetVisibility(new Vector2Int(i, j), false);
                }
            }
            foreach (Unit unit in team.units)
            {
                if (unit != null) unit.CalcVisibleArea();
            }
        }
    }
    #endregion
    #region Select first menu item
    public void HighlightFirstMenuItem(Transform menutItemsParent)
    {
        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(SelectItemDelayed(0.0001f, menutItemsParent));
    }
    GameObject GetFirstActiveItem(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.activeSelf)
            {

                return parent.GetChild(i).gameObject;
            }
        }
        Debug.Log("menu cotext: getfirstactivebutton: no active button found!");
        return null;
    }
    IEnumerator SelectItemDelayed(float delay, Transform menuParent)
    {
        yield return new WaitForSeconds(delay);
                //Debug.Log(item.name);
        eventSystem.SetSelectedGameObject(GetFirstActiveItem(menuParent));
    }
    #endregion
}
