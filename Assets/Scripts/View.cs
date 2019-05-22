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
    public Menu_BuyUnits buyMenu;
    public Menu_Context contextMenu;
    public Panel_Status statusPanel;
    public Panel_Commander commanderPanel;
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
        buyMenu.gameObject.SetActive(false);
    }
    
    #endregion
    #region Menu Methods
    
    public void DisplayContextMenu(bool value){contextMenu.gameObject.SetActive(value);}
    public void DisplayStatusPanel(bool value){statusPanel.gameObject.SetActive(value);}
    public void DisplayCommanderPanel(bool value){ commanderPanel.gameObject.SetActive(value);}
    public void DisplayBuyMenu(bool value)
    {
        buyMenu.gameObject.SetActive(value);
        DisplayContextMenu(false);
        DisplayStatusPanel(!value);
        DisplayCommanderPanel(!value);
    }
    #endregion
    #region Tilegraphics
    //Draws the tiles, that can be reached.
    public void CreateReachableTilesGfx(Unit unit)
    {
        foreach (Tile tile in unit.reachableTiles)
        {
            reachableTilesGfx.Add(Instantiate(Core.Model.Database.reachableTilePrefab, new Vector3(tile.position.x, 0, tile.position.y), Quaternion.identity, reachableTilesParent));
        }       
    }
    public void ResetReachableTiles(Unit unit)
    {
        foreach (GameObject gfx in reachableTilesGfx)
        {
            Destroy(gfx.gameObject);
        }
        reachableTilesGfx.Clear();
    }
    public void DisplayReachableTiles(bool value)
    {
        if(reachableTilesGfx.Count > 0)
        {
            foreach (GameObject gfx in reachableTilesGfx)
            {
                gfx.SetActive(value);
            }
        }
    }
    //Creates the graphics for the tiles, that can be attacked by the unit.
    public void CreateAttackableTilesGfx(Unit unit)
    {
        foreach (Tile tile in unit.attackableTiles)
        {
            attackableTilesGfx.Add(Instantiate(Core.Model.Database.attackableTilePrefab, new Vector3(tile.position.x, 0.1f, tile.position.y), Quaternion.identity, attackablTilesParent));
        }        
    }
    public void ResetAttackableTiles()
    {
        foreach (GameObject gfx in attackableTilesGfx)
        {
            Destroy(gfx.gameObject);
        }
        attackableTilesGfx.Clear();
    }
    public void DisplayAttackableTiles(bool value)
    {
        if(attackableTilesGfx.Count > 0)
        {
            foreach (GameObject gfx in attackableTilesGfx)
            {
                gfx.SetActive(value);
            }
        }
    }
    public void ToggleAttackableTilesGfx()
    {
        if (attackableTilesGfx.Count > 0)
        {
            DisplayAttackableTiles(!attackableTilesGfx[0].activeSelf);
        }
    }

    #endregion
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
