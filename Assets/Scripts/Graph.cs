using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    //General
	public enum tileType {Plain, Road, Forest, Sea, Shore, Structure, Mountain};
    private List<List<Transform>> myGraph = new List<List<Transform>>();

    //Graphic elements
    public Transform tilePrefab;
    public Transform reachableTile;
    public Transform attackableTile;

    public Transform forestPrefab;
    public Transform plainPrefab;
    public Transform roadPrefab;
	public Transform mountainPrefab;

    //Thumbnails
    public Sprite plainThumb;
    public Sprite forestThumb;
    public Sprite roadThumb;
    public Sprite mountainThumb;

	//Graph
	public Transform kantenGewichtTextMesh;

	public int gridHeight = 3;
	public int gridWidth = 3;

	// Use this for initialization
	void Start ()
    {
                 
    }
    
    
    //Create an empty Graph of plain tiles.
    private void createEmptyGraph(int dimX, int dimY)
    {
        
        for (int colIndex = 0; colIndex < dimX; colIndex++)
        {            
            List<Transform> listToAdd = new List<Transform>();
            myGraph.Add(listToAdd);
            for (int rowIndex = 0; rowIndex < dimY; rowIndex++)
            {                
                Transform myTile = createTile("Plain", colIndex, rowIndex, 0);               
                myGraph[colIndex].Add(myTile);               
            }
        }        
    }        
    //Change an existing tile.
    public void changeTile(string newTileName, int x, int y, int angle)
    {
        Destroy(myGraph[x][y].gameObject);
        myGraph[x][y] = createTile(newTileName, x, y, angle);
    }
    //Create a tile using position, angle and name to specify its properties.
    public Transform createTile(string myTileName, int x, int y, int angle)
    {
        Transform myTile;
        Tile myTileProperties;
        switch (myTileName)
        {
            case "Plain":
                //Create tile
                myTile = Instantiate(plainPrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                myTileProperties = myTile.GetComponent<Tile>();
                
                myTileProperties.terrainName = myTileName;
                myTileProperties.thumbnail = plainThumb;
                myTileProperties.xPos = x;
                myTileProperties.yPos = y;
                
                //Set weights
                myTileProperties.weight = 1;
                myTileProperties.cover = 1;                
                myTileProperties.footCost = 1;
                myTileProperties.mechCost = 1;
                myTileProperties.treadsCost = 1;
                myTileProperties.wheelsCost = 2;
                myTileProperties.landerCost = -1;
                myTileProperties.shipCost = -1;
                myTileProperties.airCost = 1;

                //Declare Levelmanager as parent.
                myTile.transform.parent = this.transform.Find("Tiles");
                //Change the name to "terrainName at X: ... Y: ..."
                myTile.name = myTileProperties.terrainName + " at X: " + myTileProperties.xPos + " Y: " + myTileProperties.yPos;

                return myTile;
                
            case "Forest":
                //Create tile
                myTile = Instantiate(forestPrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, angle, 0)));                
                myTileProperties = myTile.GetComponent<Tile>();

                myTileProperties.terrainName = myTileName;
                myTileProperties.thumbnail = forestThumb;
                myTileProperties.xPos = x;
                myTileProperties.yPos = y;

                //Set weights
                myTileProperties.weight = 1;
                myTileProperties.cover = 2;
                myTileProperties.footCost = 1;
                myTileProperties.mechCost = 1;
                myTileProperties.treadsCost = 2;
                myTileProperties.wheelsCost = 3;
                myTileProperties.landerCost = -1;
                myTileProperties.shipCost = -1;
                myTileProperties.airCost = 1;

                //Declare Levelmanager as parent.
                myTile.transform.parent = this.transform.Find("Tiles");
                //Change the name to "terrainName at X: ... Y: ..."
                myTile.name = myTileProperties.terrainName + " at X: " + myTileProperties.xPos + " Y: " + myTileProperties.yPos;

                return myTile;              

            case "Road":
                //Create tile
                myTile = Instantiate(roadPrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                myTileProperties = myTile.GetComponent<Tile>();

                myTileProperties.terrainName = myTileName;
                myTileProperties.thumbnail = roadThumb;
                myTileProperties.xPos = x;
                myTileProperties.yPos = y;

                //Set weights
                myTileProperties.weight = 1;
                myTileProperties.cover = 0;
                myTileProperties.footCost = 1;
                myTileProperties.mechCost = 1;
                myTileProperties.treadsCost = 1;
                myTileProperties.wheelsCost = 1;
                myTileProperties.landerCost = -1;
                myTileProperties.shipCost = -1;
                myTileProperties.airCost = 1;

                //Declare Levelmanager as parent.
                myTile.transform.parent = this.transform.Find("Tiles");
                //Change the name to "terrainName at X: ... Y: ..."
                myTile.name = myTileProperties.terrainName + " at X: " + myTileProperties.xPos + " Y: " + myTileProperties.yPos;

                return myTile;
              
            case "Mountain":
                //Create tile
                myTile = Instantiate(mountainPrefab, new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, angle, 0)));
                myTileProperties = myTile.GetComponent<Tile>();
                
                myTileProperties.terrainName = myTileName;
                myTileProperties.xPos = x;
                myTileProperties.yPos = y;
                //Set weights
                myTileProperties.weight = -1;
                myTileProperties.cover = 4;
                myTileProperties.footCost = 2;
                myTileProperties.mechCost = 1;
                myTileProperties.treadsCost = -1;
                myTileProperties.wheelsCost = -1;
                myTileProperties.landerCost = -1;
                myTileProperties.shipCost = -1;
                myTileProperties.airCost = 1;

                //Declare Levelmanager as parent.
                myTile.transform.parent = this.transform.Find("Tiles");
                //Change the name to "terrainName at X: ... Y: ..."
                myTile.name = myTileProperties.terrainName + " at X: " + myTileProperties.xPos + " Y: " + myTileProperties.yPos;

                return myTile;                

            default:
                Debug.Log("Incorrect Tilename: " + myTileName);
                return null;                
        }
    }
   
    //Block or unblock a tile.
    public void setIsBlockedByBlue(int x, int y, bool value)
    {
        myGraph[x][y].gameObject.GetComponent<Tile>().isBlockedByBlue = value;
    }
    
    public void setIsBlockedByRed(int x, int y, bool value)
    {
        myGraph[x][y].gameObject.GetComponent<Tile>().isBlockedByRed = value;
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
            for(int j = 0; j < myGraph[0].Count; j++)
            {
                myGraph[i][j].GetComponent<Tile>().isReachable = false;
            }
        }
    }

    //Reset the isAttackable bool on all tiles to false and delete red fields.
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
    }

    public List<List<Transform>> getGraph ()
	{
		return myGraph;
	}

    //Examplegraphs
    public void createExampleGraph01()
	{
		createEmptyGraph (gridWidth, gridHeight);
        changeTile("Mountain", 3, 1, 0);
        changeTile("Mountain", 3, 2, 0);
        changeTile("Mountain", 3, 3, 0);
        changeTile("Mountain", 3, 4, 0);
        changeTile("Mountain", 7, 3, 0);
        changeTile("Forest", 4, 2, 0);
        changeTile("Forest", 4, 3, 0);
        changeTile("Forest", 4, 4, 0);
        changeTile("Forest", 4, 5, 0);
        //changeTile("Forest", 7, 2, 0);
        //changeTile("Forest", 8, 2, 0);
        changeTile("Road", 6, 2, 90);
        changeTile("Road", 6, 3, 90);
        changeTile("Road", 6, 4, 90);
        changeTile("Road", 6, 5, 90);
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
        //Linke untere Ecke
        getTile(0, 0).nachbarn.Add(myGraph[1][0]);//rechts
        getTile(0, 0).nachbarn.Add(myGraph[0][1]);//oben
        //Rechte untere Ecke        
        getTile(myGraph.Count - 1, 0).nachbarn.Add(myGraph[myGraph.Count - 1][1]);//oben
        getTile(myGraph.Count - 1, 0).nachbarn.Add(myGraph[myGraph.Count - 2][0]);//links
        //Linke obere Ecke
        getTile(0, myGraph[0].Count - 1).nachbarn.Add(myGraph[1][myGraph[0].Count - 1]);//rechts
        getTile(0, myGraph[0].Count - 1).nachbarn.Add(myGraph[0][myGraph[0].Count - 2]);//unten
        //Rechte obere Ecke
        getTile(myGraph.Count - 1, myGraph[0].Count - 1).nachbarn.Add(myGraph[myGraph.Count - 2][myGraph[0].Count - 1]);//links
        getTile(myGraph.Count - 1, myGraph[0].Count - 1).nachbarn.Add(myGraph[myGraph.Count - 1][myGraph[0].Count - 2]);//unten

        //oberer und unterer Rand
        for (int i = 1; i < myGraph.Count - 1; i++)
        {
            //oberer Rand            
            getTile(i, myGraph[0].Count - 1).nachbarn.Add(myGraph[i - 1][myGraph[0].Count - 1]);//links            
            getTile(i, myGraph[0].Count - 1).nachbarn.Add(myGraph[i + 1][myGraph[0].Count - 1]);//rechts            
            getTile(i, myGraph[0].Count - 1).nachbarn.Add(myGraph[i][myGraph[0].Count - 2]);//unten

            //unterer Rand           
            getTile(i, 0).nachbarn.Add(myGraph[i - 1][0]);//links            
            getTile(i, 0).nachbarn.Add(myGraph[i + 1][0]);//rechts           
            getTile(i, 0).nachbarn.Add(myGraph[i][1]);//oben
        }

        //linker und rechter Rand
        for (int i = 1; i < myGraph[0].Count - 1; i++)
        {
            //linker Rand            
            getTile(0, i).nachbarn.Add(myGraph[0][i + 1]);//oben            
            getTile(0, i).nachbarn.Add(myGraph[0][i - 1]);//unten            
            getTile(0, i).nachbarn.Add(myGraph[1][i]);//rechts

            //rechter Rand            
            getTile(myGraph.Count - 1, i).nachbarn.Add(myGraph[myGraph.Count - 1][i + 1]);//oben           
            getTile(myGraph.Count - 1, i).nachbarn.Add(myGraph[myGraph.Count - 1][i - 1]);//unten            
            getTile(myGraph.Count - 1, i).nachbarn.Add(myGraph[myGraph.Count - 2][i]);//links
        }

        //der Rest
        for (int i = 1; i < myGraph.Count - 1; i++)
        {
            for (int j = 1; j < myGraph[i].Count - 1; j++)
            {               
                getTile(i, j).nachbarn.Add(myGraph[i][j + 1]); //oben                
                getTile(i, j).nachbarn.Add(myGraph[i][j - 1]);//unten                
                getTile(i, j).nachbarn.Add(myGraph[i - 1][j]);//links                
                getTile(i, j).nachbarn.Add(myGraph[i + 1][j]);//rechts
            }
        }
    }

    //Draws the weight for treads on all tiles.
	private void drawTreadsCost()
	{
        //Instantiate parent object for the weight-meshes.
        var gameObject = new GameObject();
        gameObject.transform.parent = this.transform;
        gameObject.name = "weights";
        for (int i = 0; i < myGraph.Count; i++) 
		{
			for (int j = 0; j < myGraph [i].Count; j++) 
			{			
                //Create textMesh
				Transform kantenGewichtTextTransform = Instantiate(kantenGewichtTextMesh, new Vector3 (i, 0	, j), kantenGewichtTextMesh.rotation);				
                //Get tileProperties
                Tile myTileProperties = myGraph[i][j].GetComponent<Tile>();
                //Set the text
                TextMesh kantenGewichtText = kantenGewichtTextTransform.GetComponent<TextMesh>();
                kantenGewichtText.color = Color.red;
                kantenGewichtText.text = "G: " + myTileProperties.treadsCost;				
				kantenGewichtText.name = "Weight at X: " + i + " Y: " + j + " is: " + myTileProperties.treadsCost;
                //Declare Levelmanager/weights as parent.                
                kantenGewichtText.transform.parent = this.transform.Find("weights");
            }
		}
	}

    //Draws the tiles, that can be reached by the unit.
    public void drawReachableTiles()
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

    //Sets the reachable tile to inactive, so they are not visible
    public void hideReachableTiles()
    {
        for (int i = 0; i < this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").transform.childCount; i++)
        {
            this.GetComponent<MainFunctions>().selectedUnit.transform.Find("reachableArea").GetChild(i).gameObject.SetActive(false);
        }
    }

    //Draws the tiles, that can be attacked by the unit.
    public void drawAttackableTilesForRangeAttack()
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

    //Draws the tiles, that can be attacked by the unit.
    public void drawAttackableTilesForDirectAttack()
    {
        Unit selectedUnit = this.GetComponent<MainFunctions>().selectedUnit;
        for (int i = 0; i < myGraph.Count; i++)
        {
            for (int j = 0; j < myGraph[i].Count; j++)
            {
                Unit unitOnTheTile = myGraph[i][j].GetComponent<Tile>().unitStandingHere;
                if (myGraph[i][j].GetComponent<Tile>().isAttackable && unitOnTheTile != null && ((selectedUnit.teamBlue && unitOnTheTile.teamRed) || (selectedUnit.teamRed && unitOnTheTile.teamBlue)))
                {
                    Instantiate(attackableTile, new Vector3(i, 0.1f, j), Quaternion.identity, this.GetComponent<MainFunctions>().selectedUnit.transform.Find("attackableTiles"));
                }
            }
        }
    }
}
