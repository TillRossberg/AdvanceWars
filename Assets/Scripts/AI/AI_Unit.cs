using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AI_Unit 
{
    event Action OnOrderFinished;
    public Unit Unit;
    public Unit target;
    public Tile moveTarget;
    List<IOrder> Orders = new List<IOrder>();
    int _orderIndex;
    public void Init(Unit unit)
    {
        this.Unit = unit;
    }
    public void Exit()
    {

    }

    public void AddOrder(IOrder order)
    {
        Orders.Add(order);
    }
    public void StartExecutingOrders()
    {
        _orderIndex = 0;
        if(Orders.Count > 0)
        {
            Orders[_orderIndex].Init(this);
            _orderIndex++;
        }
        else Debug.Log(Unit + " doesnt have any orders!");
    }
    public void ExecuteNextOrder()
    {
        if(_orderIndex < Orders.Count - 1)
        {
            Orders[_orderIndex].Init(this);
            _orderIndex++;
        }
        else
        {

            Debug.Log(Unit + " executed all orders!");
        }
    }
}
