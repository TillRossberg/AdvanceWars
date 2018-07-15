using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBuilder : MonoBehaviour
{
    //Graphics
    public Transform arrow;
    public Transform straight;
    public Transform curve;
    private int maxMovementPoints = 0; //Maximum MovementPoints or the maximum length of the arrow
    private int momMovementPoints = 0; //Momentary MovementPoints.
    
    private List<ArrowPart> arrowPath = new List<ArrowPart>();//The path of the movement arrow.
    private List<Vector3> movementPath = new List<Vector3>();//This path is calculated from the arrowPath and provides coordinates used for animating the units movement.

    //Initiate the arrowBuilder with the start point of the path.
    public void init(Tile myTile, int moveDist)
    {
        myTile.isPartOfArrowPath = true;
        ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
        myArrowPart.init("First node", null, myTile);
        this.GetComponent<ArrowBuilder>().arrowPath.Add(myArrowPart);//Set this tile as startpoint of the arrowPath
        this.GetComponent<ArrowBuilder>().momMovementPoints = moveDist;//Handover the maximum movement points of the unit.
        this.GetComponent<ArrowBuilder>().maxMovementPoints = moveDist;//Set a maximum for the movement points (for resetting purposes).
    }

    //Tries to draw an arrow on the tile if it is reachable.
    public void createArrowPath(Tile tile)
    {           
        //Only when the tile is adjacent (it is a neighbor) draw an arrow.
        Tile predecessor = arrowPath[arrowPath.Count - 1].getTile();
        for (int i = 0; i < tile.neighbors.Count; i++)
        {
            Tile neighborProperties = tile.neighbors[i].GetComponent<Tile>();
            if (neighborProperties == predecessor && momMovementPoints > 0 && !tile.isPartOfArrowPath)
            {
                momMovementPoints--;//Are decreased, so the arrow doesn't get too long.
                //Draw the head of the arrow (predecessor is responsible for the alignment)
                ArrowPart arrow = calcArrowDirection(tile, predecessor);
                arrowPath.Add(arrow);
                
                //If one moves further than one field away from the startfield, the predecessor arrow should be replaced by a curve or a straight part depending on the arrow heads prepredecessor.
                if (arrowPath.Count > 2)
                {                    
                    predecessor = arrowPath[arrowPath.Count - 2].getTile();
                    Tile prePredecessor = arrowPath[arrowPath.Count - 3].getTile();
                    drawArrowParts(tile, predecessor, prePredecessor);
                }
            }
        }
    }

    //Calculate the alignment of the arrow by its predecessor and return an arrowPart.
    private ArrowPart calcArrowDirection(Tile myTileProperties, Tile predecessor)
    {
        //Top
        if ((myTileProperties.xPos == predecessor.xPos) && (myTileProperties.yPos > predecessor.yPos))
        {
            myTileProperties.isPartOfArrowPath = true;
            Transform arrowPart = Instantiate(arrow, new Vector3(myTileProperties.xPos, 0.3f, myTileProperties.yPos), Quaternion.Euler(0, 180, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "arrowHead" + (arrowPath.Count - 1);
            ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
            myArrowPart.init(arrowPart.name, arrowPart, myTileProperties);
            return myArrowPart;
            
        }
        else
        //Bottom
        if ((myTileProperties.xPos == predecessor.xPos) && (myTileProperties.yPos < predecessor.yPos))
        {
            myTileProperties.isPartOfArrowPath = true;
            Transform arrowPart = Instantiate(arrow, new Vector3(myTileProperties.xPos, 0.3f, myTileProperties.yPos), Quaternion.Euler(0, 0, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "arrowHead" + (arrowPath.Count - 1);
            ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
            myArrowPart.init(arrowPart.name, arrowPart, myTileProperties);
            return myArrowPart;
        }
        else
        //Left
        if ((myTileProperties.yPos == predecessor.yPos) && (myTileProperties.xPos < predecessor.xPos))
        {
            myTileProperties.isPartOfArrowPath = true;
            Transform arrowPart = Instantiate(arrow, new Vector3(myTileProperties.xPos, 0.3f, myTileProperties.yPos), Quaternion.Euler(0, 90, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "arrowHead" + (arrowPath.Count - 1);
            ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
            myArrowPart.init(arrowPart.name, arrowPart, myTileProperties);
            return myArrowPart;
        }
        else
        //Right
        if ((myTileProperties.yPos == predecessor.yPos) && (myTileProperties.xPos > predecessor.xPos))
        {
            myTileProperties.isPartOfArrowPath = true;
            Transform arrowPart = Instantiate(arrow, new Vector3(myTileProperties.xPos, 0.3f, myTileProperties.yPos), Quaternion.Euler(0, 270, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "arrowHead" + (arrowPath.Count - 1);
            ArrowPart myArrowPart = ScriptableObject.CreateInstance("ArrowPart") as ArrowPart;
            myArrowPart.init(arrowPart.name, arrowPart, myTileProperties);
            return myArrowPart;
        }
        else
        {
            Debug.Log("Couldnt draw arrow!");
            return null;
        }
    }
    
    //Calculates the alignment of the curve/straight part by the actual tile, its predecessor and its prepredecessor.
    private void drawArrowParts(Tile tile, Tile predecessor, Tile prePredecessor)
    {
        //Vertical...
        if (tile.xPos == predecessor.xPos)
        {
            //Top/bottom in a straight line.
            if (tile.xPos == prePredecessor.xPos)
            {
                drawArrowPart("straight", predecessor.xPos, predecessor.yPos, 0);
            }
            else
            //...with one step up...
            if (tile.yPos > predecessor.yPos)
            {
                //...and the prepredecessor on the left.
                if (tile.xPos > prePredecessor.xPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 90);
                }
                else
                //...and the prepredecessor on the right.
                if (tile.xPos < prePredecessor.xPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 180);
                }
            }
            else
            //...with one step up...
            if (tile.yPos < predecessor.yPos)
            {
                //...and the prepredecessor on the left.
                if (tile.xPos > prePredecessor.xPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 0);
                }
                else
                //...and the prepredecessor on the right.
                if (tile.xPos < prePredecessor.xPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 270);
                }
            }
        }
        else
        //Horizontal
        if (tile.yPos == predecessor.yPos)
        {
            //Left/Right
            if (tile.yPos == prePredecessor.yPos)
            {
                drawArrowPart("straight", predecessor.xPos, predecessor.yPos, 90);
            }
            else
            //...with one step to the right...
            if (tile.xPos > predecessor.xPos)
            {
                //...and the prepredecessor upwards.
                if (tile.yPos < prePredecessor.yPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 180);
                }
                else
                //...and the prepredecessor downwards.
                if (tile.yPos > prePredecessor.yPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 270);
                }
            }
            else
            //...with one step to the left...
            if (tile.xPos < predecessor.xPos)
            {
                //...and the prepredecessor upwards.
                if (tile.yPos < prePredecessor.yPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 90);
                }
                else
                //...and the prepredecessor downwards.
                if (tile.yPos > prePredecessor.yPos)
                {
                    drawArrowPart("curve", predecessor.xPos, predecessor.yPos, 0);
                }
            }
        }    
    }
    
    //Draw a curve or a straight part of the arrowpart.
    private void drawArrowPart(string partName, int x, int y, int angle)
    {        
        if (partName == "straight")
        {
            Transform arrowPart = Instantiate(straight, new Vector3(x, 0.3f, y), Quaternion.Euler(0, angle, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "straight" + (arrowPath.Count - 2);

            arrowPath[arrowPath.Count - 2].arrowPartName = arrowPart.name;
            arrowPath[arrowPath.Count - 2].replaceArrowGraphic(arrowPart);
        }
        if (partName == "curve")
        {
            Transform arrowPart = Instantiate(curve, new Vector3(x, 0.3f, y), Quaternion.Euler(0, angle, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "curve" + (arrowPath.Count - 2);

            arrowPath[arrowPath.Count - 2].arrowPartName = arrowPart.name;
            arrowPath[arrowPath.Count - 2].replaceArrowGraphic(arrowPart);
        }       
    }

    
    //Clear the arrow path, inform the tiles that they are no longer part of the arrow path and reset the movement points.
    //Also we set the first entry of the arrow path to be the position of the unit, so we can start building a new path from there.
    public void resetArrowPath()
    {
        momMovementPoints = maxMovementPoints;
        ArrowPart startTile = arrowPath[0];
        for(int i = 1; i < arrowPath.Count; i++)
        {
            Destroy(arrowPath[i].myArrowPart.gameObject);
            arrowPath[i].getTile().isPartOfArrowPath = false;
            arrowPath[i].setTile(null);
        }
        arrowPath.Clear();
        arrowPath.Add(startTile);
    }

    public void resetAll()
    {        
        momMovementPoints = 0;
        maxMovementPoints = 0;
        if(arrowPath.Count > 0)
        {
            for (int i = 0; i < arrowPath.Count; i++)
            {                
                if(arrowPath[i].myArrowPart != null)
                {
                    Destroy(arrowPath[i].myArrowPart.gameObject);
                }
                arrowPath[i].getTile().isPartOfArrowPath = false;
            }
            arrowPath.Clear();
        }        
    }

    
    
    //If you hover with the mouse over the predecessor of the arrow, the arrow should become smaller.
    public void tryToGoBack(Tile myTileproperties)
    {       
        if(arrowPath.Count > 1)//Dont't touch the first entry, because that's the tile where the unit stands on.
        {
            if(arrowPath[arrowPath.Count - 2].getTile() == myTileproperties)//If you hover over the predecessor tile, start deleting.
            {                           
                momMovementPoints++;//If you go back, you have more movement points available.
                //Delete last entry of the arrow path, because that is the arrowhead.
                arrowPath[arrowPath.Count - 1].getTile().isPartOfArrowPath = false;//Is no longe part of the arrow path.
                Destroy(arrowPath[arrowPath.Count - 1].myArrowPart.gameObject);//Delete Graphic.
                arrowPath.RemoveAt(arrowPath.Count - 1);//Finally delete it from the list.

                //Only if you are at least one tile away from the unit, a curve/straight part is visible, only then replace this part with the new arrowhead.
                if(arrowPath.Count >= 2)
                {
                    //Replace predecessor entry (always is a straight/curve part, can never be an arrowhead!) with an arrowhead.
                    Tile predecessor = arrowPath[arrowPath.Count - 2].getTile();
                    ArrowPart newArrowHead = calcArrowDirection(myTileproperties, predecessor);
                    arrowPath[arrowPath.Count - 1].replaceArrowGraphic(newArrowHead.myArrowPart);               
                }
            }            
        }     
    }
        
    //Calculates a direct path from the arrow path. I.e.: combine arrow parts that are in a straight line to be just one checkpoint for the movement.
    public List<Vector3> createMovementPath()
    {
        movementPath.Clear();
        movementPath.Add(new Vector3(arrowPath[0].getTile().xPos, 0, arrowPath[0].getTile().yPos));//Set the starting position of the movement path as the position we are on right now.
        int wayPointX = -1;
        int wayPointY = -1;
        //TODO: implement the height for mountains and rivers!

        for(int i = 0; i < arrowPath.Count; i++)
        {
            Tile currentTile = arrowPath[i].getTile();
            Tile nextTileToTest;            

            if (i+1 < arrowPath.Count)//Check if we reached the last tile in the list.
            {
                nextTileToTest = arrowPath[i + 1].getTile();
            }
            else
            {
                break;//If we reach the second to the last of the arrow path, we don't want to continue. (We add the last point of the path after the loop.)
            }

            //Check up/downwards
            if (currentTile.xPos == nextTileToTest.xPos)
            {
                wayPointX = currentTile.xPos;//Fix the x position.
                for(int j = i; j < arrowPath.Count; j++)
                {
                    if(currentTile.xPos == nextTileToTest.xPos)
                    {
                        if (j + 1 < arrowPath.Count)//Check if we reached the last tile in the list...
                        {
                            nextTileToTest = arrowPath[j + 1].getTile();//...if not we want to test its alignment.
                        }
                    }
                    else
                    {
                        wayPointY = nextTileToTest.yPos;
                        movementPath.Add(new Vector3(wayPointX, 0, wayPointY));//X position of the current and next tile are not equal, so we add this "corner" to the list.
                        i = j - 2;//The magic! Difficult to explain in one comment.
                        break;
                    }
                }
            }
            else
            //Check left/right
            if (currentTile.yPos == nextTileToTest.yPos)
            {
                wayPointY = currentTile.yPos;
                for (int j = i; j < arrowPath.Count; j++)
                {
                    if (currentTile.yPos == nextTileToTest.yPos)
                    {
                        if (j + 1 < arrowPath.Count)//Check if we reached the last tile in the list...
                        {
                            nextTileToTest = arrowPath[j + 1].getTile();//...if not we want to test its alignment.
                        }
                    }
                    else
                    {
                        wayPointX = nextTileToTest.xPos;
                        movementPath.Add(new Vector3(wayPointX, 0, wayPointY));//Y position of the current and next tile are not equal, so we add this "corner" to the list.
                        i = j - 2;//The magic! Difficult to explain in one comment.
                        break;
                    }
                }
            }
        }
        movementPath.Add(new Vector3(arrowPath[arrowPath.Count - 1].getTile().xPos, 0, arrowPath[arrowPath.Count - 1].getTile().yPos));//Endpoint
        return movementPath;
    }

    //Get the arrowPath
    public List<ArrowPart> getArrowPath()
    {
        return arrowPath;
    }

    //Tests if there is an enemy on the arrow path and shortens the path, so we are stopping directly before the enemy.
    public void checkForInterruption()
    {
        List<Team> enemyTeams = GetComponent<GameFunctions>().getSelectedUnit().myTeam.getEnemyTeams();
        for (int i = 0; i < arrowPath.Count; i++)
        {
            for (int j = 0; j < enemyTeams.Count; j++)
            {
                if(enemyTeams[j].myUnits.Contains(arrowPath[i].getTile().getUnitHere().transform))
                {
                    for (int k = i ; k < arrowPath.Count; k++)
                    {
                        if (arrowPath[k].myArrowPart != null)
                        {
                            Destroy(arrowPath[k].myArrowPart.gameObject);
                        }
                        arrowPath[k].getTile().isPartOfArrowPath = false;
                    }
                    arrowPath.RemoveRange(i, arrowPath.Count - i);
                    GetComponent<GameFunctions>().getSelectedUnit().setIsInterrupted(true);
                    GetComponent<GameFunctions>().getSelectedUnit().setCanFire(false);
                    break;
                }              
            }
        }
    }
}
