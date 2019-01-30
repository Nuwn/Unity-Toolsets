using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FootSound Template", menuName = "ScriptableObjects/FootSoundSO/FootSoundSo", order = 1)]
public class FootSoundSO : ScriptableObject
{
    public Texture texture;
    public AudioClip[] sounds;
    public float Volume = 0.8f;
    public float VolumeVariance = 0.2f;
    public float Pitch = 1f;
    public float PitchVariance = 0.05f;

}
