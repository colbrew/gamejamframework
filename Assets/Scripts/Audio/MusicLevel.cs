using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicLevel : MonoBehaviour, IEndDragHandler, IPointerUpHandler
{
    AudioMixer mixer;

    private void Awake()
    {
        mixer = Resources.Load<AudioMixer>("Mixer");
    }

    private void SetVolume()
    {
        float sliderValue = GetComponent<Slider>().value;
        if (sliderValue == 0)
        {
            mixer.SetFloat("MusicVol", -80);
        }
        else
        {
            mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 30);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetVolume();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetVolume();
    }
}
