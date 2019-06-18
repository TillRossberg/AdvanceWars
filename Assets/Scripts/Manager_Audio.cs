using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Audio : MonoBehaviour
{
    
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioSource dialoguePlayer;
    [SerializeField] AudioSource musicPlayer;
        
    public void Init()
    {
       
    }    
  
    public void PlaySFX(AudioClip sound)
    {        
        sfxPlayer.Stop();
        sfxPlayer.clip = sound;
        sfxPlayer.Play();
    }
    public void PlayDialogue(AudioClip audioClip)
    {
        dialoguePlayer.Stop();
        dialoguePlayer.clip = audioClip;
        dialoguePlayer.Play();
    }
    public void PlayMusic(AudioClip audioClip)
    {
        musicPlayer.Stop();
        musicPlayer.clip = audioClip;
        musicPlayer.Play();
    }

    public void PlayNopeSound()
    {
        PlaySFX(Core.Model.Database.Sounds.NopeSound);
    }
}
