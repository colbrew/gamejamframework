using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Instructions
// Setup:
//    In Unity, create a game object named SFXManager in your first scene 
//    and attach the SFXManager script to it.
// To create a new SFX clip:
//    1. Add the name of the clip to the SFX enum below.
//    2. Create a SFXScriptableObject, add the audio clip(s) and adjust settings as needed on the object.
//    3. Add the newly created SFXScriptableObject to the "Sound Effects" list on the SFXManager you created 
//       in the Inspector and set the name of the SFX clip using the dropdown.


// Per step 1 above, when creating a new SFX clip, add its name to the bottom of this enum 
// and set it equal to the next number. DO NOT CHANGE already assigned numbers.
public enum SFX
{
    selectSfxName = 0,
    menuSelect = 1, // 1-4 are examples, feel free to delete at start of project
    pause = 2, 
    unpause = 3,
    playerDies = 4
}

[System.Serializable]
public class SFXObject
{
    [SerializeField]
    public SFX name;
    [SerializeField]
    public SFXScriptableObject sfxSO;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public static Transform SFX_ANCHOR;

    [Tooltip("To create a new sound effect, click Create->ScriptableObjects->SoundEffectScriptableObject and then add it to this list")]
    public List<SFXObject> soundEffects;

    private List<AudioSourceController> _pool = new List<AudioSourceController>();
    private AudioMixer mixer;

    AudioSourceController asc;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            instance._pool.Clear(); 
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        mixer = Resources.Load<AudioMixer>("Mixer");
    }

    public AudioClip GetClip(SFX clipName)
    {
        SFXScriptableObject sfxSO = soundEffects.Find(x => x.name == clipName).sfxSO;
        var sfxClipArraySize = sfxSO.audioClips.Length;
        if (sfxClipArraySize == 0)
        {
            Debug.LogError("Please add audio clip(s) to " + sfxSO.name + " scriptable object.");
            return null;
        }

        return sfxSO.audioClips[Random.Range(0, sfxClipArraySize)];
    }
    // Returns length of clip that is played
    public float PlayClip(SFX effect, Vector3 pos)
    {
        float clipLength;
        var z = soundEffects.Find(x => x.name == effect);
        if (z != null)
        {
            clipLength = PlayRandomClip(z.sfxSO, pos);
            return clipLength;
        }
        else
        {
            Debug.Log("SFX audio clip not found");
            return 0;
        }
    }
       
    // Returns length of random clip just played
    private float PlayRandomClip(SFXScriptableObject sfxSO, Vector3 pos)
    {
        asc = GetController();
        var audioClipArraySize = sfxSO.audioClips.Length;
        if (audioClipArraySize == 0)
        {
            Debug.LogError("Please set audio clip(s) on " + sfxSO.name + " scriptable object.");
            return 0;
        }
        
        AudioClip clip = sfxSO.audioClips[Random.Range(0,audioClipArraySize)];

        if (asc != null)
        {
            asc.SetPosition(pos);
            asc.SetSourceProperties(clip, sfxSO.volume, sfxSO.pitch, sfxSO.loop);
            asc.Play();
        }
        return clip.length;
    }

    public AudioSourceController GetController()
    {
        if(SFX_ANCHOR == null)
        {
            GameObject go = new GameObject("_SFXAnchor");
            SFX_ANCHOR = go.transform;
            //go.transform.SetParent(this.transform);
        }

        AudioSourceController output = null;
        if (_pool.Count > 0)
        {
            output = _pool[0];
            _pool.Remove(output);
            return output;
        }
        else
        {
            GameObject go = new GameObject("AudioController");
            output = go.AddComponent<AudioSourceController>();
            if (mixer != null)
            {
                output.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            }
            go.transform.SetParent(SFX_ANCHOR, true);
            return output;
        }

    }

    public void ReturnController(AudioSourceController controller)
    {
        if (_pool.Contains(controller) == false)
            _pool.Add(controller);
    }
}
