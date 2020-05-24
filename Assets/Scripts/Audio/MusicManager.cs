using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections.Generic;

// Instructions
// Setup:
//    In Unity, create a game object named MusicManager in your first scene 
//    and attach the Music Manager script to it.
// To create a new music clip:
//    1. Add the name of the clip to the Music enum below.
//    2. Create a MusicScriptableObject, add the audio clip(s) and adjust settings as needed on the object.
//    3. Add the newly created MusicScriptableObject to the MusicManager in the Inspector 
//       and set the name of the music clip using the dropdown.


// Per step 1 above, when creating a new music clip, add its name to the bottom of this enum 
// and set it equal to the next number. DO NOT CHANGE already assigned numbers.
public enum Music
{
    SelectMusicType = 0,
    MainMenuMusic = 1,
    GameplayLoopRegular = 2,
    GameplayLoopIntense = 3,
    LevelIntro = 4,
    LevelRestart = 5,
    LoseLevel = 6,
    WinLevel = 7,
    WinGame = 8
}

[System.Serializable]
public class MusicObject
{
    [SerializeField]
    public Music name;
    [SerializeField]
    public MusicScriptableObject musicSO;
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;
    [Header("Set in Inspector")]
    [Tooltip("To create a new Music clip, click Create->ScriptableObjects->MusicScriptableObject and then add it to this list")]
    public List<MusicObject> music;
    [SerializeField] float crossfadeDuration = 1;

    [Header("For Testing")]
    public bool crossFadeGamePlayLoops = false;

    private AudioSource[] audioSources;
    private bool crossFaded = false;
    private AudioMixer mixer;
    private MusicScriptableObject musicSO;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        // Add two audio sources (for cross-fading)
        gameObject.AddComponent<AudioSource>();
        gameObject.AddComponent<AudioSource>();
        audioSources = GetComponents<AudioSource>();

        mixer = Resources.Load<AudioMixer>("Mixer");
        if (mixer != null)
        {
            foreach(AudioSource audio in audioSources)
            {
                audio.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
            }
        }
    }

    private void Start()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index == 0)
            PlayMusic(Music.MainMenuMusic, true);
        else
        {
            PlayMusic(Music.GameplayLoopRegular);
        }   
    }

    private void Update()
    {
        Testing();
    }
    
    private void Testing()
    {
        if (crossFadeGamePlayLoops && !crossFaded)
        {
            crossFaded = true;
            CrossFadeAtoB(audioSources[0], audioSources[1], crossfadeDuration);
        }

        if (!crossFadeGamePlayLoops && crossFaded)
        {
            crossFaded = false;
            CrossFadeAtoB(audioSources[1], audioSources[0], crossfadeDuration);
        }
    }
    /*
    public void PlayMusicWIntro(AudioClip introClip, AudioClip loopingClip)
    {
        StopOtherMusicCoroutines();
        StartCoroutine(PlayMusicIntro(introClip, loopingClip));
    }

    IEnumerator PlayMusicIntro(AudioClip introClip, AudioClip loopingClip)
    {
        audioSources[0].Stop();
        audioSources[0].clip = introClip;
        audioSources[0].loop = false;
        audioSources[0].Play();
        yield return new WaitForSeconds(audioSources[0].clip.length);
        StartMusicLoop(loopingClip);
    }*/

    public AudioClip GetClip(Music clipName)
    {
        musicSO = music.Find(x => x.name == clipName).musicSO;
        var musicClipArraySize = musicSO.musicClip.Length;
        if (musicClipArraySize == 0)
        {
            Debug.LogError("Please add audio clip(s) to " + musicSO.name + " scriptable object.");
            return null;
        }

        return musicSO.musicClip[Random.Range(0, musicClipArraySize)];
    }

    private void PlayMusic(Music clip, bool looping = false)
    {
        StopMusic();
        if (looping)
        {
            audioSources[0].loop = true;
        }
        else
        {
            audioSources[0].loop = false;
        }
        audioSources[0].clip = GetClip(clip);
        audioSources[0].Play();
    }

    public void PlayTwoMusicLoops(Music clipA, Music clipB, float delay = 0)
    {
        StartCoroutine(DelayingTwoMusicClips(clipA, clipB, delay));
    }

    IEnumerator DelayingTwoMusicClips(Music clipA, Music clipB, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTwoLoops(clipA, clipB);
    }

    public void StartTwoLoops(Music clipA, Music clipB)
    {
        if (audioSources.Length < 2)
        {
            Debug.LogError("Not enough audio sources to play two music loops at the same time.");
            return;
        }

        AudioClip loopingClipA = GetClip(clipA);
        AudioClip loopingClipB = GetClip(clipB);

        StopMusic();

        audioSources[0].clip = loopingClipA;
        audioSources[0].volume = 1;

        audioSources[1].clip = loopingClipB;
        audioSources[1].volume = 0;

        foreach (AudioSource source in audioSources)
        {
            source.loop = true;
            source.Play();
        }
    }

    public void CrossFadeAtoB(AudioSource a, AudioSource b, float duration)
    {
        StartCoroutine(CrossFade(a, b, duration));
    }

    IEnumerator CrossFade(AudioSource a, AudioSource b, float duration)
    {
        float startTime = Time.time;
        float u = 0;
        float aVolStart = a.volume;
        float bVolStart = b.volume;
        float aVol;
        float bVol;

        while(u < 1)
        {
            u = (Time.time - startTime) / duration;
            aVol = Mathf.Lerp(aVolStart, bVolStart, u);
            bVol = Mathf.Lerp(bVolStart, aVolStart, u);
            a.volume = aVol;
            b.volume = bVol;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public void StopMusic()
    {
        StopOtherMusicCoroutines();
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
        ResetVolumes();
    }

    void StopOtherMusicCoroutines()
    {
        StopAllCoroutines();
    }

    void ResetVolumes()
    {
        audioSources[0].volume = 1;
        audioSources[1].volume = 0;
    }

    public void CrossFadeRegularToIntense()
    {
        CrossFadeAtoB(audioSources[0], audioSources[1], crossfadeDuration);
    }

    public void CrossFadeIntenseToRegular()
    {
        CrossFadeAtoB(audioSources[1], audioSources[0], crossfadeDuration);
    }
}
