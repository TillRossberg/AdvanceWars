using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	//General
	public string unitName;
    public Sprite thumbNail;
    public bool hasTurn = false;//The unit is ready for action.
    public bool hasMoved = true;//States if the unit has already moved this turn.
    public bool canFire = false;//Some units can't fire after they have moved.
    public bool directAttack = false;
    public bool rangeAttack = false;
    public Team myTeam;
    public List<Team> enemyTeams = new List<Team>();
    public bool isSelected = false;
    public int xPos;
    public int yPos;
    public int rotation;
    //Previous position and rotation. (For resetting purposes)
    public int prePosX;
    public int prePosY;
    public int preRot;
    private int counter = 0;//Counts the iterations of the calcReachableArea algorithm.

    //Required data structures
    GameObject myLevelManager;
    private List<List<Transform>> myGraph = new List<List<Transform>>();
  
    //Tilestuff
	public Transform reachableTilePrefab;
    public Transform attackableTilePrefab;
    public List<Tile> attackableTiles = new List<Tile>();
    public List<Unit> attackableUnits = new List<Unit>();
    public List<Tile> reachableTiles = new List<Tile>();

    //Properties
    public int health = 100;
	public int ammo;
    public int maxAmmo;
    public int fuel;
    public int maxFuel;
	public int moveDist;
	public int vision;
    public int minRange;
    public int maxRange;
    public int cost;

    public type myUnitType; 
    public enum type { Flak, APC, Tank, Artillery, Rockets, Missiles, Titantank, Recon, Infantry, MdTank, Mech, TCopter, BCopter, Bomber, Fighter, Lander, Battleship, Cruiser, Sub, Pipe };
    public moveType myMoveType;
    public enum moveType {Foot, Mech, Treads, Wheels, Lander, Ship, Air};


    // Use this for initialization
    void Start () 
	{
		myLevelManager = GameObject.FindGameObjectWithTag ("LevelManager");
        myGraph = myLevelManager.GetComponent<Graph>().getGraph();
        
    }
	
    private void OnMouseDown()
    {
        //If normal mode is activated.
        if (myLevelManager.GetComponent<MainFunctions>().normalMode && !myLevelManager.GetComponent<Menu_BuyUnits>().isOpened && hasTurn)
        {               
            //Einheit als ausgewählt markieren.
            myLevelManager.GetComponent<MainFunctions>().selectUnit(this);
            //Calculate reachable area and instantiate the graphics for the tiles.
            counter = 0;
            calcReachableArea(this.xPos, this.yPos, moveDist, myMoveType, null);
            Debug.Log("Reachable iterations: " + counter);
            myLevelManager.GetComponent<Graph>().createReachableTiles();
            //Calculate attackable area, instantiate the graphics for the tiles and store the attackable units in a list.
            findAttackableTiles();
            myLevelManager.GetComponent<Graph>().createAttackableTiles();
            myLevelManager.GetComponent<Graph>().showAttackableTiles(false);
            findAttackableEnemies();

            myLevelManager.GetComponent<MainFunctions>().activateMoveMode();
        }
        else
        //If move mode is activated
        if (myLevelManager.GetComponent<MainFunctions>().moveMode)
        {
            if (isSelected)
            {                
                //Decide if the menu with firebutton and wait button is opened OR if only the wait button is to display.
                if (myLevelManager.GetComponent<MainFunctions>().selectedUnit.attackableUnits.Count > 0)
                {
                    myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
                }
                else
                {
                    myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 0);
                }               
            }
        }
        else
        //If fire mode is activated.
        if(myLevelManager.GetComponent<MainFunctions>().fireMode)
        {
            //Select unit, that is attackable and pass it to the fight function.
            if(myGraph[xPos][yPos].GetComponent<Tile>().isAttackable)
            {
                Unit attacker = myLevelManager.GetComponent<MainFunctions>().selectedUnit;
                Unit defender = this;
                //Align the units to face each other.
                alignUnit(attacker.xPos, attacker.yPos);
                attacker.alignUnit(this.xPos, this.yPos);
                //Puts the health indicator in the right position.
                displayHealth();
                attacker.displayHealth();

                myLevelManager.GetComponent<BattleMode>().fight(attacker, defender);//Battle
                myLevelManager.GetComponent<MainFunctions>().deselectObject();//Deselect the current unit                
            }
        }
    }

    public void subtractHealth(int healthToSubtract)
    {
        health = health - healthToSubtract;
        displayHealth();
        if(health <= 0)
        {
            killUnit();
        }
    }

    //Displays the actual lifepoints in the "3D"TextMesh
    public void displayHealth()
    {
        //Reposition the health text so it is always at the lower left corner of the tile.
        TextMesh myText = this.GetComponentInChildren<TextMesh>();
        myText.transform.SetPositionAndRotation(new Vector3(this.transform.position.x - 0.4f,this.transform.position.y, this.transform.position.z - 0.2f), Quaternion.Euler(90, 0, 0));
        //If the health goes below five the value will be rounded to 0, but we dont want that!
        if(health > 10)
        {
            myText.text = "" + (int)(health / 10);
        }
        else
        {
            myText.text = "1";
        }
        if(health < 0)
        {
            myText.text = "0";
        }
    }

    //Destroys the unit
    public void killUnit()
    {
        //Set the unit standing on this tile as null.
        myGraph[xPos][yPos].GetComponent<Tile>().clearUnitHere();
        //Boom
        myLevelManager.GetComponent<AnimController>().boom(xPos,yPos);
        //Finally delete the unit.
        Destroy(this.gameObject);
    }

    //Move the unit to a field an align it so it faces away, from where it came.
    public void moveUnitTo(int newX, int newY)
    {
        //Only if we drew at least one arrow Path, we should be able move.
        if (myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count > 1)
        {
            //Rotate unit so it faces away from where it came, using the predecessor x and y of the arrow path.
            int pathX = myLevelManager.GetComponent<ArrowBuilder>().arrowPath[myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count - 2].tile.xPos;
            int pathY = myLevelManager.GetComponent<ArrowBuilder>().arrowPath[myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count - 2].tile.yPos;
            preRot = this.rotation;//Remember the previous rotation.
            //Face up
            if (newX == pathX && newY > pathY){rotateUnit(90);}
            //Face down
            if (newX == pathX && newY < pathY){rotateUnit(270);}
            //Face right
            if (newY == pathY && newX > pathX){rotateUnit(180);}
            //Face left
            if (newY == pathY && newX < pathX){rotateUnit(0);}

            //If a possible path was found, go to the desired position.
            this.transform.position = new Vector3(newX, 0, newY);
            //Reset the unitStandingHere property of the old tile to null
            myGraph[xPos][yPos].GetComponent<Tile>().clearUnitHere();
            //Remember the last position of the unit. (For resetting purposes.)
            prePosX = this.xPos;
            prePosY = this.yPos;            
            //Set xPos and yPos to the new position.
            this.xPos = newX;
            this.yPos = newY;            
            myGraph[xPos][yPos].GetComponent<Tile>().setUnitHere(this);//Inform the new tile, that a unit is standing on it.
            myLevelManager.GetComponent<StatusWindow>().updateCover(xPos, yPos);//When you move the unit, you should see the new cover for the tile it will stand on.
            displayHealth();//Update the display of the health (if the unit rotates, the healthdisplay rotates with it and we dont want that.)
            hasMoved = true;
        }
    }

    //Resets the position and rotation of the unit to where it was before. (If we click the right mouse button or close the menu after we moved it somewhere.)
    public void resetPosition()
    {
        myGraph[xPos][yPos].GetComponent<Tile>().clearUnitHere();//Reset the unitStandingHere property of the tile, we went to, to null
        //Set the position and rotation of the unit to where it was before
        this.transform.position = new Vector3(prePosX, 0, prePosY);
        this.xPos = prePosX;
        this.yPos = prePosY;
        rotateUnit(preRot);
        this.rotation = preRot;
        myLevelManager.GetComponent<StatusWindow>().updateCover(xPos, yPos);//When the unit moves back, the display of the cover should be set to the old tile.
        myGraph[prePosX][prePosX].GetComponent<Tile>().setUnitHere(this);//Inform the old tile, that we are back.
        displayHealth();//Repostition the health indicator.       
        hasMoved = false;
    }

  

    //Aligns the unit so it faces the direction of the given coordinates. 
    public void alignUnit(int targetX, int targetY)
    {
        if(directAttack)
        {
            //For direct attack only!
            //Face right
            if(this.xPos < targetX && this.yPos == targetY)
            {
                rotateUnit(180);
            }
            //Face left
            if(this.xPos > targetX && this.yPos == targetY)
            {            
                rotateUnit(0);
            }
            //Face up
            if(this.yPos < targetY && this.xPos == targetX)
            {
                rotateUnit(90);
            }
            //Face down
            if (this.yPos > targetY && this.xPos == targetX)
            {            
                rotateUnit(270);
            }

        }
        if(rangeAttack)
        {
            //How to decide, if the unit to face is e.g. more to the left than to the bottom ?
            //Compare distances
            //For direct attack only!
            //Face right
            if (this.xPos < targetX && this.yPos == targetY)
            {
                rotateUnit(180);
            }
            //Face left
            if (this.xPos > targetX && this.yPos == targetY)
            {
                rotateUnit(0);
            }
            //Face up
            if (this.yPos < targetY && this.xPos == targetX)
            {
                rotateUnit(90);
            }
            //Face down
            if (this.yPos > targetY && this.xPos == targetX)
            {
                rotateUnit(270);
            }
        }

    }

    public void rotateUnit(int angle)
    {
        this.transform.rotation = Quaternion.Euler(0, angle, 0);
        this.rotation = angle;
        displayHealth();
    }

    //Creates a list of tiles the unit can attack.
    public void findAttackableTiles()
    {
        attackableTiles.Clear();
        if(directAttack)
        {
            findAttackableTileForDirectAttack();
        }
        //TODO: Try to find a cool mathematical solution, not just BRUTE FORCE!
        if(rangeAttack)
        {
            //Try to mark the tiles, the unit can attack and add them to the list.
            //left
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (xPos - 2 - i > -1)
                {
                    myGraph[xPos - 2 - i][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 2 - i][yPos].GetComponent<Tile>());
                }
                //Down
                if (xPos - 1 - i > -1 && yPos - 1 > -1)
                {
                    myGraph[xPos - 1 - i][yPos - 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 1 - i][yPos - 1].GetComponent<Tile>());
                }
                //Up
                if (xPos - 1 - i > -1 && yPos + 1 < myGraph[0].Count)
                {
                    myGraph[xPos - 1 - i][yPos + 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 1 - i][yPos + 1].GetComponent<Tile>());
                }
            }
            //Right
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (xPos + 2 + i < myGraph.Count)
                {
                    myGraph[xPos + 2 + i][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 2 + i][yPos].GetComponent<Tile>());
                }
                //Down
                if (xPos + 1 + i < myGraph.Count && yPos - 1 > -1)
                {
                    myGraph[xPos + 1 + i][yPos - 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 1 + i][yPos - 1].GetComponent<Tile>());
                }
                //Up
                if (xPos + 1 + i < myGraph.Count && yPos + 1 < myGraph[0].Count)
                {
                    myGraph[xPos + 1 + i][yPos + 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 1 + i][yPos + 1].GetComponent<Tile>());
                }
            }
            //Top
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (yPos + 2 + i < myGraph[0].Count)
                {
                    myGraph[xPos][yPos + 2 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos][yPos + 2 + i].GetComponent<Tile>());
                }
                //Left
                if (yPos + 1 + i < myGraph[0].Count && xPos - 1 > -1)
                {
                    myGraph[xPos - 1][yPos + 1 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 1][yPos + 1 + i].GetComponent<Tile>());
                }
                //Right
                if (yPos + 1 + i < myGraph[0].Count && xPos + 1 < myGraph.Count)
                {
                    myGraph[xPos + 1][yPos + 1 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 1][yPos + 1 + i].GetComponent<Tile>());
                }
            }
            //Bottom
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (yPos - 2 - i > -1)
                {
                    myGraph[xPos][yPos - 2 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos][yPos - 2 - i].GetComponent<Tile>());
                }
                //Left
                if (yPos - 1 - i > -1 && xPos - 1 > 0)
                {
                    myGraph[xPos - 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 1][yPos - 1 - i].GetComponent<Tile>());
                }
                //Right
                if (yPos - 1 - i > -1 && xPos + 1 < myGraph.Count)
                {
                    myGraph[xPos + 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 1][yPos - 1 - i].GetComponent<Tile>());
                }
            }

            //Corners
            //Lower left
            if (xPos - 2 > -1 && yPos - 2 > -1)
            {
                myGraph[xPos - 2][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 2][yPos - 2].GetComponent<Tile>());
            }
            if (xPos - 3 > -1 && yPos - 2 > -1)
            {
                myGraph[xPos - 3][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 3][yPos - 2].GetComponent<Tile>());
            }
            if (xPos - 2 > -1 && yPos - 3 > -1)
            {
                myGraph[xPos - 2][yPos - 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 2][yPos - 3].GetComponent<Tile>());
            }

            //Upper left
            if (xPos - 2 > -1 && yPos + 2 < myGraph[0].Count)
            {
                myGraph[xPos - 2][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 2][yPos + 2].GetComponent<Tile>());
            }
            if (xPos - 3 > -1 && yPos + 2 < myGraph[0].Count)
            {
                myGraph[xPos - 3][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 3][yPos + 2].GetComponent<Tile>());
            }
            if (xPos - 2 > -1 && yPos + 3 < myGraph[0].Count)
            {
                myGraph[xPos - 2][yPos + 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos - 2][yPos + 3].GetComponent<Tile>());
            }
            //Upper right
            if (xPos + 2 < myGraph.Count && yPos + 2 < myGraph[0].Count)
            {
                myGraph[xPos + 2][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 2][yPos + 2].GetComponent<Tile>());
            }
            if (xPos + 3 < myGraph.Count && yPos + 2 < myGraph[0].Count)
            {
                myGraph[xPos + 3][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 3][yPos + 2].GetComponent<Tile>());
            }
            if (xPos + 2 < myGraph.Count && yPos + 3 < myGraph[0].Count)
            {
                myGraph[xPos + 2][yPos + 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 2][yPos + 3].GetComponent<Tile>());
            }
            //Lower right
            if (xPos + 2 < myGraph.Count && yPos - 2 > -1)
            {
                myGraph[xPos + 2][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 2][yPos - 2].GetComponent<Tile>());
            }
            if (xPos + 3 < myGraph.Count && yPos - 2 > -1)
            {
                myGraph[xPos + 3][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 3][yPos - 2].GetComponent<Tile>());
            }
            if (xPos + 2 < myGraph.Count && yPos - 3 > -1)
            {
                myGraph[xPos + 2][yPos - 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(myGraph[xPos + 2][yPos - 3].GetComponent<Tile>());
            }

        }
    }

    //Calculates the attackable tiles for direct attack units.
    public void findAttackableTileForDirectAttack()
    {
        //Left
        if (xPos > 0)
        {
            myGraph[xPos - 1][yPos].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(myGraph[xPos - 1][yPos].GetComponent<Tile>());
        }
        //Right
        if (xPos < myGraph.Count - 1)
        {
            myGraph[xPos + 1][yPos].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(myGraph[xPos + 1][yPos].GetComponent<Tile>());
        }
        //Top
        if (yPos > 0)
        {
            myGraph[xPos][yPos - 1].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(myGraph[xPos][yPos - 1].GetComponent<Tile>());
        }
        //Bottom
        if (yPos < myGraph[0].Count - 1)
        {
            myGraph[xPos][yPos + 1].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(myGraph[xPos][yPos + 1].GetComponent<Tile>());
        }
    }

    //Checks the attackable tiles for enemies.
    public void findAttackableEnemies()
    {
        attackableUnits.Clear();
        for(int i = 0; i < attackableTiles.Count; i++)
        {
            if(attackableTiles[i].unitStandingHere != null)
            {
                //Only if the unit on the tile is in the opposite team add it to the attackableUnits list.
                if(isEnemy(attackableTiles[i].unitStandingHere))
                //if((this.teamBlue && attackableTiles[i].unitStandingHere.teamRed) || (this.teamRed && attackableTiles[i].unitStandingHere.teamBlue))
                {
                    attackableUnits.Add(attackableTiles[i].unitStandingHere);
                }
            }
        }
    }

    //Checks if a given unit is in the enemy team of THIS unit. 
    public bool isEnemy(Unit possibleEnemy)
    {
        if(possibleEnemy != null)
        {
            for(int i = 0; i < myTeam.enemyTeams.Count; i++)
            {
                for(int j = 0; j < myTeam.enemyTeams[i].myUnits.Count; j++)
                {
                    if(myTeam.enemyTeams[i].myUnits[j] != null)//Entries of the teamUnits list can be empty! (Destroying an item in a list doesnt fill the gaps!)
                    {
                        if(myTeam.enemyTeams[i].myUnits[j].GetComponent<Unit>() == possibleEnemy)
                        {
                            return true;
                        }
                    }
                }
            }
        }        
        return false;
    }

    //Indicates the units that can be attacked by this unit.
    public void showAttackableEnemies()
    {
        for(int i = 0; i < attackableUnits.Count; i++)
        {
            myLevelManager.GetComponent<MainFunctions>().createMarkingCursor(attackableUnits[i]);
        }
    }

    //Resets all the battle information.
    public void resetBattleInformation()
    {
        attackableUnits.Clear();
        attackableTiles.Clear();
    }

    //Set the thumbnail for the unit
    public void setThumbnail(Sprite myThumb)
    {
        this.thumbNail = myThumb;
    }

    //Calculate how far you can move from a certain position depending on your movement points.
    private void calcReachableArea(int x, int y, int movementPoints, moveType myMoveType, Tile cameFromTile)
    {
        counter++;
        Tile tile = myGraph[x][y].gameObject.GetComponent<Tile>();

        movementPoints = movementPoints - tile.getMovementCost(myMoveType);

        //If enough movement points are left and the tile is passable (we can move through our own units, but are blocked by enemies), do the recursion.
        if ((movementPoints >= 0) && (tile.getMovementCost(myMoveType) > 0) && !isEnemy(tile.unitStandingHere))
        {
            List<Transform> myNeighbors = tile.neighbors;
            tile.isReachable = true;//Mark as rachable.

            //The tile was reached, so test all its neighbors for reachability. Ignore the tile you came from.
            for (int i = 0; i < myNeighbors.Count; i++)
            {
                if (myNeighbors[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int neighborX = myNeighbors[i].GetComponent<Tile>().xPos;
                    int neighborY = myNeighbors[i].GetComponent<Tile>().yPos;

                    calcReachableArea(neighborX, neighborY, movementPoints, myMoveType, tile);
                }
            }
        }
    }      
}
