//created by Till Roßberg, 2017-18

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Transform explosion;


	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void boom(int x, int y)
    {
        Core.AudioManager.PlaySFX(Core.Model.Database.sounds.explosion01Sound);
        Transform exploder = Instantiate(explosion, new Vector3(x, 0, y), this.transform.rotation);
        Destroy(exploder.gameObject, 3);
    }

    public void moveTo(Unit unit, int x, int y)
    {
        
    }
}
