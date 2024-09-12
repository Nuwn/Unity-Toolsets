using System.Collections;
using Lazy.Timeline;
using UnityEngine;
using UnityEngine.Playables;

public class TestTriggers : MonoBehaviour
{
    IEnumerator Start()
    {
        var director = GetComponent<PlayableDirector>();

        yield return new WaitForSeconds(3);

        if (director.state != PlayState.Playing)
            director.Play();

        director.SetTrigger("Trigger1");
    }
} 
