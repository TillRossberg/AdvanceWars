//created by Till Roßberg, 2017-18
//The container is supposed to store data if we load another scene.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public int nextLevelToLoad;
    
    //Level
    public void setNextLevel(int value)
    {
        nextLevelToLoad = value;
    }
    public int getNextLevelIndex()
    {
        return nextLevelToLoad;
    }

}
