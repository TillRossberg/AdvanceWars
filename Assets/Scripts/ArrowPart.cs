using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPart : ScriptableObject
{
    //General
    public string arrowPartName;    
    public Transform myArrowPart;
    public Tile myTileProperties;
        
    //Replace the current graphic by a given one.
    public void replaceArrowGraphic(Transform newGraphic)
    {
        Destroy(myArrowPart.gameObject);
        this.myArrowPart = newGraphic;
    }
    //Nessecary since you can't instantiate ScriptableObjects with parameters.
    public void init(string name, Transform myArrowPart, Tile myTileProperties)
    {
        this.arrowPartName = name;
        this.myArrowPart = myArrowPart;
        this.myTileProperties = myTileProperties;
    }
	
}
