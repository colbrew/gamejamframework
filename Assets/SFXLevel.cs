using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXLevel : MonoBehaviour, IEndDragHandler, IPointerUpHandler
{
    const SFX CLIPTOPLAYTOSETVOLLEVEL = SFX.pause;

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
            mixer.SetFloat("SFXVol", -80);
        }
        else
        {
            mixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 30);
            SFXManager.instance.PlayClip(CLIPTOPLAYTOSETVOLLEVEL, Vector3.zero);
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
