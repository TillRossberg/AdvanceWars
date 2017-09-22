using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitManager : MonoBehaviour
{
    //General
    public List<Transform> teamRed = new List<Transform>();
    private int redUnitCounter = 0;//Counts the overal created units.
    public List<Transform> teamBlue = new List<Transform>();
    private int blueUnitCounter = 0;//Counts the overal created units.

    //Required data structures   
    private Graph myGraph = new Graph();
    public Transform unitPrefab;
    public Sprite tankThumb;
    public Sprite rocketsThumb;

    //Graphics
    public Mesh tankMesh;
    public Mesh rocketsMesh;

    //Teamcolors
    public Material redColor;
    public Material blueColor;

    //Enums
    //Team
    public enum team { red, blue};
    public team myTeam;
    //Type of the Unit
    public enum unitType { Infantry, Mech, Recon, APC, Tank, MediumTank, NeoTank, AntiAir, Artillery, Rockets, Missiles, Battleship, Sub, Cruiser, Lander, Fighter, Bomber, BattleCopter, TransportCopter};
    public unitType myUnitType;


    // Use this for initialization
    void Start ()
    {		
        myGraph = this.GetComponent<Graph>();       
    }
	
	public void createUnitSet01()
    {
        createUnit(unitType.Tank, "red", 8, 4, 90);
        createUnit(unitType.Tank, "red", 7, 4, 90);
        createUnit(unitType.Tank, "blue", 8, 5, 90);
        createUnit(unitType.Tank, "blue", 7, 5, 90);
        createUnit(unitType.Rockets, "red", 10, 6, 90);
    }
    public void createUnit(unitType myUnitType, string team,  int x, int y, int rotation)
    {
        string teamName = "";
        string unitName = "";
        int unitCounter = 0;
        switch(myUnitType)
        {
            case unitType.Tank:

                //Init
                if (team == "red")
                {
                    teamName = "TeamRed";
                    unitName = "Red Tank";
                    unitCounter = redUnitCounter;
                }
                if (team == "blue")
                {
                    teamName = "TeamBlue";
                    unitName = "Blue Tank";
                    unitCounter = blueUnitCounter;
                }
                //Create the Unit
                Transform myUnit = Instantiate(unitPrefab, new Vector3(x, 0, y),Quaternion.Euler(0, rotation, 0) ,this.transform.Find(teamName));
                Unit myProperties = myUnit.GetComponent<Unit>();


                //Set the Properties
                myUnit.name = unitName + unitCounter;
                myProperties.unitName = "Tank";
                myProperties.setThumbnail(tankThumb);
                myProperties.xPos = x;
                myProperties.yPos = y;
                myProperties.rotation = rotation;
                myProperties.myUnitType = Unit.unitType.Tank;
                myProperties.myMoveType = Unit.moveType.Treads;
                myUnit.GetComponent<MeshFilter>().mesh = tankMesh;

                myProperties.directAttack = true;
                myProperties.ammo = 9;
                myProperties.fuel = 100;
                myProperties.moveDist = 6;
                myProperties.vision = 3;

                //Add to the correct team list.
                if (team == "red")
                {
                    myProperties.teamRed = true;
                    myProperties.GetComponent<MeshRenderer>().material = redColor;
                    redUnitCounter++;
                    myGraph.setIsBlockedByRed(x, y, true);
                    myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(myProperties);//Pass the unit to the tile it stands on
                    teamRed.Add(myUnit);
                }
                if (team == "blue")
                {
                    myProperties.teamBlue = true;
                    myProperties.GetComponent<MeshRenderer>().material = blueColor;
                    blueUnitCounter++;
                    myGraph.setIsBlockedByBlue(x, y, true);
                    myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(myProperties);//Pass the unit to the tile it stands on
                    teamBlue.Add(myUnit);
                }
                break;

            case unitType.Rockets:
                //Init
                if (team == "red")
                {
                    teamName = "TeamRed";
                    unitName = "Red Rockets";
                    unitCounter = redUnitCounter;
                }
                if (team == "blue")
                {
                    teamName = "TeamBlue";
                    unitName = "Blue Rockets";
                    unitCounter = blueUnitCounter;
                }
                //Create the Unit
                myUnit = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, rotation, 0), this.transform.Find(teamName));
                myProperties = myUnit.GetComponent<Unit>();


                //Set the Properties
                myUnit.name = unitName + unitCounter;
                myProperties.unitName = "Rockets";
                myProperties.setThumbnail(rocketsThumb);
                myProperties.xPos = x;
                myProperties.yPos = y;
                myProperties.rotation = rotation;
                myProperties.myUnitType = Unit.unitType.Rockets;
                myProperties.myMoveType = Unit.moveType.Wheels;
                myUnit.GetComponent<MeshFilter>().mesh = rocketsMesh;

                myProperties.rangeAttack = true;
                myProperties.ammo = 6;
                myProperties.fuel = 50;
                myProperties.moveDist = 5;
                myProperties.vision = 1;

                //Add to the correct team list.
                if (team == "red")
                {
                    myProperties.teamRed = true;
                    myProperties.GetComponent<MeshRenderer>().material = redColor;
                    redUnitCounter++;
                    myGraph.setIsBlockedByRed(x, y, true);
                    myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(myProperties);//Pass the unit to the tile it stands on
                    teamRed.Add(myUnit);
                }
                if (team == "blue")
                {
                    myProperties.teamBlue = true;
                    myProperties.GetComponent<MeshRenderer>().material = blueColor;
                    blueUnitCounter++;
                    myGraph.setIsBlockedByBlue(x, y, true);
                    myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(myProperties);//Pass the unit to the tile it stands on
                    teamBlue.Add(myUnit);
                }

                break;

            default:
            Debug.Log("Unittype not found!");
            break;

        }
    }
}
