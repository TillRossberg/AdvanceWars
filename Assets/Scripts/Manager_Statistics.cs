using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Statistics : MonoBehaviour
{

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
        List<Team> teams = Core.Model.teams;
        for (int i = 0; i < teams.Count; i++)
        {
            if(i == 0)
            {
                createStatisticsPanel(teams[i], leftAnchor);
            }
            else
            {
                createStatisticsPanel(teams[i], rightAnchor);
            }
        }
    }

    public void createStatisticsPanel(Team team, RectTransform anchor)
    {
        RectTransform newStatisticsPanel = Instantiate(statisticsPanelPrefab, canvas.transform);
        newStatisticsPanel.localPosition = anchor.localPosition;
        setStatisticsPanelValues(newStatisticsPanel, team);
        newStatisticsPanel.GetComponent<Panel_Statistics>().createUnitBuiltPanels(team);
    }

    public void setStatisticsPanelValues(RectTransform panel, Team team)
    {
        panel.Find("PlayerPic").GetComponent<Image>().sprite = Core.Model.Database.GetCommanderThumb(team.data.commander);
        panel.Find("KillCountText").GetComponent<Text>().text = team.data.GetUnitsKilledCount().ToString();
        panel.Find("UnitsBuiltText").GetComponent<Text>().text = "Units built: " + team.data.GetUnitsBuiltCounter().ToString();
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
            Debug.Log("Statistics: No container found, loading default container!");
            return Instantiate(containerPrefab).GetComponent<Container>();
        }
    }
}
