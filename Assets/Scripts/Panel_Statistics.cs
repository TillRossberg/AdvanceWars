using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Panel_Statistics : MonoBehaviour
{
    public RectTransform unitPanelPrefab;
    public RectTransform leftAnchor;
    public RectTransform rightAnchor;
    private Database _database;
    private float yStride = 25;

    private void Awake()
    {
        _database = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Database>();
    }

    

    public void createUnitBuiltPanels(Team team)
    {
        List<int> unitsBuilt = team.getUnitsBuilt();
        //TODO: find a better way to do this... all
        int counter = 0;
        for (int i = 0; i < unitsBuilt.Count; i++)
        {
            RectTransform newUnitPanel = Instantiate(unitPanelPrefab, this.transform);
            if (i <= 10)
            {
                newUnitPanel.localPosition = new Vector3(leftAnchor.localPosition.x, leftAnchor.localPosition.y - i * yStride, leftAnchor.localPosition.z );
                newUnitPanel.parent = leftAnchor;
            }
            if (i > 10)
            {
                newUnitPanel.localPosition = new Vector3(rightAnchor.localPosition.x, rightAnchor.localPosition.y - counter * yStride, rightAnchor.localPosition.z);
                newUnitPanel.parent = rightAnchor;
                counter++;
            }
            switch (i)
            {
                case 0:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Flak");
                    break;
                case 1:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "APC");
                    break;
                case 2:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Tank");
                    break;
                case 3:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Artillery");
                    break;
                case 4:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[1], unitsBuilt[i], "Rockets");
                    break;
                case 5:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Missile");
                    break;
                case 6:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Titantank");
                    break;
                case 7:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Recon");
                    break;
                case 8:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Infantry");
                    break;
                case 9:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Medium Tank");
                    break;
                case 10:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Mech");
                    break;
                case 11:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Transport Copter");
                    break;
                case 12:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Battle Copter");
                    break;
                case 13:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Bomber");
                    break;
                case 14:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Jet");
                    break;
                case 15:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Landing Ship");
                    break;
                case 16:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Battleship");
                    break;
                case 17:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Cruiser");
                    break;
                case 18:
                    newUnitPanel.GetComponent<Panel_UnitStatistics>().setValues(_database.unitThumbs[0], unitsBuilt[i], "Submarine");
                    break;

                default:
                    Debug.Log("Statistics: createUnitBuildPanels: index not found!");
                    break;
            }
        }
    }
}
