using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IOrder
{

    public void Execute()
    {
        
    }
    public void Exit()
    {
        
    }

    public void Init(AI_Unit aiUnit)
    {
        throw new System.NotImplementedException();
    }

    public void Init(AI_Unit aI_Unit, Unit target)
    {
        throw new System.NotImplementedException();
    }

    public void Init(AI_Unit aI_Unit, Tile target)
    {
        throw new System.NotImplementedException();
    }
}
