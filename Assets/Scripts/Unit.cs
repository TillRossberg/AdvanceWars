//created by Till Roßberg, 2017-18
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
    private bool isInterrupted = false;//If we move through terrain that is covered by fog of war, we can be interrupted by an invisible enemy unit that is on our arrowpath.
    public bool directAttack = false;
    public bool rangeAttack = false;
    public Team myTeam;
    public List<Team> enemyTeams = new List<Team>();
    public bool isSelected = false;
    public int xPos;
    public int yPos;
    public int rotation;
    public facingDirection myFacingDirection;
    public enum facingDirection { North, East, South, West};
    //Previous position and rotation. (For resetting purposes)
    public int prePosX;
    public int prePosY;
    public Unit.facingDirection preDirection;
    private int counter = 0;//Counts the iterations of the calcReachableArea algorithm.

    //Required data structures
    GameObject myLevelManager;
    private List<List<Transform>> graphMatrix = new List<List<Transform>>();
    MapCreator graph;
    //Tilestuff
	public Transform reachableTilePrefab;
    public Transform attackableTilePrefab;
    public List<Tile> attackableTiles = new List<Tile>();
    public List<Unit> attackableUnits = new List<Unit>();
    public List<Tile> reachableTiles = new List<Tile>();

    //Properties
    public int health = 100;
    private TextMesh healthText; 
	public int ammo;
    public int maxAmmo;
    public int fuel;
    public int maxFuel;
	public int moveDist;
	public int visionRange;
    public int minRange;
    public int maxRange;
    public int cost;

    public type myUnitType; 
    public enum type { Flak, APC, Tank, Artillery, Rockets, Missiles, Titantank, Recon, Infantry, MdTank, Mech, TCopter, BCopter, Bomber, Fighter, Lander, Battleship, Cruiser, Sub, Pipe };
    public moveType myMoveType;
    public enum moveType {Foot, Mech, Treads, Wheels, Lander, Ship, Air};

    //Animationstuff
    Vector3 target;
    List<Vector3> wayPointList = new List<Vector3>();
    private int wayPointIndex = 1;

    private float movementSpeed = 3;
    private float rotationSpeed = 7;

    private Quaternion startRotation;
    private Quaternion endRotation;
    private Vector3 lookingDirection;

    private bool move = false;
    private bool rotate = false;
    private bool isMoving = false;

    // Use this for initialization
    void Awake () 
	{
		myLevelManager = GameObject.FindGameObjectWithTag ("LevelManager");
        graphMatrix = myLevelManager.GetComponent<MapCreator>().getGraph();
        graph = myLevelManager.GetComponent<MapCreator>();
        healthText = this.transform.Find("Lifepoints").GetComponent<TextMesh>();
    }

    public void Update()
    {
        //Rotate the unit towards a point the move it to this point, then get the next point and so on.
        if (isMoving)
        {
            if (move)
            {
                //Debug.Log("Moving!");
                float step = movementSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(this.transform.position, target, step);
            }

            if (rotate)
            {
                //Debug.Log("Rotating!");
                startRotation = this.transform.rotation;
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, rotationSpeed * Time.deltaTime);//Do the rotation with a lerp.
            }

            if (rotationComplete() && rotate)
            {
                rotate = false;
                move = true;
                //Stop rotation sound.
                //Play move sound.
            }

            if (wayPointReached(wayPointList[wayPointIndex]) && move)
            {
                //Debug.Log("Reached waypoint: " + wayPointIndex);
                wayPointIndex++;
                if (wayPointIndex >= wayPointList.Count)//Start from the beginning again.
                {
                    isMoving = false;                   
                    wayPointIndex = 1;
                    displayHealth(true);
                    setFacingDirection(this.transform.rotation.eulerAngles.y);
                    choseAMenuType();
                    if (isInterrupted)
                    {
                        wait();
                        myLevelManager.GetComponent<ContextMenu>().showExclamationMark(xPos, yPos); //Show exclamation mark.
                        //Stop move sound.
                        //Play interruption sound
                    }
                    Debug.Log("Reached last way point!");                    
                }
                target = wayPointList[wayPointIndex];
                lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
                endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target.  
                //Stop move sound.
                //Play rotation sound.
                move = false;
                rotate = true;
            }
            //Debug.DrawRay(this.transform.position, this.transform.forward * 2);
            //Debug.DrawLine(this.transform.position, target, Color.green);
        }
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
            //Debug.Log("Reachable iterations: " + counter);
            myLevelManager.GetComponent<MapCreator>().createReachableTiles();
            //Calculate attackable area, instantiate the graphics for the tiles and store the attackable units in a list.
            findAttackableTiles();
            myLevelManager.GetComponent<MapCreator>().createAttackableTiles();
            myLevelManager.GetComponent<MapCreator>().showAttackableTiles(false);
            findAttackableEnemies();
            
            calcVisibleArea();
            myLevelManager.GetComponent<MainFunctions>().activateMoveMode();
            
        }
        else
        //If move mode is activated
        if (myLevelManager.GetComponent<MainFunctions>().moveMode)
        {
            if (isSelected)
            {
                //Decide if the menu with firebutton and wait button is opened ...
                if (myLevelManager.GetComponent<MainFunctions>().selectedUnit.attackableUnits.Count > 0)
                {
                    //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                    if (graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
                    {
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 3);
                    }
                    else
                    {
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
                    }
                }
                //...OR if only the wait button is to display.
                else
                {
                    //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
                    if (graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
                    {
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 2);
                    }
                    else
                    {
                        myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 0);
                    }
                }               
            }
        }
        else
        //If fire mode is activated.
        if(myLevelManager.GetComponent<MainFunctions>().fireMode)
        {
            //Select unit, that is attackable and pass it to the fight function.
            if(graphMatrix[xPos][yPos].GetComponent<Tile>().isAttackable)
            {
                Unit attacker = myLevelManager.GetComponent<MainFunctions>().selectedUnit;
                Unit defender = this;
                //Align the units to face each other.
                alignUnit(attacker.xPos, attacker.yPos);
                attacker.alignUnit(this.xPos, this.yPos);
                //Puts the health indicator in the right position.
                displayHealth(true);
                attacker.displayHealth(true);

                myLevelManager.GetComponent<BattleMode>().fight(attacker, defender);//Battle
                myLevelManager.GetComponent<MainFunctions>().deselectObject();//Deselect the current unit                
            }
        }
    }

    //Ends the turn for the unit.
    public void wait()
    {
        canFire = false;
        hasTurn = false;
        myLevelManager.GetComponent<TurnManager>().setFogOfWar(myTeam);
        myLevelManager.GetComponent<MainFunctions>().deselectObject();
    }

    public void subtractHealth(int healthToSubtract)
    {
        health = health - healthToSubtract;
        displayHealth(true);
        if(health <= 0)
        {
            killUnit();
        }
    }

    //Displays the actual lifepoints in the "3D"TextMesh
    public void displayHealth(bool value)
    {
        if(value)
        {
            healthText.gameObject.SetActive(true);
            //Reposition the health text so it is always at the lower left corner of the tile.
            healthText.transform.SetPositionAndRotation(new Vector3(this.transform.position.x - 0.4f,this.transform.position.y, this.transform.position.z - 0.2f), Quaternion.Euler(90, 0, 0));
            //If the health goes below five the value will be rounded to 0, but we dont want that!
            if(health > 10)
            {
                healthText.text = "" + (int)(health / 10);
            }
            else
            {
                healthText.text = "1";
            }
            if(health < 0)
            {
                healthText.text = "0";
            }
        }
        else
        {
            healthText.gameObject.SetActive(false);
        }
    }

    //Destroys the unit
    public void killUnit()
    {
        //Set the unit standing on this tile as null.
        graphMatrix[xPos][yPos].GetComponent<Tile>().clearUnitHere();
        //Boom
        myLevelManager.GetComponent<AnimController>().boom(xPos,yPos);
        //Remove unit from team list
        myTeam.myUnits.Remove(this.transform);
        //If this was the last unit of the player the game is lost.
        if(myTeam.myUnits.Count <= 0)
        {
            myLevelManager.GetComponent<LevelLoader>().loadGameFinishedScreenWithDelay();//This has a short delay, so the player sees how the last unit is destroyed.
        }
        //Finally delete the unit.
        Destroy(this.gameObject);
    }

    //Move the unit to a field and align it so it faces away, from where it came.
    public void moveUnitTo(int newX, int newY)
    {
        //Only if we drew at least one arrow Path, we should be able move.
        if (myLevelManager.GetComponent<ArrowBuilder>().getArrowPath().Count > 1)
        {
            //If a possible path was found, go to the desired position.
            myLevelManager.GetComponent<ArrowBuilder>().checkForInterruption();//Check for interruption with enemies and make the tile before the interruption the last of the arrow path!
            wayPointList = myLevelManager.GetComponent<ArrowBuilder>().createMovementPath();
            target = wayPointList[wayPointIndex];//Set the first target for the movement.
            lookingDirection = (wayPointList[wayPointIndex] - transform.position).normalized;//Vector from our position to the target
            startRotation = Quaternion.LookRotation(this.transform.position);
            endRotation = Quaternion.LookRotation(lookingDirection);//The actual rotation we need to look at the target
            isMoving = true; //Init the sequencer in the update function...
            rotate = true;//...and start rotating towards the first waypoint.

            //Reset the unitStandingHere property of the old tile to null
            graphMatrix[xPos][yPos].GetComponent<Tile>().clearUnitHere();
            //Reset the take over counter
            graphMatrix[xPos][yPos].GetComponent<Tile>().resetTakeOverCounter();
            //Remember the last position and rotation of the unit. (For resetting purposes.)
            preDirection = myFacingDirection;
            prePosX = this.xPos;
            prePosY = this.yPos;
            //Set xPos and yPos to the new position.
            this.xPos = (int)(wayPointList[wayPointList.Count - 1].x);
            this.yPos = (int)(wayPointList[wayPointList.Count - 1].z);
            graphMatrix[xPos][yPos].GetComponent<Tile>().setUnitHere(this.transform);//Inform the new tile, that a unit is standing on it.
            myLevelManager.GetComponent<StatusWindow>().updateCover(xPos, yPos);//When you move the unit, you should see the new cover for the tile it will stand on.
            displayHealth(false);//While moving we dont want to see the health.
            hasMoved = true;
        }
    }

    //Resets the position and rotation of the unit to where it was before. (If we click the right mouse button or close the menu after we moved it somewhere.)
    public void resetPosition()
    {
        graphMatrix[xPos][yPos].GetComponent<Tile>().clearUnitHere();//Reset the unitStandingHere property of the tile, we went to, to null
        //Set the position and rotation of the unit to where it was before
        this.transform.position = new Vector3(prePosX, 0, prePosY);
        this.xPos = prePosX;
        this.yPos = prePosY;
        rotateUnit(preDirection);
        wayPointIndex = 1;
        myLevelManager.GetComponent<StatusWindow>().updateCover(xPos, yPos);//When the unit moves back, the display of the cover should be set to the old tile.
        graphMatrix[prePosX][prePosX].GetComponent<Tile>().setUnitHere(this.transform);//Inform the old tile, that we are back.
        displayHealth(true);//Repostition the health indicator.       
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
                rotateUnit(Unit.facingDirection.East);
            }
            //Face left
            if(this.xPos > targetX && this.yPos == targetY)
            {            
                rotateUnit(Unit.facingDirection.West);
            }
            //Face up
            if(this.yPos < targetY && this.xPos == targetX)
            {
                rotateUnit(Unit.facingDirection.North);
            }
            //Face down
            if (this.yPos > targetY && this.xPos == targetX)
            {            
                rotateUnit(Unit.facingDirection.South);
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
                rotateUnit(Unit.facingDirection.East);
            }
            //Face left
            if (this.xPos > targetX && this.yPos == targetY)
            {
                rotateUnit(Unit.facingDirection.West);
            }
            //Face up
            if (this.yPos < targetY && this.xPos == targetX)
            {
                rotateUnit(Unit.facingDirection.North);
            }
            //Face down
            if (this.yPos > targetY && this.xPos == targetX)
            {
                rotateUnit(Unit.facingDirection.South);
            }
        }

    }

    //Rotate the unit so it faces north, east, south or west.
    public void rotateUnit(Unit.facingDirection newDirection)
    {
        //this.transform.rotation = Quaternion.Euler(0, angle, 0);
        //this.rotation = 45;
        preDirection = myFacingDirection;
        myFacingDirection = newDirection;
        switch (newDirection)
        {
            case facingDirection.North: this.transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case facingDirection.East: this.transform.rotation = Quaternion.Euler(0, 90, 0); break;
            case facingDirection.South: this.transform.rotation = Quaternion.Euler(0, 180, 0); break;
            case facingDirection.West: this.transform.rotation = Quaternion.Euler(0, 270, 0); break;
            default:
                break;
        }
        displayHealth(true);
    }

    //Sets the facing direction depending on the transforms rotation.
    public void setFacingDirection(float angle)
    {
        if(-1 < angle && angle < 1)
        {
            myFacingDirection = facingDirection.North;
        }
        if (89 < angle && angle < 91)
        {
            myFacingDirection = facingDirection.East;
        }
        if (179 < angle && angle < 181)
        {
            myFacingDirection = facingDirection.South;
        }
        if (269 < angle && angle < 271)
        {
            myFacingDirection = facingDirection.West;
        }
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
                    graphMatrix[xPos - 2 - i][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos - 2 - i][yPos].GetComponent<Tile>());
                }
                //Down
                if (xPos - 1 - i > -1 && yPos - 1 > -1)
                {
                    graphMatrix[xPos - 1 - i][yPos - 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos - 1 - i][yPos - 1].GetComponent<Tile>());
                }
                //Up
                if (xPos - 1 - i > -1 && yPos + 1 < graphMatrix[0].Count)
                {
                    graphMatrix[xPos - 1 - i][yPos + 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos - 1 - i][yPos + 1].GetComponent<Tile>());
                }
            }
            //Right
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (xPos + 2 + i < graphMatrix.Count)
                {
                    graphMatrix[xPos + 2 + i][yPos].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos + 2 + i][yPos].GetComponent<Tile>());
                }
                //Down
                if (xPos + 1 + i < graphMatrix.Count && yPos - 1 > -1)
                {
                    graphMatrix[xPos + 1 + i][yPos - 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos + 1 + i][yPos - 1].GetComponent<Tile>());
                }
                //Up
                if (xPos + 1 + i < graphMatrix.Count && yPos + 1 < graphMatrix[0].Count)
                {
                    graphMatrix[xPos + 1 + i][yPos + 1].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos + 1 + i][yPos + 1].GetComponent<Tile>());
                }
            }
            //Top
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (yPos + 2 + i < graphMatrix[0].Count)
                {
                    graphMatrix[xPos][yPos + 2 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos][yPos + 2 + i].GetComponent<Tile>());
                }
                //Left
                if (yPos + 1 + i < graphMatrix[0].Count && xPos - 1 > -1)
                {
                    graphMatrix[xPos - 1][yPos + 1 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos - 1][yPos + 1 + i].GetComponent<Tile>());
                }
                //Right
                if (yPos + 1 + i < graphMatrix[0].Count && xPos + 1 < graphMatrix.Count)
                {
                    graphMatrix[xPos + 1][yPos + 1 + i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos + 1][yPos + 1 + i].GetComponent<Tile>());
                }
            }
            //Bottom
            for (int i = 1; i < 4; i++)
            {
                //Straight
                if (yPos - 2 - i > -1)
                {
                    graphMatrix[xPos][yPos - 2 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos][yPos - 2 - i].GetComponent<Tile>());
                }
                //Left
                if (yPos - 1 - i > -1 && xPos - 1 > 0)
                {
                    graphMatrix[xPos - 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos - 1][yPos - 1 - i].GetComponent<Tile>());
                }
                //Right
                if (yPos - 1 - i > -1 && xPos + 1 < graphMatrix.Count)
                {
                    graphMatrix[xPos + 1][yPos - 1 - i].GetComponent<Tile>().isAttackable = true;
                    attackableTiles.Add(graphMatrix[xPos + 1][yPos - 1 - i].GetComponent<Tile>());
                }
            }

            //Corners
            //Lower left
            if (xPos - 2 > -1 && yPos - 2 > -1)
            {
                graphMatrix[xPos - 2][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 2][yPos - 2].GetComponent<Tile>());
            }
            if (xPos - 3 > -1 && yPos - 2 > -1)
            {
                graphMatrix[xPos - 3][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 3][yPos - 2].GetComponent<Tile>());
            }
            if (xPos - 2 > -1 && yPos - 3 > -1)
            {
                graphMatrix[xPos - 2][yPos - 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 2][yPos - 3].GetComponent<Tile>());
            }

            //Upper left
            if (xPos - 2 > -1 && yPos + 2 < graphMatrix[0].Count)
            {
                graphMatrix[xPos - 2][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 2][yPos + 2].GetComponent<Tile>());
            }
            if (xPos - 3 > -1 && yPos + 2 < graphMatrix[0].Count)
            {
                graphMatrix[xPos - 3][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 3][yPos + 2].GetComponent<Tile>());
            }
            if (xPos - 2 > -1 && yPos + 3 < graphMatrix[0].Count)
            {
                graphMatrix[xPos - 2][yPos + 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos - 2][yPos + 3].GetComponent<Tile>());
            }
            //Upper right
            if (xPos + 2 < graphMatrix.Count && yPos + 2 < graphMatrix[0].Count)
            {
                graphMatrix[xPos + 2][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 2][yPos + 2].GetComponent<Tile>());
            }
            if (xPos + 3 < graphMatrix.Count && yPos + 2 < graphMatrix[0].Count)
            {
                graphMatrix[xPos + 3][yPos + 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 3][yPos + 2].GetComponent<Tile>());
            }
            if (xPos + 2 < graphMatrix.Count && yPos + 3 < graphMatrix[0].Count)
            {
                graphMatrix[xPos + 2][yPos + 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 2][yPos + 3].GetComponent<Tile>());
            }
            //Lower right
            if (xPos + 2 < graphMatrix.Count && yPos - 2 > -1)
            {
                graphMatrix[xPos + 2][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 2][yPos - 2].GetComponent<Tile>());
            }
            if (xPos + 3 < graphMatrix.Count && yPos - 2 > -1)
            {
                graphMatrix[xPos + 3][yPos - 2].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 3][yPos - 2].GetComponent<Tile>());
            }
            if (xPos + 2 < graphMatrix.Count && yPos - 3 > -1)
            {
                graphMatrix[xPos + 2][yPos - 3].GetComponent<Tile>().isAttackable = true;
                attackableTiles.Add(graphMatrix[xPos + 2][yPos - 3].GetComponent<Tile>());
            }

        }
    }

    //Calculates the attackable tiles for direct attack units.
    public void findAttackableTileForDirectAttack()
    {
        //Left
        if (xPos > 0)
        {
            graphMatrix[xPos - 1][yPos].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(graphMatrix[xPos - 1][yPos].GetComponent<Tile>());
        }
        //Right
        if (xPos < graphMatrix.Count - 1)
        {
            graphMatrix[xPos + 1][yPos].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(graphMatrix[xPos + 1][yPos].GetComponent<Tile>());
        }
        //Top
        if (yPos > 0)
        {
            graphMatrix[xPos][yPos - 1].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(graphMatrix[xPos][yPos - 1].GetComponent<Tile>());
        }
        //Bottom
        if (yPos < graphMatrix[0].Count - 1)
        {
            graphMatrix[xPos][yPos + 1].GetComponent<Tile>().isAttackable = true;
            attackableTiles.Add(graphMatrix[xPos][yPos + 1].GetComponent<Tile>());
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
                if(isEnemyHere(attackableTiles[i]))
                //if((this.teamBlue && attackableTiles[i].unitStandingHere.teamRed) || (this.teamRed && attackableTiles[i].unitStandingHere.teamBlue))
                {
                    attackableUnits.Add(attackableTiles[i].unitStandingHere.GetComponent<Unit>());
                }
            }
        }
    }

    //Checks if an enemy is standing on this tile.
    public bool isEnemyHere(Tile tile)
    {
        if(tile.unitStandingHere != null && tile.getVisibility())
        {
            Unit possibleEnemy = tile.unitStandingHere.GetComponent<Unit>();            
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
        Tile tile = graphMatrix[x][y].gameObject.GetComponent<Tile>();

        movementPoints = movementPoints - tile.getMovementCost(myMoveType);

        //If enough movement points are left and the tile is passable (we can move through our own units, but are blocked by enemies), do the recursion.
        if ((movementPoints >= 0) && (tile.getMovementCost(myMoveType) > 0) && !isEnemyHere(tile))
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
    
    //Calculates the visible area of this unit depending on its vision range and marks the visible tiles in the graph.
    public void calcVisibleArea()
    {        
        if(myLevelManager.GetComponent<MasterClass>().container.fogOfWar)
        {
            graph.getTile(xPos, yPos).setVisible(true);//Mark own position as visible.
            for (int i = 1; i <= visionRange; i++)
            {
                //Left
                int xTest = this.xPos - i;
                int yTest = this.yPos;
                if (xTest >= 0)
                {
                    if ((i < 2) || (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest))
                    {
                        graph.getTile(xTest, yTest).setVisible(true);
                    }                
                    for(int j = 1; j <= visionRange - i; j++)
                    {
                        //...and up.
                        yTest = this.yPos + j;
                        if(yTest < graph.getGraph()[0].Count)
                        {
                            if (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest)
                            {
                                graph.getTile(xTest, yTest).setVisible(true);
                            }
                        }
                        //...and down.
                        yTest = this.yPos - j;
                        if (yTest >= 0)
                        {
                            if (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest)
                            {
                                graph.getTile(xTest, yTest).setVisible(true);
                            }
                        }
                    }                
                }
                //Right
                xTest = this.xPos + i;
                yTest = this.yPos;
                if (xTest < graph.getGraph().Count)
                {
                    if ((i < 2) || (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest))
                    {
                        graph.getTile(xTest, yTest).setVisible(true);
                    }
                    for (int j = 1; j <= visionRange - i; j++)
                    {
                        //...and up.
                        yTest = this.yPos + j;
                        if (yTest < graph.getGraph()[0].Count)
                        {
                            if (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest)
                            {
                                graph.getTile(xTest, yTest).setVisible(true);
                            }
                        }
                        //...and down.
                        yTest = this.yPos - j;
                        if (yTest >= 0)
                        {
                            if (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest)
                            {
                                graph.getTile(xTest, yTest).setVisible(true);
                            }
                        }
                    }
                }
            }
        }
        for(int i = 1; i <= visionRange; i++)
        {
            //Up
            int xTest = this.xPos;
            int yTest = this.yPos + i;
            if(yTest < graph.getGraph()[0].Count)
            {
                if ((i < 2) || (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest))
                {
                    graph.getTile(xTest, yTest).setVisible(true);
                }
            }
            //Down
            yTest = this.yPos - i;
            if (yTest >= 0)
            {
                if ((i < 2) || (graph.getTile(xTest, yTest).myTileType != Tile.type.Forest))
                {
                    graph.getTile(xTest, yTest).setVisible(true);
                }
            }
        }
    }

    //Returns the actual health of the unit downsized to ten and as int.
    public int getHealthAsInt()
    {
        Debug.Log(Mathf.RoundToInt(health / 10));
        return Mathf.RoundToInt(health/10);
    }

    private bool wayPointReached(Vector3 nextWaypoint)
    {
        if (nextWaypoint == this.transform.position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //If the forward vector of the unit aligns with the vector from the unit to the target, we finished the rotation.
    private bool rotationComplete()
    {
        if ( Vector3.Angle(this.transform.forward, lookingDirection) < 1 )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Chose a menu depending on the below given factors.
    public void choseAMenuType()
    {
        //Decide if the menu with firebutton and wait button is opened ...    
        if (attackableUnits.Count > 0)
        {
            //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
            if (graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
            {
                myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 3);
            }
            else
            {
                myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 1);
            }
        }
        //...OR if only the wait button is to display.
        else
        {
            //If the selected unit is infantry/mech and this tile is a neutral/enemy property also load the 'occupy button'.
            if (graphMatrix[xPos][yPos].GetComponent<Tile>().isOccupyable(this))
            {
                myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 2);
            }
            else
            {
                myLevelManager.GetComponent<ContextMenu>().openContextMenu(xPos, yPos, 0);
            }
        }
    }

    //Set/get if the unit is moving.
    public void setIsMoving(bool value)
    {
        isMoving = value;
    }
    public bool getIsMoving()
    {
        return isMoving;
    }
    //Set/get if the unit is interrupted.
    public void setIsInterrupted(bool value)
    {
        isInterrupted = value;        
    }
    public bool getIsInterrupted()
    {
        return isInterrupted;
    }

    public void setCanFire(bool value)
    {
        canFire = value;
    }
    public bool getCanFire()
    {
        return canFire;
    }
}
