using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundEffect", menuName = "ScriptableObjects/SoundEffectScriptableObject", order = 1)]
public class SFXScriptableObject : ScriptableObject
{
    public AudioClip[] audioClips;
    [Range(0, 1)]
    public float volume = 1;
    [Range(-3,3)]
    public float pitch = 1;
    public bool loop = false;
}
