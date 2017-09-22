using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	//General
	public string unitName;
    public Sprite thumbNail;
    public bool hasTurn = true;//The unit is ready for action.
    public bool hasMoved = false;//States if the unit has already moved this turn.
    public bool canFire = true;//Some units cant fire after thy moved.
    public bool directAttack = false;
    public bool rangeAttack = false;
    public bool teamRed = false;
    public bool teamBlue = false;
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
  
    //Prefabs    
	public Transform reachableTile;
    public Transform attackableTile;
	public Transform unReachableTile;
	public Transform kantenGewichtTextMesh;

    //Properties
    public int baseDamage;
    public int health = 100;
	public int ammo;
    public int fuel;
	public int moveDist;
	public int vision;
	public int range;
    public enum unitType { AntiAir, APC, Tank, Artillery, Rockets, Missiles, Neotank, Recon, Infantry, MdTank, Mech, TCopter, BCopter, Bomber, Fighter, Lander, Battleship, Cruiser, Sub, Pipe };
    public unitType myUnitType; 
    public enum moveType {Foot, Mech, Treads, Wheels, Lander, Ship, Air};
    public moveType myMoveType;
	public int cost;
    public List<Tile> attackableTiles = new List<Tile>();
    public List<Unit> attackableUnits = new List<Unit>();
	

	// Use this for initialization
	void Start () 
	{
		myLevelManager = GameObject.FindGameObjectWithTag ("LevelManager");
        myGraph = myLevelManager.GetComponent<Graph>().getGraph();
    }
	
    private void OnMouseDown()
    {
        //If normal mode is activated.
        if (myLevelManager.GetComponent<MainFunctions>().normalMode)
        {               
            //Einheit als ausgewählt markieren.
            myLevelManager.GetComponent<MainFunctions>().selectUnit(this);
            //Erreichbaren Bereich berechnen und zeichnen.
            calcReachableArea(this.xPos, this.yPos, moveDist, myMoveType);
            myLevelManager.GetComponent<Graph>().drawReachableTiles();
            myLevelManager.GetComponent<MainFunctions>().activateMoveMode();
        }
        else
        //If move mode is activated
        if (myLevelManager.GetComponent<MainFunctions>().moveMode)
        {
            if (isSelected)
            {
                myLevelManager.GetComponent<MainFunctions>().selectedUnit.findAttackableTiles();//Try to find an unit to attack.
                myLevelManager.GetComponent<Graph>().hideReachableTiles();
                //Decide if the menu with firebutton and wait button is opened OR if only the wait button is to display.
                if (myLevelManager.GetComponent<MainFunctions>().selectedUnit.attackableUnits.Count > 0)
                {
                    //Open context menu at the position you want to go to.
                    myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
                }
                else
                {
                    //Open context menu at the position you want to go to.
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
                displayHealth();//Puts the health indicator in the right position.
                attacker.alignUnit(this.xPos, this.yPos);
                attacker.displayHealth();//Puts the health indicator in the right position.
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
        if(health > 5)
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
        //Unblock the tile
        myGraph[xPos][yPos].GetComponent<Tile>().isBlockedByBlue = false;
        myGraph[xPos][yPos].GetComponent<Tile>().isBlockedByRed = false;
        //Set the unit standing on this tile as null.
        myGraph[xPos][yPos].GetComponent<Tile>().setUnitHere(null);
        //Finally delete the unit.
        Destroy(this.gameObject);
    }

    //Move the unit to a field an align it so it faces away, from where it came.
    public void moveUnitTo(int newX, int newY)
    {
        //Only if we drew at least one arrow Path, we should move.
        if (myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count > 1)
        {
            unBlockPositions(newX, newY, this.xPos, this.yPos);            

            //Rotate unit so it faces away from where it came, using the predecessor x and y of the arrow path.
            int pathX = myLevelManager.GetComponent<ArrowBuilder>().arrowPath[myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count - 2].myTileProperties.xPos;
            int pathY = myLevelManager.GetComponent<ArrowBuilder>().arrowPath[myLevelManager.GetComponent<ArrowBuilder>().arrowPath.Count - 2].myTileProperties.yPos;
            preRot = this.rotation;//Remember the previous rotation.
            //Face up
            if (newX == pathX && newY > pathY)
            {
                rotateUnit(90);
            }
            //Face down
            if (newX == pathX && newY < pathY)
            {
                rotateUnit(270);
            }
            //Face right
            if (newY == pathY && newX > pathX)
            {
                rotateUnit(180);
            }
            //Face left
            if (newY == pathY && newX < pathX)
            {
                rotateUnit(0);
            }

            //If a possible path was found, go to the desired position.
            this.transform.position = new Vector3(newX, 0, newY);
            //Reset the unitStandingHere property of the old tile to null
            myGraph[xPos][yPos].GetComponent<Tile>().clearUnit();
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
        //Block/Unblock the positions according to wich team we belong to.
        unBlockPositions(prePosX, prePosY, this.xPos, this.yPos);

        myGraph[xPos][yPos].GetComponent<Tile>().clearUnit();//Reset the unitStandingHere property of the tile, we went to, to null
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

    //Blocks the new position according to the team we belong to and unblocks the old position the same way.
    public void unBlockPositions(int newX, int newY, int oldX, int oldY)
    {
        //Unblock the old position
        //Team Blue
        if (teamBlue)
        {
            myLevelManager.GetComponent<Graph>().setIsBlockedByBlue(oldX, oldY, false);
        }
        //Team Red
        if (teamRed)
        {
            myLevelManager.GetComponent<Graph>().setIsBlockedByRed(oldX, oldY, false);
        }

        //Block the new position
        //Team Blue
        if (teamBlue)
        {
            myLevelManager.GetComponent<Graph>().setIsBlockedByBlue(newX, newY, true);
        }
        //Team Red
        if (teamRed)
        {
            myLevelManager.GetComponent<Graph>().setIsBlockedByRed(newX, newY, true);
        }
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
        }

    }

    public void rotateUnit(int angle)
    {
        this.transform.rotation = Quaternion.Euler(0, angle, 0);
        this.rotation = angle;
    }

    //Creates a list of tiles the unit can attack.
    public void findAttackableTiles()
    {
        switch(myUnitType)
        {
            case unitType.Tank:
                //Try to mark the tiles, the unit can attack and add them to the list.
                //Left
                if(xPos > 0)
                {
                    myGraph[xPos - 1][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos - 1][yPos].GetComponent<Tile>());
                }
                //Right
                if(xPos < myGraph.Count - 1)
                {
                    myGraph[xPos + 1][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(myGraph[xPos + 1][yPos].GetComponent<Tile>());
                }
                //Top
                if(yPos > 0)
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

                break;

            case unitType.Rockets:
                //Try to mark the tiles, the unit can attack and add them to the list.
                //left
                for (int i = 1; i < 4; i++)
                {
                    //Straight
                    if(xPos - 2 - i > -1)
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
                    if (yPos - 2 - i > - 1)
                    {
                        myGraph[xPos][yPos - 2 - i].GetComponent<Tile>().isAttackable = true;
                        attackableTiles.Add(myGraph[xPos][yPos - 2 - i].GetComponent<Tile>());
                    }
                    //Left
                    if (yPos - 1 - i > - 1 && xPos - 1 > 0)
                    {
                        myGraph[xPos - 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                        attackableTiles.Add(myGraph[xPos - 1][yPos - 1 - i].GetComponent<Tile>());
                    }
                    //Right
                    if (yPos - 1 - i > - 1 && xPos + 1 < myGraph.Count)
                    {
                        myGraph[xPos + 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                        attackableTiles.Add(myGraph[xPos + 1][yPos - 1 - i].GetComponent<Tile>());
                    }
                }
                
                //Corners
                //Lower left
                if(xPos - 2 > -1 && yPos - 2 > -1)
                {
                    myGraph[xPos - 2][yPos -2].GetComponent<Tile>().isAttackable = true;
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
                if (xPos + 2 <  myGraph.Count && yPos + 3 < myGraph[0].Count)
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


                break;

            default:

                Debug.Log("Invalid unit type for finding attackable tiles!");
                break;           
        }
        findEnemies();
    }

    //Checks the attackable tiles for enemies.
    public void findEnemies()
    {
        for(int i = 0; i < attackableTiles.Count; i++)
        {
            if(attackableTiles[i].unitStandingHere != null)
            {
                //Only if the unit on the tile is in the opposite team add it to the attackableUnits list.
                if((this.teamBlue && attackableTiles[i].unitStandingHere.teamRed) || (this.teamRed && attackableTiles[i].unitStandingHere.teamBlue))
                {
                    attackableUnits.Add(attackableTiles[i].unitStandingHere);
                }
            }
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

    //Berechnen, wie weit man von einer Position mit einer bestimmten Anzahl von Bewegungspunkten kommt.
    private void calcReachableArea(int x, int y, int Bewegungspunkte, moveType myMoveType)
    {
        switch (myMoveType)
        {
            case moveType.Foot:
                calcReachableAreaFoot(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().footCost, null);
                break;

            case moveType.Mech:
                calcReachableAreaMech(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().mechCost, null);
                break;

            case moveType.Treads:                
                calcReachableAreaTreads(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().treadsCost, null);
                break;

            case moveType.Wheels:
                calcReachableAreaWheels(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().wheelsCost, null);
                break;

            case moveType.Lander:
                calcReachableAreaLander(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().landerCost, null);
                break;

            case moveType.Ship:
                calcReachableAreaShip(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().shipCost, null);
                break;

            case moveType.Air:
                calcReachableAreaAir(x, y, Bewegungspunkte + myGraph[x][y].GetComponent<Tile>().airCost, null);
                break;

            default:
                Debug.Log("Incorrect move type!");
                
                break;
        }
    }
    //Foot
    private void calcReachableAreaFoot(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();
               
        Bewegungspunkte = Bewegungspunkte - myTileProperties.footCost;            
        
        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.footCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {             
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;
            
            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if(meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile )
                {
                int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                calcReachableAreaFoot(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }                     
    }

    //Mech
    private void calcReachableAreaMech(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.mechCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.mechCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaMech(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

    //Treads
    private void calcReachableAreaTreads(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.treadsCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.treadsCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist, wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaTreads(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

    //Wheels
    private void calcReachableAreaWheels(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.wheelsCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.wheelsCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaWheels(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

    //Lander
    private void calcReachableAreaLander(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.landerCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.landerCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaLander(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

    //Ship
    private void calcReachableAreaShip(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.shipCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.shipCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaShip(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

    //Air
    private void calcReachableAreaAir(int x, int y, int Bewegungspunkte, Tile cameFromTile)
    {
        counter++;
        Tile myTileProperties = myGraph[x][y].gameObject.GetComponent<Tile>();

        Bewegungspunkte = Bewegungspunkte - myTileProperties.airCost;

        //Wenn genug Bewegungspunkt übrig sind und das Feld erreichbar ist, ist die Rekursion durchführen.
        if ((Bewegungspunkte >= 0) && (myTileProperties.airCost > 0) && ((teamRed && !myTileProperties.isBlockedByBlue) || (teamBlue && !myTileProperties.isBlockedByRed)))
        {
            List<Transform> meineNachbarn = myTileProperties.nachbarn;
            //Als erreichbar markieren
            myTileProperties.isReachable = true;

            //Das Feld wurde erreicht, also werden alle Nachbarn auf ihre Erreichbarkeit geprüft. Das Feld, von dem man gekommen ist wird ignoriert.
            for (int i = 0; i < meineNachbarn.Count; i++)
            {
                if (meineNachbarn[i].gameObject.GetComponent<Tile>() != cameFromTile)
                {
                    int nachbarX = meineNachbarn[i].GetComponent<Tile>().xPos;
                    int nachbarY = meineNachbarn[i].GetComponent<Tile>().yPos;

                    calcReachableAreaAir(nachbarX, nachbarY, Bewegungspunkte, myTileProperties);
                }
            }
        }
    }

      
}
