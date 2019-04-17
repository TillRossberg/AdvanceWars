using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Database Sounds")]
public class Data_Sounds : ScriptableObject
{
    public AudioClip explosion01Sound;
    public AudioClip clickSound;
    public AudioClip nopeSound;
    public List<AudioClip> music = new List<AudioClip>();
}
