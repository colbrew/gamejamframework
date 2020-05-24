using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourceController : MonoBehaviour
{
    private AudioSource _source;
    private Transform _transform;
    private Transform _parentObject;
    private bool _claimed;

    void Awake()
    {
        _transform = this.transform;
        _source = GetComponent<AudioSource>();
        if (_source == null)
        {
            _source = gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().priority = 50;
        }
    }

    void LateUpdate()
    {
        if (_claimed && _source.isPlaying == false)
        {
            Stop();
            return;
        }
        if (_parentObject != null)
        {
            _transform.position = _parentObject.position;
        }
    }

    public void SetSourceProperties(AudioClip clip, float volume, float pitch, bool loop)
    {
        _source.clip = clip;
        _source.volume = volume;
        _source.pitch = pitch;
        _source.loop = loop;
    }

    public void SetClip(AudioClip clip)
    {
        _source.clip = clip;
    }

    public void SetParent(Transform parent)
    {
        _parentObject = parent;
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void Play()
    {
        _claimed = true;
        _source.Play();
    }

    public void Stop()
    {
        _source.Stop();
        Reset();
        SFXManager.instance.ReturnController(this);
    }

    private void Reset()
    {
        _parentObject = null;
        _claimed = false;
    }
}
