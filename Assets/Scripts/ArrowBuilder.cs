using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBuilder : MonoBehaviour
{
    //Graphics
    public Transform arrow;
    public Transform straight;
    public Transform curve;

    public int maxMovementPoints = 0; //Maximum MovementPoints or the maximum length of the arrow
    public int momMovementPoints = 0; //Momentary MovementPoints.
    
    public List<ArrowPart> arrowPath = new List<ArrowPart>();//arrowParts hold information about the tile they are on and about the graphic element they should display.

    //Initiate the arrowBuilder.
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
        Tile predecessor = arrowPath[arrowPath.Count - 1].tile;
        for (int i = 0; i < tile.neighbors.Count; i++)
        {
            Tile neighborProperties = tile.neighbors[i].GetComponent<Tile>();
            if (neighborProperties == predecessor && momMovementPoints > 0 && !tile.isPartOfArrowPath)
            {
                momMovementPoints--;//Are decreased, so the arrow doesn't get too long.
                //Draw the head of the arrow (predecessor is responsible for the alignment)
                ArrowPart arrow = calcArrowDirection(tile, predecessor);
                arrowPath.Add(arrow);
                
                //If one moves further than one field away from the startfield, the predecessor arrow should be replace by a curve or a straight part.
                if (arrowPath.Count > 2)
                {                    
                    predecessor = arrowPath[arrowPath.Count - 2].tile;//Set a new predecessor, because we drew a
                    Tile prePredecessor = arrowPath[arrowPath.Count - 3].tile;

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
            arrowPath[i].tile.isPartOfArrowPath = false;
            arrowPath[i].tile = null;
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
                arrowPath[i].tile.isPartOfArrowPath = false;
            }
            arrowPath.Clear();
        }        
    }
    
    //If you hover with the mouse over the predecessor of the arrow, the arrow should become smaller.
    public void tryToGoBack(Tile myTileproperties)
    {       
        if(arrowPath.Count > 1)//Dont't touch the first entry, because that's the tile where the unit stands on.
        {
            if(arrowPath[arrowPath.Count - 2].tile == myTileproperties)//If you hover over the predecessor tile, start deleting.
            {                           
                momMovementPoints++;//If you go back, you have more movement points available.
                //Delete last entry of the arrow path, because that is the arrowhead.
                arrowPath[arrowPath.Count - 1].tile.isPartOfArrowPath = false;//Is no longe part of the arrow path.
                Destroy(arrowPath[arrowPath.Count - 1].myArrowPart.gameObject);//Delete Graphic.
                arrowPath.RemoveAt(arrowPath.Count - 1);//Finally delete it from the list.

                //Only if you are at least one tile away from the unit, a curve/straight part is visible, only then replace this part with the new arrowhead.
                if(arrowPath.Count >= 2)
                {
                    //Replace predecessor entry (always is a straight/curve part, can never be an arrowhead!) with an arrowhead.
                    Tile predecessor = arrowPath[arrowPath.Count - 2].tile;
                    ArrowPart newArrowHead = calcArrowDirection(myTileproperties, predecessor);
                    arrowPath[arrowPath.Count - 1].replaceArrowGraphic(newArrowHead.myArrowPart);               
                }
            }            
        }     
    }
        
}
