using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundArea : MonoBehaviour
{

    [SerializeField] private AudioScriptableObject[] AudioSo;
    public bool PlayAsQueue;
    float LastPlay;


    private void OnTriggerEnter(Collider other)
    {
        foreach (var a in AudioSo)
        {
            AudioSource ac = gameObject.AddComponent<AudioSource>();
            ac.loop = a.Loop;
            ac.clip = a.AudioClip;
            ac.playOnAwake = false; //prevent dupe
            ac.volume = a.Volume;
            float length = a.AudioClip.length;
        }

        if (PlayAsQueue)
        {
            StartCoroutine(PlayNext());
        }
        else
        {
            foreach (var a in GetComponents<AudioSource>())
            {
                a.Play();
            }   
        }
    }

    IEnumerator PlayNext()
    {
        foreach (var a in GetComponents<AudioSource>())
        {
            if(a != null)
            {
                a.Play();                
                yield return new WaitForSeconds(a.clip.length);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach(var ac in GetComponents<AudioSource>())
        {
            Destroy(ac);
        }
    }


}
