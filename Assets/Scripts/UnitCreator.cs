//created by Till Roßberg, 2017-18
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitCreator : MonoBehaviour
{
    //Required data structures   
    private MapCreator myGraph = new MapCreator();
    public Transform unitPrefab;
    Database database;

    //Graphics
    

    //Teamcolors
    public Material redColor;
    public Material blueColor;

    // Use this for initialization
    void Start ()
    {		
        myGraph = this.GetComponent<MapCreator>();
        database = this.GetComponent<Database>();
    }
	
	public void createUnitSet00()
    {
        Team teamRed = this.GetComponent<TeamManager>().getTeam("TeamRed");
        Team teamBlue = this.GetComponent<TeamManager>().getTeam("TeamBlue");
        createUnit(Database.commander.Andy, Unit.type.Tank, teamRed, 5, 8, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamRed, 5, 7, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamRed, 13, 8, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Mech, teamRed, 5, 6, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Recon, teamRed, 5, 5, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.APC, teamRed, 4, 5, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Artillery, teamRed, 3, 5, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Flak, teamRed, 2, 5, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Rockets, teamRed, 6, 6, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.MdTank, teamRed, 6, 9, Unit.facingDirection.West);

        createUnit(Database.commander.Andy, Unit.type.Tank, teamBlue, 10, 8, Unit.facingDirection.East);
        createUnit(Database.commander.Andy, Unit.type.Rockets, teamBlue, 9, 6, Unit.facingDirection.East);
        createUnit(Database.commander.Andy, Unit.type.Infantry, teamBlue, 9, 9, Unit.facingDirection.East);
    }

    public void createUnitSet01()
    {
        Team teamRed = this.GetComponent<TeamManager>().getTeam("TeamRed");
        Team teamBlue = this.GetComponent<TeamManager>().getTeam("TeamBlue");
        createUnit(Database.commander.Andy, Unit.type.Tank, teamRed, 1, 1, Unit.facingDirection.East);
        createUnit(Database.commander.Andy, Unit.type.Tank, teamRed, 1, 2, Unit.facingDirection.West);
        createUnit(Database.commander.Andy, Unit.type.Tank, teamBlue, 5, 2 , Unit.facingDirection.North);
        createUnit(Database.commander.Andy, Unit.type.Tank, teamBlue, 10, 11 , Unit.facingDirection.South);

    }

    private void setUnit()
    {

    }

    //Create a unit for the given team, position and rotation.
    public void createUnit(Database.commander myCommanderType, Unit.type myUnitType, Team team,  int x, int y, Unit.facingDirection myFacingDirection)
    {
        //Create the Unit
        Transform unitTransform = Instantiate(unitPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0), this.transform.Find(team.name));
        Unit unit = unitTransform.GetComponent<Unit>();
        unit.rotateUnit(myFacingDirection);
        myGraph.getGraph()[x][y].GetComponent<Tile>().setUnitHere(unitTransform);//Pass the unit to the tile it stands on
        setPosition(unit, x, y);
        unit.myUnitType = myUnitType;
        setUnitProperties(unit, myUnitType, myCommanderType);
        this.GetComponent<TeamManager>().addUnit(unitTransform, team);//Add to the correct team list.

        switch(myUnitType)
        {
            case Unit.type.Infantry:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[3];  
                
                unit.unitName = "Infantry";
                unitTransform.name = "Infantry" + unit.myTeam.unitsBuiltCounter;  
                
                unit.myMoveType = Unit.moveType.Foot;
                unit.directAttack = true;
                
                break;

            case Unit.type.Mech:                
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[4];

                unit.unitName = "Mech";
                unitTransform.name = "Mech" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Mech;
                unit.directAttack = true;
                break;

            case Unit.type.Recon:                
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[5];

                unit.unitName = "Recon";
                unitTransform.name = "Recon" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Wheels;
                unit.directAttack = true;
                break;

            case Unit.type.APC:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[6];

                unit.unitName = "APC";
                unitTransform.name = "APC" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Treads;
                unit.directAttack = false;
                break;

            case Unit.type.Artillery:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[7];

                unit.unitName = "Artillery";
                unitTransform.name = "Artillery" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Treads;
                unit.rangeAttack = true;
                break;

            case Unit.type.Flak:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[8];

                unit.unitName = "Flak";
                unitTransform.name = "Flak" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Treads;
                unit.directAttack = true;
                break;

            case Unit.type.Tank:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[0];

                unit.unitName = "Tank";
                unitTransform.name = "Tank" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Treads;
                unit.directAttack = true;
                break;

            case Unit.type.MdTank:
                unit.setThumbnail(database.unitThumbs[0]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[2];

                unit.unitName = "Medium Tank";
                unitTransform.name = "Medium Tank" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Treads;
                unit.directAttack = true;
                break;

            case Unit.type.Rockets:
                unit.setThumbnail(database.unitThumbs[1]);
                unitTransform.GetComponentInChildren<MeshFilter>().mesh = database.meshes[1];

                unit.unitName = "Rockets";
                unitTransform.name = "Rockets" + unit.myTeam.unitsBuiltCounter;

                unit.myMoveType = Unit.moveType.Wheels;
                unit.rangeAttack = true;
                break;

            default:
            Debug.Log("Unitmanager: Unittype not found!");
            break;

        }
    }

    //Sets the rotation and position for a given unit.
    public void setPosition(Unit unit, int x, int y)
    {
        unit.xPos = x;
        unit.yPos = y;
    }

    //Sets the properties for a given unit depending on the commander and the unittype.
    public void setUnitProperties(Unit unit, Unit.type myUnitType, Database.commander myCommanderType)
    {
        unit.maxAmmo = database.getAmmo(myUnitType, myCommanderType);
        unit.ammo = unit.maxAmmo;
        unit.maxFuel = database.getMaxFuel(myUnitType, myCommanderType);
        unit.fuel = unit.maxFuel;
        unit.moveDist = database.getMoveDistance(myUnitType, myCommanderType);
        unit.visionRange = database.getVision(myUnitType, myCommanderType);
        unit.minRange = database.getMinRange(myUnitType, myCommanderType);
        unit.maxRange = database.getMaxRange(myUnitType, myCommanderType);
        unit.cost = database.getCost(myUnitType, myCommanderType);
    }
}
