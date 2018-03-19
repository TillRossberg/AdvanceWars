using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    //Required data structures
    Database database;

    //General
    private List<List<Transform>> myGraph = new List<List<Transform>>();


    //Graphic elements
    public Transform tilePrefab;
    public Transform reachableTile;
    public Transform attackableTile;
    public Transform fogOfWarTile;
    //Plain
    public Transform plainPrefab;
    public Transform plainWaterPrefab;
    public Transform plainWaterCornerPrefab;
    //Forest
    public Transform forestPrefab;
    //Road
    public Transform roadPrefab;
    public Transform roadBridgePrefab;
    public Transform roadCurvePrefab;
    public Transform roadCrossingPrefab;
    public Transform roadTPartPrefab;
    public Transform roadDeadEndPrefab;
    //Mountain
    public Transform mountainPrefab;
    //Property
    public Transform HQPrefab;
    public Transform cityPrefab;
    public Transform facilityPrefab;
    public Transform airPortPrefab;
    public Transform portPrefab;
    //Sea
    public Transform seaPrefab;
    public Transform reef;

    public enum GraphicType { plain_Normal, plain_Water, plain_WaterCorner, forest_Normal, road_Normal, road_Bridge, road_Curve, road_Crossing, road_TPart, road_Deadend,
                              mountain_Normal, property_HQ, property_City, property_Facility, property_Airport, property_Port, sea_Normal, reef_Normal  };
    public List<Transform> tilePrefabs = new List<Transform>(); // 0 = plain, 1 = forest, 2 = road, 3 = mountain, 4 = river, 5 = shoal, 6 = sea, 7 = reef, 8 = property, 9 = port

    //Thumbnails
    public Sprite plainThumb;
    public Sprite forestThumb;
    public Sprite roadThumb;
    public Sprite mountainThumb;

    //Stuff i dont know where to put else
    private float tileHeight = -0.07f;

	//Graph
	public int gridHeight = 3;
	public int gridWidth = 3;
    
    public void init()
    {
        database = GetComponent<Database>();       
    }

    //Create an empty Graph of plain tiles.
    private void createEmptyGraph(int dimX, int dimY, Tile.type myTileType)
    {
        for (int colIndex = 0; colIndex < dimX; colIndex++)
        {
            myGraph.Add(new List<Transform>());
            for (int rowIndex = 0; rowIndex < dimY; rowIndex++)
            {                
                Transform myTile = createTile(myTileType, colIndex, rowIndex, 0);               
                myGraph[colIndex].Add(myTile);               
            }
        }        
    }        
    //Change an existing tile.
    public void changeTile(int x, int y, int angle, Tile.type myTileType)
    {
        Destroy(myGraph[x][y].gameObject);
        myGraph[x][y] = createTile(myTileType, x, y, angle);
    }

    //Sets the default values for a tile.
    private void setDefaultValues(Transform tileTransform, Tile.type myTileType)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        tile.terrainName = myTileType.ToString();
        tile.myTileType = myTileType;
        tile.xPos = (int)(tileTransform.position.x);
        tile.yPos = (int)(tileTransform.position.z);
        setMovementCost(tile, GetComponent<TurnManager>().getWeather());

        //Create the graphic element for the fog of war and make it invisible.
        tile.fogOfWar = Instantiate(fogOfWarTile, new Vector3(tile.xPos, 0.5f, tile.yPos), Quaternion.identity, this.transform.Find("Tiles").Find("FogOfWar"));
        tile.fogOfWar.gameObject.SetActive(false);
        //Declare Levelmanager as parent.
        tileTransform.transform.parent = this.transform.Find("Tiles");
        //Change the name to "terrainName at X: ... Y: ..."
        tileTransform.name = tile.terrainName + " at X: " + tile.xPos + " Y: " + tile.yPos;
    }

    //Create a tile using position, angle and name to specify its properties.
    public Transform createTile(Tile.type myTileType, int x, int y, int angle)
    {
        Transform tileTransform;
        Tile tile;
        switch (myTileType)
        {
            case Tile.type.Plain:
                //Create tile
                tileTransform = Instantiate(plainPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 1;
                tile.thumbnail = plainThumb;

                return tileTransform;
                
            case Tile.type.Forest:
                //Create tile
                tileTransform = Instantiate(forestPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));                
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 2;
                tile.thumbnail = forestThumb;         

                return tileTransform;              

            case Tile.type.RoadStraight:
                //Create tile
                tileTransform = Instantiate(roadPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 0;
                tile.terrainName = "Road";
                tile.myTileType = Tile.type.RoadStraight;
                tile.thumbnail = roadThumb;               

                return tileTransform;

            case Tile.type.RoadCurve:
                //Create tile
                tileTransform = Instantiate(roadCurvePrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 0;
                tile.terrainName = "Road";
                tile.myTileType = Tile.type.RoadStraight;
                tile.thumbnail = roadThumb;
                

                return tileTransform;

            case Tile.type.Mountain:
                //Create tile
                tileTransform = Instantiate(mountainPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 4;
                
                return tileTransform;

            case Tile.type.HQ:
                //Create tile
                tileTransform = Instantiate(HQPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 4;
                
                return tileTransform;

            case Tile.type.City:
                //Create tile
                tileTransform = Instantiate(cityPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 3;

                return tileTransform;

            case Tile.type.Facility:
                //Create tile
                tileTransform = Instantiate(facilityPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 3;
               
                return tileTransform;

            case Tile.type.Sea:
                //Create tile
                tileTransform = Instantiate(seaPrefab, new Vector3(x, tileHeight, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                tile = tileTransform.GetComponent<Tile>();
                setDefaultValues(tileTransform, myTileType);

                //Individual values.
                tile.cover = 4;
                
                return tileTransform;

            default:
                Debug.Log("Graph: Couldn't find the given tile type: " + myTileType);
                return null;                
        }
    }
   
    //Sets the cost of movement for the tile depending on the type of the tile, weather and the movement type of a unit.
    public void setMovementCost(Tile tile, Database.weather myWeather)
    {
        tile.footCost = database.getMovementCost(tile.myTileType, Unit.moveType.Foot, myWeather);
        tile.mechCost = database.getMovementCost(tile.myTileType, Unit.moveType.Mech, myWeather);
        tile.treadsCost = database.getMovementCost(tile.myTileType, Unit.moveType.Treads, myWeather);
        tile.wheelsCost = database.getMovementCost(tile.myTileType, Unit.moveType.Wheels, myWeather);
        tile.landerCost = database.getMovementCost(tile.myTileType, Unit.moveType.Lander, myWeather);
        tile.shipCost = database.getMovementCost(tile.myTileType, Unit.moveType.Ship, myWeather);
        tile.airCost = database.getMovementCost(tile.myTileType, Unit.moveType.Air, myWeather);
    }

    //Change the weather and with it the movement costs of all tiles.
    public void changeWeather(Database.weather newWeather)
    {
        for(int i = 0; i < myGraph.Count; i++)
        {
            for(int j = 0; j < myGraph[0].Count; i++)
            {
                setMovementCost(myGraph[i][j].GetComponent<Tile>(), newWeather);
            }
        }
    }

    public List<List<Transform>> getGraph ()
	{
		return myGraph;
	}

    //Examplegraphs
    public void createExampleGraph01()
	{
		createEmptyGraph (gridWidth, gridHeight, Tile.type.Plain);
        changeTile(3, 1, 0, Tile.type.Mountain);
        changeTile(3, 2, 0, Tile.type.Mountain);
        changeTile(3, 3, 0, Tile.type.Mountain);
        changeTile(3, 4, 0, Tile.type.Mountain);
        changeTile(7, 3, 0, Tile.type.Mountain);
        changeTile(4, 2, 0, Tile.type.Forest);
        changeTile(4, 3, 0, Tile.type.Forest);
        changeTile(4, 4, 0, Tile.type.Forest);
        changeTile(4, 5, 0, Tile.type.Forest);
        changeTile(7, 2, 0, Tile.type.Forest);
        changeTile(8, 2, 0, Tile.type.Forest);
        changeTile(6, 2, 90, Tile.type.RoadStraight);
        changeTile(6, 3, 90, Tile.type.RoadStraight);
        changeTile(6, 4, 90, Tile.type.RoadStraight);
        changeTile(6, 5, 90, Tile.type.RoadStraight);
        changeTile(0, 0, 0, Tile.type.City);
        changeTile(9, 2, 0, Tile.type.City);
        changeTile(10, 2, 0, Tile.type.City);
        changeTile(11, 2, 0, Tile.type.Facility);
        changeTile(13, 2, 0, Tile.type.Facility);
        findNeighbors();
    }

    public void createLevel01()
    {
        createEmptyGraph(12, 12, Tile.type.Plain);
        changeTile(3, 5, 0, Tile.type.Forest);
        
        findNeighbors();
    }

    public void createLevel00()
    {
        createEmptyGraph(16, 15, Tile.type.Plain);
        //x = 0
        changeTile(0, 0, 0, Tile.type.Sea);
        changeTile(0, 1, 0, Tile.type.Sea);
        changeTile(0, 2, 0, Tile.type.Sea);
        changeTile(0, 3, 0, Tile.type.Sea);
        changeTile(0, 4, 0, Tile.type.Sea);
        changeTile(0, 5, 0, Tile.type.Sea);
        changeTile(0, 6, 0, Tile.type.Sea);
        changeTile(0, 7, 0, Tile.type.Sea);
        changeTile(0, 8, 0, Tile.type.Sea);
        changeTile(0, 9, 0, Tile.type.Sea);
        changeTile(0, 10, 0, Tile.type.Sea);
        changeTile(0, 11, 0, Tile.type.Sea);
        changeTile(0, 12, 0, Tile.type.Sea);
        changeTile(0, 13, 0, Tile.type.Sea);
        changeTile(0, 14, 0, Tile.type.Sea);
        //x = 1
        changeTile(1, 0, 0, Tile.type.Sea);
        changeTile(1, 1, 0, Tile.type.Sea);
        changeTile(1, 2, 0, Tile.type.Sea);
        changeTile(1, 3, 0, Tile.type.Sea);
        changeTile(1, 4, 0, Tile.type.City);
        changeTile(1, 6, 0, Tile.type.Facility);
        changeTile(1, 7, 0, Tile.type.HQ);
        changeTile(1, 8, 0, Tile.type.Facility);
        changeTile(1, 10, 0, Tile.type.City);
        changeTile(1, 12, 0, Tile.type.Sea);
        changeTile(1, 13, 0, Tile.type.Sea);
        changeTile(1, 14, 0, Tile.type.Sea);
        //x = 2
        changeTile(2, 0, 0, Tile.type.Sea);
        changeTile(2, 1, 0, Tile.type.Sea);
        changeTile(2, 2, 0, Tile.type.Sea);
        changeTile(2, 3, 0, Tile.type.Sea);
        changeTile(2, 4, 0, Tile.type.City);
        changeTile(2, 6, 0, Tile.type.Forest);
        changeTile(2, 7, 0, Tile.type.Facility);
        changeTile(2, 10, 0, Tile.type.City);
        changeTile(2, 13, 0, Tile.type.Sea);
        changeTile(2, 14, 0, Tile.type.Sea);
        //x = 3
        changeTile(3, 0, 0, Tile.type.Sea);
        changeTile(3, 1, 0, Tile.type.Sea);
        changeTile(3, 2, 0, Tile.type.Sea);
        changeTile(3, 5, 90, Tile.type.RoadCurve);
        changeTile(3, 6, 90, Tile.type.RoadStraight);
        changeTile(3, 7, 90, Tile.type.RoadStraight);
        changeTile(3, 8, 0, Tile.type.Facility);
        changeTile(3, 10, 0, Tile.type.Mountain);
        changeTile(3, 11, 0, Tile.type.Mountain);
        changeTile(3, 13, 0, Tile.type.Sea);
        changeTile(3, 14, 0, Tile.type.Sea);
        //x = 4
        changeTile(4, 0, 0, Tile.type.Sea);
        changeTile(4, 1, 0, Tile.type.Sea);
        changeTile(4, 4, 0, Tile.type.Forest);
        changeTile(4, 5, 0, Tile.type.RoadStraight);
        changeTile(4, 6, 0, Tile.type.Mountain);
        changeTile(4, 7, 0, Tile.type.Mountain);
        changeTile(4, 8, 0, Tile.type.Mountain);
        changeTile(4, 9, 0, Tile.type.Mountain);
        changeTile(4, 10, 0, Tile.type.Mountain);
        changeTile(4, 11, 0, Tile.type.Mountain);
        changeTile(4, 12, 0, Tile.type.Mountain);
        changeTile(4, 13, 0, Tile.type.Sea);
        changeTile(4, 14, 0, Tile.type.Sea);
        //x = 5
        changeTile(5, 0, 0, Tile.type.Sea);
        changeTile(5, 1, 0, Tile.type.Sea);
        changeTile(5, 4, 0, Tile.type.Forest);
        changeTile(5, 5, 0, Tile.type.RoadCurve);
        changeTile(5, 6, 90, Tile.type.RoadStraight);
        changeTile(5, 7, 90, Tile.type.RoadStraight);
        changeTile(5, 8, 90, Tile.type.RoadStraight);
        changeTile(5, 9, 180, Tile.type.RoadCurve);
        changeTile(5, 10, 0, Tile.type.City);
        changeTile(5, 12, 0, Tile.type.Forest);
        changeTile(5, 14, 0, Tile.type.Sea);
        //x = 6
        changeTile(6, 0, 0, Tile.type.Sea);
        changeTile(6, 1, 0, Tile.type.Sea);
        changeTile(6, 2, 0, Tile.type.Sea);
        changeTile(6, 4, 0, Tile.type.Mountain);
        changeTile(6, 5, 0, Tile.type.City);
        changeTile(6, 7, 0, Tile.type.City);
        changeTile(6, 9, 0, Tile.type.RoadStraight);
        changeTile(6, 10, 0, Tile.type.City);
        changeTile(6, 12, 0, Tile.type.Forest);
        changeTile(6, 14, 0, Tile.type.Sea);
        //x = 7
        changeTile(7, 0, 0, Tile.type.Sea);
        changeTile(7, 1, 0, Tile.type.Sea);
        changeTile(7, 2, 0, Tile.type.Sea);
        changeTile(7, 4, 0, Tile.type.Mountain);
        changeTile(7, 5, 0, Tile.type.Mountain);
        changeTile(7, 6, 0, Tile.type.Mountain);
        changeTile(7, 7, 0, Tile.type.Mountain);
        changeTile(7, 8, 0, Tile.type.Mountain);
        changeTile(7, 9, 0, Tile.type.RoadStraight);
        changeTile(7, 10, 0, Tile.type.City);
        changeTile(7, 12, 0, Tile.type.Forest);
        changeTile(7, 13, 0, Tile.type.Sea);
        changeTile(7, 14, 0, Tile.type.Sea);

        //x = 8
        changeTile(8, 0, 0, Tile.type.Sea);
        changeTile(8, 1, 0, Tile.type.Sea);
        changeTile(8, 2, 0, Tile.type.Sea);
        changeTile(8, 3, 0, Tile.type.Sea);
        changeTile(8, 4, 0, Tile.type.Mountain);
        changeTile(8, 5, 0, Tile.type.Mountain);
        changeTile(8, 6, 0, Tile.type.Mountain);
        changeTile(8, 7, 0, Tile.type.Mountain);
        changeTile(8, 8, 0, Tile.type.Mountain);
        changeTile(8, 9, 0, Tile.type.RoadStraight);
        changeTile(8, 10, 0, Tile.type.City);
        changeTile(8, 12, 0, Tile.type.Forest);
        changeTile(8, 13, 0, Tile.type.Sea);
        changeTile(8, 14, 0, Tile.type.Sea);

        //x = 9
        changeTile(9, 0, 0, Tile.type.Sea);
        changeTile(9, 1, 0, Tile.type.Sea);
        changeTile(9, 2, 0, Tile.type.Sea);
        changeTile(9, 4, 0, Tile.type.Mountain);
        changeTile(9, 5, 0, Tile.type.City);
        changeTile(9, 7, 0, Tile.type.City);
        changeTile(9, 9, 0, Tile.type.RoadStraight);
        changeTile(9, 10, 0, Tile.type.City);
        changeTile(9, 12, 0, Tile.type.Forest);
        changeTile(9, 13, 0, Tile.type.Sea);
        changeTile(9, 14, 0, Tile.type.Sea);

        //x = 10
        changeTile(10, 0, 0, Tile.type.Sea);
        changeTile(10, 1, 0, Tile.type.Sea);
        changeTile(10, 2, 0, Tile.type.Sea);
        changeTile(10, 4, 0, Tile.type.Forest);
        changeTile(10, 5, 90, Tile.type.RoadCurve);
        changeTile(10, 6, 90, Tile.type.RoadStraight);
        changeTile(10, 7, 90, Tile.type.RoadStraight);
        changeTile(10, 8, 90, Tile.type.RoadStraight);
        changeTile(10, 9, 270, Tile.type.RoadCurve);
        changeTile(10, 10, 0, Tile.type.City);
        changeTile(10, 12, 0, Tile.type.Forest);
        changeTile(10, 14, 0, Tile.type.Sea);

        //x = 11
        changeTile(11, 0, 0, Tile.type.Sea);
        changeTile(11, 1, 0, Tile.type.Sea);
        changeTile(11, 4, 0, Tile.type.Forest);
        changeTile(11, 5, 0, Tile.type.RoadStraight);
        changeTile(11, 6, 0, Tile.type.Mountain);
        changeTile(11, 7, 0, Tile.type.Mountain);
        changeTile(11, 8, 0, Tile.type.Mountain);
        changeTile(11, 9, 0, Tile.type.Mountain);
        changeTile(11, 10, 0, Tile.type.Mountain);
        changeTile(11, 11, 0, Tile.type.Mountain);
        changeTile(11, 12, 0, Tile.type.Mountain);
        changeTile(11, 13, 0, Tile.type.Mountain);
        changeTile(11, 14, 0, Tile.type.Sea);

        //x = 12 
        changeTile(12, 0, 0, Tile.type.Sea);
        changeTile(12, 1, 0, Tile.type.Sea);
        changeTile(12, 2, 0, Tile.type.Sea);
        changeTile(12, 4, 0, Tile.type.Forest);
        changeTile(12, 5, 0, Tile.type.RoadCurve);
        changeTile(12, 6, 90, Tile.type.RoadStraight);
        changeTile(12, 7, 90, Tile.type.RoadStraight);
        changeTile(12, 8, 0, Tile.type.Facility);
        changeTile(12, 10, 0, Tile.type.Mountain);
        changeTile(12, 11, 0, Tile.type.Mountain);
        changeTile(12, 12, 0, Tile.type.Mountain);
        changeTile(12, 13, 0, Tile.type.Sea);
        changeTile(12, 14, 0, Tile.type.Sea);

        //x = 13
        changeTile(13, 0, 0, Tile.type.Sea);
        changeTile(13, 1, 0, Tile.type.Sea);
        changeTile(13, 2, 0, Tile.type.Sea);
        changeTile(13, 3, 0, Tile.type.Sea);
        changeTile(13, 4, 0, Tile.type.City);
        changeTile(13, 6, 0, Tile.type.Forest);
        changeTile(13, 7, 0, Tile.type.Facility);
        changeTile(13, 10, 0, Tile.type.City);
        changeTile(13, 13, 0, Tile.type.Sea);
        changeTile(13, 14, 0, Tile.type.Sea);

        //x = 14
        changeTile(14, 0, 0, Tile.type.Sea);
        changeTile(14, 1, 0, Tile.type.Sea);
        changeTile(14, 2, 0, Tile.type.Sea);
        changeTile(14, 3, 0, Tile.type.Sea);
        changeTile(14, 4, 0, Tile.type.City);
        changeTile(14, 6, 0, Tile.type.Facility);
        changeTile(14, 7, 0, Tile.type.HQ);
        changeTile(14, 8, 0, Tile.type.Facility);
        changeTile(14, 10, 0, Tile.type.City);
        changeTile(14, 12, 0, Tile.type.Sea);
        changeTile(14, 13, 0, Tile.type.Sea);
        changeTile(14, 14, 0, Tile.type.Sea);

        //x = 15
        changeTile(15, 0, 0, Tile.type.Sea);
        changeTile(15, 1, 0, Tile.type.Sea);
        changeTile(15, 2, 0, Tile.type.Sea);
        changeTile(15, 3, 0, Tile.type.Sea);
        changeTile(15, 4, 0, Tile.type.Sea);
        changeTile(15, 5, 0, Tile.type.Sea);
        changeTile(15, 6, 0, Tile.type.Sea);
        changeTile(15, 7, 0, Tile.type.Sea);
        changeTile(15, 8, 0, Tile.type.Sea);
        changeTile(15, 9, 0, Tile.type.Sea);
        changeTile(15, 10, 0, Tile.type.Sea);
        changeTile(15, 11, 0, Tile.type.Sea);
        changeTile(15, 12, 0, Tile.type.Sea);
        changeTile(15, 13, 0, Tile.type.Sea);
        changeTile(15, 14, 0, Tile.type.Sea);


        //Team red properties
        Team teamRed = this.GetComponent<TeamManager>().getSuperTeamList()[0][0];
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(1,4));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(1,6));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(1,7));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(1,8));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(1,10));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(2,4));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(2,7));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(2,10));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(3,8));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(6,5));
        this.GetComponent<TeamManager>().occupyProperty(teamRed, getTile(6,7));
    
        //Team blue properties
        Team teamBlue = this.GetComponent<TeamManager>().getSuperTeamList()[1][0];
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(9,5));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(9,7));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(12,8));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(13,4));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(13,7));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(13,10));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(14,4));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(14,6));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(14,7));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(14,8));
        this.GetComponent<TeamManager>().occupyProperty(teamBlue, getTile(14,10));

        findNeighbors();
    }

    //Accesses the tileProperties of a tile by the given position.
    public Tile getTile(int x, int y)
    {
        return myGraph[x][y].gameObject.GetComponent<Tile>();
    }

    //Find the neighbors of all tiles.
    private void findNeighbors()
    {
        //Lower left corner
        getTile(0, 0).neighbors.Add(myGraph[1][0]);//right
        getTile(0, 0).neighbors.Add(myGraph[0][1]);//up
        //Lower right corner       
        getTile(myGraph.Count - 1, 0).neighbors.Add(myGraph[myGraph.Count - 1][1]);//up
        getTile(myGraph.Count - 1, 0).neighbors.Add(myGraph[myGraph.Count - 2][0]);//left
        //Upper left corner
        getTile(0, myGraph[0].Count - 1).neighbors.Add(myGraph[1][myGraph[0].Count - 1]);//right
        getTile(0, myGraph[0].Count - 1).neighbors.Add(myGraph[0][myGraph[0].Count - 2]);//down
        //Upper right corner
        getTile(myGraph.Count - 1, myGraph[0].Count - 1).neighbors.Add(myGraph[myGraph.Count - 2][myGraph[0].Count - 1]);//left
        getTile(myGraph.Count - 1, myGraph[0].Count - 1).neighbors.Add(myGraph[myGraph.Count - 1][myGraph[0].Count - 2]);//down

        //Upper and lower border
        for (int i = 1; i < myGraph.Count - 1; i++)
        {
            //Upper border
            getTile(i, myGraph[0].Count - 1).neighbors.Add(myGraph[i - 1][myGraph[0].Count - 1]);//left            
            getTile(i, myGraph[0].Count - 1).neighbors.Add(myGraph[i + 1][myGraph[0].Count - 1]);//right
            getTile(i, myGraph[0].Count - 1).neighbors.Add(myGraph[i][myGraph[0].Count - 2]);//down

            //Lower border
            getTile(i, 0).neighbors.Add(myGraph[i - 1][0]);//left            
            getTile(i, 0).neighbors.Add(myGraph[i + 1][0]);//right
            getTile(i, 0).neighbors.Add(myGraph[i][1]);//up
        }

        //Left and right border
        for (int i = 1; i < myGraph[0].Count - 1; i++)
        {
            //Left border
            getTile(0, i).neighbors.Add(myGraph[0][i + 1]);//up            
            getTile(0, i).neighbors.Add(myGraph[0][i - 1]);//down
            getTile(0, i).neighbors.Add(myGraph[1][i]);//right

            //Right border
            getTile(myGraph.Count - 1, i).neighbors.Add(myGraph[myGraph.Count - 1][i + 1]);//up           
            getTile(myGraph.Count - 1, i).neighbors.Add(myGraph[myGraph.Count - 1][i - 1]);//down            
            getTile(myGraph.Count - 1, i).neighbors.Add(myGraph[myGraph.Count - 2][i]);//left
        }

        //The rest
        for (int i = 1; i < myGraph.Count - 1; i++)
        {
            for (int j = 1; j < myGraph[i].Count - 1; j++)
            {               
                getTile(i, j).neighbors.Add(myGraph[i][j + 1]); //up                
                getTile(i, j).neighbors.Add(myGraph[i][j - 1]);//down
                getTile(i, j).neighbors.Add(myGraph[i - 1][j]);//left
                getTile(i, j).neighbors.Add(myGraph[i + 1][j]);//right
            }
        }
    }

    //Draws the tiles, that can be reached by the unit.
    public void createReachableTiles()
    {
        if (this.GetComponent<MainFunctions>().selectedUnit.reachableTiles.Count == 0)
        {
            for (int i = 0; i < myGraph.Count; i++)
            {
                for (int j = 0; j < myGraph[i].Count; j++)
                {
                    if (myGraph[i][j].gameObject.GetComponent<Tile>().isReachable)
                    {
                        Instantiate(reachableTile, new Vector3(i, 0, j), Quaternion.identity, this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea"));
                    }
                }
            }
        }
    }

    //Creates the graphics for the tiles, that can be attacked by the unit.
    public void createAttackableTiles()
    {
        for (int i = 0; i < myGraph.Count; i++)
        {
            for (int j = 0; j < myGraph[i].Count; j++)
            {
                if (myGraph[i][j].GetComponent<Tile>().isAttackable)
                {
                    Instantiate(attackableTile, new Vector3(i, 0.1f, j), Quaternion.identity, this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles"));
                }
            }
        }
    }
    
    //Switches the visibility of the graphic element of the fog of war and the enemy units for each tile.
    public void setVisibility()
    {
        for (int i = 0; i < myGraph.Count; i++)
        {
            for (int j = 0; j < myGraph[i].Count; j++)
            {
                if (myGraph[i][j].GetComponent<Tile>().getVisibility())
                {
                    myGraph[i][j].GetComponent<Tile>().fogOfWar.gameObject.SetActive(false);
                    //Check if a unit is standing on this tile and make it visible.
                    if (myGraph[i][j].GetComponent<Tile>().unitStandingHere != null)
                    {
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.Find("Mesh").GetComponent<MeshRenderer>().enabled = true;
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.Find("Lifepoints").GetComponent<MeshRenderer>().enabled = true;
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.GetComponent<BoxCollider>().enabled = true;
                    }
                }
                else
                {
                    myGraph[i][j].GetComponent<Tile>().fogOfWar.gameObject.SetActive(true);
                    //Check if a unit is standing on this tile and make it invisible.
                    if (myGraph[i][j].GetComponent<Tile>().unitStandingHere != null)
                    {
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.Find("Mesh").GetComponent<MeshRenderer>().enabled = false;
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.Find("Lifepoints").GetComponent<MeshRenderer>().enabled = false;//Also set the mesh that indicates the lifepoints to invisible.
                        myGraph[i][j].GetComponent<Tile>().unitStandingHere.GetComponent<BoxCollider>().enabled = false;//Collider needs to be deactivated, so we cannot indicate by clicking if there is a unit.
                    }
                }
            }
        }
    }

    //Sets the reachable tiles to active or inactive, so they are visible or not.
    public void showReachableTiles(bool value)
    {
        for (int i = 0; i < this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").transform.childCount; i++)
        {
            this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").GetChild(i).gameObject.SetActive(value);
        }
        this.GetComponent<ContextMenu>().showReachableTiles = value;//Inform the context menu, if the tiles are visible or not.
    }

    //Sets the attackable tiles to active or inactive, so they are visible or not.
    public void showAttackableTiles(bool value)
    {
        for (int i = 0; i < this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles").transform.childCount; i++)
        {
            this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles").GetChild(i).gameObject.SetActive(value);
        }
        this.GetComponent<ContextMenu>().showAttackableTiles = value;//Inform the context menu, if the tiles are visible or not.
    }

    //Resets the visiblity value of each tile to invisible.
    public void resetFogOfWar()
    {
        if(GetComponent<MasterClass>().container.fogOfWar)
        {
            for (int i = 0; i < myGraph.Count; i++)
            {
                for (int j = 0; j < myGraph[i].Count; j++)
                {
                    myGraph[i][j].GetComponent<Tile>().setVisible(false);
                }
            }
        }
    }

    //Reset the reachable bool on all tiles to false and delete the blue fields.
    public void resetReachableTiles()
    {
        for (int i = 0; i < this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").transform.childCount; i++)
        {
            Destroy(this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").GetChild(i).gameObject);
        }

        for (int i = 0; i < myGraph.Count; i++)
        {
            for (int j = 0; j < myGraph[0].Count; j++)
            {
                myGraph[i][j].GetComponent<Tile>().isReachable = false;
            }
        }
        this.GetComponent<MainFunctions>().selectedUnit.reachableTiles.Clear();
        this.GetComponent<ContextMenu>().showReachableTiles = false;
    }

    //Reset the isAttackable bool on all tiles to false and delete the red fields.
    public void resetAttackableTiles()
    {
        for (int i = 0; i < this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles").transform.childCount; i++)
        {
            Destroy(this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles").GetChild(i).gameObject);
        }

        for (int i = 0; i < myGraph.Count; i++)
        {
            for (int j = 0; j < myGraph[0].Count; j++)
            {
                myGraph[i][j].GetComponent<Tile>().isAttackable = false;
            }
        }
        this.GetComponent<MainFunctions>().selectedUnit.attackableTiles.Clear();
        this.GetComponent<ContextMenu>().showAttackableTiles = false;
    }

    
    
}
