using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Template", menuName = "ScriptableObjects/Audio/AudioSo", order = 1)]
public class AudioScriptableObject : ScriptableObject
{
    public AudioClip AudioClip;
    [Range(0.0f, 1.0f)]
    public float Volume;
    public AudioMixerGroup AudioMixerGroup;
    public bool Loop;
}
