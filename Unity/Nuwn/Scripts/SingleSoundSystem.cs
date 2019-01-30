using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SingleSoundSystem : MonoBehaviour
{
    
    public AudioScriptableObject[] sounds;
    AudioSource audioSource;
    public UnityEvent OnSoundPlay;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        foreach(var s in sounds)
        {
            audioSource.PlayOneShot(s.AudioClip);
        }
        
    }
    /// <summary>
    /// Create a AudioScriptableObject, use that when playing a sound with this component
    /// </summary>
    public void PlayEventSound(AudioScriptableObject sound)
    {
        audioSource.volume = sound.Volume;
        audioSource.clip = sound.AudioClip;
        audioSource.outputAudioMixerGroup = sound.AudioMixerGroup;
        audioSource.loop = sound.Loop;
        audioSource.Play();
        OnSoundPlay.Invoke();
    }
    /// <summary>
    /// Create a AudioScriptableObjectAdv, use that when playing a sound with this component, this one lets you set pitch variance and volume variance
    /// </summary>
    public void PlayAdvancedEventSound(AudioScriptableObjectAdv sound)
    {
        audioSource.outputAudioMixerGroup = sound.AudioMixerGroup;
        audioSource.loop = sound.Loop;
        int getRandom = UnityEngine.Random.Range(0, sound.AudioClip.Length - 1);
        audioSource.volume = sound.Volume + UnityEngine.Random.Range(-sound.VolumeVariance, sound.VolumeVariance);
        audioSource.pitch = sound.Pitch + UnityEngine.Random.Range(-sound.PitchVariance, sound.PitchVariance);
        audioSource.PlayOneShot(sound.AudioClip[getRandom]);
        OnSoundPlay.Invoke();
    }

}
