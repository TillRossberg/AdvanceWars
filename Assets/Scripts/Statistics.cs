using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    private Database _database;
    public RectTransform statisticsPanelPrefab;
    public RectTransform leftAnchor;
    public RectTransform rightAnchor;
    public Canvas canvas;

    public Container container;
    public GameObject containerPrefab;

    // Use this for initialization
    void Awake ()
    {
        container = getContainer();
        //_database = GetComponent<Database>();
        createStatisticsPanels();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void createStatisticsPanels()
    {
        for (int i = 0; i < container.getTeams().Count; i++)
        {
            if(i == 0)
            {
                createStatisticsPanel(container.getTeams()[i], leftAnchor);
            }
            else
            {
                createStatisticsPanel(container.getTeams()[i], rightAnchor);
            }
        }
    }

    public void createStatisticsPanel(Team team, RectTransform anchor)
    {
        RectTransform newStatisticsPanel = Instantiate(statisticsPanelPrefab, canvas.transform);
        newStatisticsPanel.localPosition = anchor.localPosition;
        setStatisticsPanelValues(newStatisticsPanel, team);
        newStatisticsPanel.GetComponent<StatisticsPanel>().createUnitBuiltPanels(team);
    }

    public void setStatisticsPanelValues(RectTransform panel, Team team)
    {
        panel.Find("PlayerPic").GetComponent<Image>().sprite = team.getPlayerPic();
        panel.Find("KillCountText").GetComponent<Text>().text = team.getUnitsKilledCount().ToString();
        panel.Find("UnitsBuiltText").GetComponent<Text>().text = "Units built: " + team.getUnitsBuiltCounter().ToString();
    }



    //Check if the data container with a game setup created in the main menu is existing and return it. If not, we create a default container.
    public Container getContainer()
    {
        if (GameObject.FindWithTag("Container") != null)
        {
            return GameObject.FindWithTag("Container").GetComponent<Container>();
        }
        else
        {
            Debug.Log("MasterClass: No container found, loading default container!");
            return Instantiate(containerPrefab).GetComponent<Container>();
        }
    }
}
