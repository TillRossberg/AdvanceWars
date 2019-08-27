using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class View : MonoBehaviour
{
    #region References
    public Canvas Canvas;    
    public EventSystem EventSystem;
    public Menu_BuyUnits BuyMenu;
    public Menu_Context ContextMenu;
    public Menu_Details_Tile TileDetails;
    public Panel_Status StatusPanel;
    public Panel_Commander CommanderPanel;
    public Screen_Victory VictoryScreen;
    #endregion
    #region Fields
    List<string> commanderNames { get; set; }
    List<string> weatherNames { get; set; }//List to hold the available weather types for creating the dropdown menu to chose from them.
    #endregion
    #region Parent Object Fields
    public Transform ReachableTilesParent;
    public Transform AttackablTilesParent;
    #endregion
    #region Tile Fields
    List<GameObject> reachableTilesGfx = new List<GameObject>();
    List<GameObject> attackableTilesGfx = new List<GameObject>();
    #endregion

    #region Base Methods
    public void Init()
    {
        weatherNames = System.Enum.GetNames(typeof(Weather)).ToList<string>();
        commanderNames = System.Enum.GetNames(typeof(CommanderType)).ToList<string>();
        if (!Canvas.gameObject.activeSelf) Canvas.gameObject.SetActive(true);
        HideAll();
        StatusPanel.gameObject.SetActive(true);
        CommanderPanel.gameObject.SetActive(true);
    }

    #endregion
    #region Show and Hide Methods
    public void ShowStandardPanels()
    {
        CommanderPanel.Show();
        StatusPanel.Show();
    }
    public void HideAll()
    {
        for (int i = 0; i < Canvas.transform.childCount; i++) Canvas.transform.GetChild(i).gameObject.SetActive(false);            
    }     
    #endregion    
    #region Fog of War Methods    
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
            foreach (Unit unit in team.Units)
            {
                if (unit != null) unit.CalcVisibleArea();
            }
        }
    }
    #endregion
    #region Utility Methods
    public void HighlightFirstMenuItem(Transform menutItemsParent)
    {
        EventSystem.SetSelectedGameObject(null);
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
        EventSystem.SetSelectedGameObject(GetFirstActiveItem(menuParent));
    }
    #endregion
}
