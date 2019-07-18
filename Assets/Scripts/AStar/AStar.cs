﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public List<Tile> finalPath = new List<Tile>();

    public void CalcPath(UnitMoveType moveType, Tile startTile, Tile endTile)
    {
        int counter = 0;

        //Init
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();      
        openList.Add(startTile);
        //Calculate
        bool finished = false;
        while (!finished)
        {
            //Go through the open list
            if (openList.Count > 0)
            {
                int winner = 0;
                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].F < openList[winner].F) winner = i;
                }
                Tile currentTile = openList[winner];
                //end reached
                if (currentTile == endTile)
                {
                    List<Tile> pathFromTheEnd = ReconstructPath(currentTile, startTile);
                    finalPath = InvertOrder(pathFromTheEnd);
                    finished = true;
                }
                openList.RemoveAt(winner);
                closedList.Add(currentTile);
                //check all neighbors of current
                foreach (Tile neighbor in currentTile.Neighbors)
                {
                    if(neighbor.data.GetMovementCost(moveType) > 0)//Make sure impassable terrain is ignored
                    {
                        if (!closedList.Contains(neighbor))
                        {
                            float tempG = currentTile.G + neighbor.data.GetMovementCost(moveType);
                            if (openList.Contains(neighbor))
                            {
                                if (tempG < neighbor.G) neighbor.G = tempG;
                            }
                            else
                            {
                                neighbor.G = tempG;
                                openList.Add(neighbor);
                            }
                            //vielleicht fehler, sollte es nicht von neighbor zu end sein?
                            neighbor.H = Vector3.Distance(currentTile.transform.position, endTile.transform.position);
                            neighbor.F = neighbor.H + neighbor.G;
                            neighbor.PreviousTile = currentTile;
                        }
                    }
                }
            }
            else
            {
                finished = true;
            }
            counter++;
            if (counter > 1000)
            {
                finished = true;
                throw new System.Exception("A* execution took too long!");
            }
        }
    }
    float CalcDistance(Tile tile1, Tile tile2)
    {
        return Vector3.Distance(tile1.transform.position, tile2.transform.position);
    }   
    //Create a path of the previous nodes saved inside the nodes, this path starts at the end position.
    List<Tile> ReconstructPath(Tile currentTile, Tile startTile)
    {
        List<Tile> path = new List<Tile>();
        path.Add(currentTile);
        while (currentTile.PreviousTile != null)
        {
            path.Add(currentTile);
            currentTile = currentTile.PreviousTile;
        }
        path.Add(startTile);
        return path;
    }
    //Invert the order of this list.
    List<Tile> InvertOrder(List<Tile> tiles)
    {
        List<Tile> tempList = new List<Tile>();

        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            tempList.Add(tiles[i]);
        }
        return tempList;
    }
    //Create a list of tiles of the node list.
    
    public void Reset()
    {
        finalPath.Clear();
        for (int x = 0; x < Core.Model.MapMatrix.Count; x++)
        {
            for (int y = 0; y < Core.Model.MapMatrix[x].Count; y++)
            {
                Core.Model.MapMatrix[x][y].ResetAStar();
            }
        }
    }
}
