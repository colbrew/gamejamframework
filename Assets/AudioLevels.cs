using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AudioLevels : MonoBehaviour, IEndDragHandler
{ 
    AudioMixer mixer;

    private bool SFXPlaying = false;

    private void Awake()
    {
        mixer = Resources.Load<AudioMixer>("Mixer");
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        if (!SFXPlaying)
        {
            SFXPlaying = true;
            StartCoroutine(SFXChange());
        }
    }

    IEnumerator SFXChange()
    {
        float delay = SFXManager.instance.PlayClip(SFX.pause, Vector3.zero);
        yield return new WaitForSeconds(delay);
        SFXPlaying = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SFXManager.instance.PlayClip(SFX.pause, Vector3.zero);
    }
}
