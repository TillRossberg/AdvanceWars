using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_MarkingCursor : MonoBehaviour
{
    //References
    public Manager _manager;
    public MapCreator _mapCreator;
    //Fields
    private int xGraph = 0;
    private int yGraph = 0;

    public void init(int x, int y)
    {
        _manager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Manager>();
        _mapCreator = _manager.getMapCreator();
        xGraph = x;
        yGraph = y;
        this.transform.position = new Vector3(x, 0, y);
    }

    private void Update()
    {
        if (Input.GetKeyUp("up"))
        {
            if (_mapCreator.isInsideGraph(xGraph, yGraph + 1))
            {
                goTo(xGraph, yGraph + 1);
            }
        }
        if (Input.GetKeyUp("down"))
        {
            if (_mapCreator.isInsideGraph(xGraph, yGraph - 1))
            {
                goTo(xGraph, yGraph - 1);
            }
        }
        if (Input.GetKeyUp("left"))
        {
            if (_mapCreator.isInsideGraph(xGraph - 1, yGraph))
            {
                goTo(xGraph - 1, yGraph);
            }
        }
        if (Input.GetKeyUp("right"))
        {
            if (_mapCreator.isInsideGraph(xGraph + 1, yGraph))
            {
                goTo(xGraph + 1, yGraph);
            }
        }
    }

    private void goTo(int x, int y)
    {
        this.transform.position = new Vector3(_mapCreator.getTile(x, y).transform.position.x, 0, _mapCreator.getTile(x, y).transform.position.z);
        xGraph = x;
        yGraph = y;
    }
    
}
