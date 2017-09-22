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

    //Tries to draw an arrow on the tile if it is reachable.
    public void createArrowPath(Tile myTileProperties)
    {           
        //Only when the tile is adjacent (it is a neighbor) draw an arrow.
        Tile predecessor = arrowPath[arrowPath.Count - 1].myTileProperties;
        for (int i = 0; i < myTileProperties.nachbarn.Count; i++)
        {
            Tile neighborProperties = myTileProperties.nachbarn[i].GetComponent<Tile>();
            if (neighborProperties == predecessor && momMovementPoints > 0 && !myTileProperties.isPartOfArrowPath)
            {
                momMovementPoints--;//Are decreased, so the arrow doesn't get too long.
                //Draw the head of the arrow (predecessor is responsible for the alignment)
                ArrowPart arrow = calcArrowDirection(myTileProperties, predecessor);
                arrowPath.Add(arrow);
                
                //If one moves further than one field away from the startfield, the predecessor arrow should be replace by a curve or a straight part.
                if (arrowPath.Count > 2)
                {                    
                    predecessor = arrowPath[arrowPath.Count - 2].myTileProperties;//Set a new predecessor, because we drew a
                    Tile prePredecessor = arrowPath[arrowPath.Count - 3].myTileProperties;

                    drawArrowParts(myTileProperties, predecessor, prePredecessor);
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
    
    //Calculates the alignment of the curve/straight partn by the actual tile, its predecessor and its prepredecessor.
    private void drawArrowParts(Tile myTileProperties, Tile predecessor, Tile prePredecessor)
    {
        //Vertical...
        if (myTileProperties.xPos == predecessor.xPos)
        {
            //Top/bottom in a straight line.
            if (myTileProperties.xPos == prePredecessor.xPos)
            {
                drawArrowPart("gerade", predecessor.xPos, predecessor.yPos, 0, myTileProperties);
            }
            //...with one step up...
            if (myTileProperties.yPos > predecessor.yPos)
            {
                //...and the prepredecessor on the left.
                if (myTileProperties.xPos > prePredecessor.xPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 90, myTileProperties);
                }
                //...and the prepredecessor on the right.
                if (myTileProperties.xPos < prePredecessor.xPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 180, myTileProperties);
                }
            }
            //...mit einem Schritt nach unten...
            if (myTileProperties.yPos < predecessor.yPos)
            {
                //...und dem Vorvorgänger links.
                if (myTileProperties.xPos > prePredecessor.xPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 0, myTileProperties);
                }
                //...und dem Vorvorgänger rechts.
                if (myTileProperties.xPos < prePredecessor.xPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 270, myTileProperties);
                }
            }
        }
        //Horizontal
        if (myTileProperties.yPos == predecessor.yPos)
        {
            //Links/Rechts
            if (myTileProperties.yPos == prePredecessor.yPos)
            {
                drawArrowPart("gerade", predecessor.xPos, predecessor.yPos, 90, myTileProperties);
            }
            //...mit einem Schritt nach rechts...
            if (myTileProperties.xPos > predecessor.xPos)
            {
                //...und dem Vorvorgänger oben.
                if (myTileProperties.yPos < prePredecessor.yPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 180, myTileProperties);
                }
                //...und dem Vorvorgänger unten.
                if (myTileProperties.yPos > prePredecessor.yPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 270, myTileProperties);
                }
            }
            //...mit einem Schritt nach links...
            if (myTileProperties.xPos < predecessor.xPos)
            {
                //...und dem Vorvorgänger oben.
                if (myTileProperties.yPos < prePredecessor.yPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 90, myTileProperties);
                }
                //...und dem Vorvorgänger unten.
                if (myTileProperties.yPos > prePredecessor.yPos)
                {
                    drawArrowPart("kurve", predecessor.xPos, predecessor.yPos, 0, myTileProperties);
                }
            }
        }    
    }
    
    private void drawArrowPart(string name, int x, int y, int angle, Tile myTileProperties)
    {        
        if (name == "gerade")
        {
            Transform arrowPart = Instantiate(straight, new Vector3(x, 0.3f, y), Quaternion.Euler(0, angle, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "gerade" + (arrowPath.Count - 2);

            arrowPath[arrowPath.Count - 2].arrowPartName = arrowPart.name;
            arrowPath[arrowPath.Count - 2].replaceArrowGraphic(arrowPart);
        }
        if (name == "kurve")
        {
            Transform arrowPart = Instantiate(curve, new Vector3(x, 0.3f, y), Quaternion.Euler(0, angle, 0), this.transform.Find("MovementArrow"));
            arrowPart.name = "kurve" + (arrowPath.Count - 2);

            arrowPath[arrowPath.Count - 2].arrowPartName = arrowPart.name;
            arrowPath[arrowPath.Count - 2].replaceArrowGraphic(arrowPart);
        }       
    }

   
    //Remove all parts of the arrowPath list and reset the movement points.
    public void resetArrowPath()
    {
        momMovementPoints = maxMovementPoints;
        ArrowPart startTile = arrowPath[0];
        //Teile des Pfeils zerstören, tileProperties null setzten (sonst werden die auch gelöscht) und nicht mehr als Teil des ArrowPath setzen.
        for(int i = 1; i < arrowPath.Count; i++)
        {
            Destroy(arrowPath[i].myArrowPart.gameObject);
            arrowPath[i].myTileProperties.isPartOfArrowPath = false;
            arrowPath[i].myTileProperties = null;
        }
        arrowPath.Clear();
        arrowPath.Add(startTile);
    }

    public void resetAll()
    {        
        momMovementPoints = 0;
        maxMovementPoints = 0;
        //Teile des Pfeils zerstören, tileProperties null setzten (sonst werden die auch gelöscht) und nicht mehr als Teil des ArrowPath setzen.
        if(arrowPath.Count > 0)
        {
            for (int i = 0; i < arrowPath.Count; i++)
            {                
                if(arrowPath[i].myArrowPart != null)
                {
                    Destroy(arrowPath[i].myArrowPart.gameObject);
                }
                arrowPath[i].myTileProperties.isPartOfArrowPath = false;
                arrowPath[i].myTileProperties = null;
            }
            arrowPath.Clear();
        }        
    }
    
    //If one hovers with the mouse over the predecessor of the arrow, the arrow should become smaller.
    public void tryToGoBack(Tile myTileproperties)
    {       
        if(arrowPath.Count > 1)//Der erste Eintrag soll unangetastet bleiben, denn das ist das Feld mit der Einheit drauf.
        {
            if(arrowPath[arrowPath.Count - 2].myTileProperties == myTileproperties)//Wenn man auf das vorletzte Feld geht, soll fröhlich gelöscht werden.
            {                
                momMovementPoints++;//Da man zurück geht, werden die Bewegungspunkte wieder erhöht.
                //Letzten Eintrag in arrowParts löschen (und damit den Pfeilkopf!).
                arrowPath[arrowPath.Count - 1].myTileProperties.isPartOfArrowPath = false;//Ist nicht mehr Teil des Arrowpath.
                Destroy(arrowPath[arrowPath.Count - 1].myArrowPart.gameObject);//Grafik löschen.
                arrowPath[arrowPath.Count - 1].myTileProperties = null;//Verhindert, dass die Tile gelöscht wird.
                arrowPath.RemoveAt(arrowPath.Count - 1);//Aus Liste löschen.

                //Nur wenn man ein Feld weit weg vom Startfeld ist, ist eine Kurve/Gerade zu sehen, nur dann soll diese durch den Pfeil ersetzt werden.
                if(arrowPath.Count >= 2)
                {
                    //Vorletzten Eintrag (is immer ne Kurve oder ne Gerade, kann niemals ein Pfeil sein!) durch eine Pfeilspitze ersetzen.
                    Tile vorgänger = arrowPath[arrowPath.Count - 2].myTileProperties;
                    ArrowPart newArrowHead = calcArrowDirection(myTileproperties, vorgänger);
                    arrowPath[arrowPath.Count - 1].replaceArrowGraphic(newArrowHead.myArrowPart);               

                }
            }            
        }     
    }
        
}
