using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefKlasse : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        this.GetComponent<Graph>().createExampleGraph01();
        this.GetComponent<UnitManager>().createUnitSet01();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
