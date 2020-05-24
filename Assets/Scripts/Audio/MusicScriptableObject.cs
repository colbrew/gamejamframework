using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "Music", menuName = "ScriptableObjects/MusicScriptableObject", order = 1)]
public class MusicScriptableObject : ScriptableObject
{
    public AudioClip[] musicClip;
    [Range(0, 1)]
    public float volume = 1;
    [Range(-3, 3)]
    public float pitch = 1;
    public bool loop = false;
}
