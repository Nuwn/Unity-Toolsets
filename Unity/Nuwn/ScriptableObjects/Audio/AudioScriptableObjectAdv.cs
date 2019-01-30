using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Template", menuName = "ScriptableObjects/Audio/AudioSoAdv", order = 1)]
public class AudioScriptableObjectAdv : ScriptableObject
{
    public AudioClip[] AudioClip;
    public float Volume = 0.8f;
    public float VolumeVariance = 0.2f;
    public float Pitch = 1f;
    public float PitchVariance = 0.05f;
    public AudioMixerGroup AudioMixerGroup;
    public bool Loop;
}
