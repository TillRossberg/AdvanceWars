using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Audio : MonoBehaviour
{
    
    [SerializeField] AudioSource SfxPlayer;
    [SerializeField] AudioSource DialoguePlayer;
    [SerializeField] AudioSource MusicPlayer;
        
    public void Init()
    {
       
    }    
  
    public void PlaySFX(AudioClip sound)
    {        
        SfxPlayer.Stop();
        SfxPlayer.clip = sound;
        SfxPlayer.Play();
    }
    public void PlayDialogue(AudioClip audioClip)
    {
        DialoguePlayer.Stop();
        DialoguePlayer.clip = audioClip;
        DialoguePlayer.Play();
    }
    public void PlayMusic(AudioClip audioClip)
    {
        MusicPlayer.Stop();
        MusicPlayer.clip = audioClip;
        MusicPlayer.Play();
    }

    public void PlayNopeSound()
    {
        PlaySFX(Core.Model.Database.Sounds.NopeSound);
    }
}
