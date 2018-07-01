using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip explosion01Sound;
    public AudioClip clickSound;
    public List<AudioClip> music = new List<AudioClip>();
    public AudioSource player;
    public AudioSource backgroundMusicPlayer;
        
    public void init()
    {
        initBackgroundMusic(0);
        //playBackGroundMusic(true);
    }

    public void playSoundAt(string soundName, int x, int y)
    {
        switch(soundName)
        {
            case "Standardexplosion":  
                AudioSource.PlayClipAtPoint(explosion01Sound, new Vector3(x, 0, y), 1f);
                break;

            default:
                Debug.Log("AudioManager: No such sound found!");
                break;
        }
    }
    
    public void initBackgroundMusic(int index)
    {
        backgroundMusicPlayer.clip = music[index];
    }

    public void playSound(AudioClip sound)
    {
        player.Stop();
        player.clip = sound;
        player.Play();
    }

    public void playClickSound()
    {
        playSound(clickSound);
    }

    public void playBackGroundMusic(bool value)
    {
        if(value)
        {
            backgroundMusicPlayer.Play();
        }
        else
        {
            backgroundMusicPlayer.Stop();
        }
    }    
}
