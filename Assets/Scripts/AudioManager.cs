using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip explosion01;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void playSoundAt(string soundName, int x, int y)
    {
        switch(soundName)
        {
            case "Standardexplosion":  
                AudioSource.PlayClipAtPoint(explosion01, new Vector3(x, 0, y), 1f);
                break;

            default:
                Debug.Log("AudioManager: No such sound found!");
                break;
        }
    }
}
