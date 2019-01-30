using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)),DisallowMultipleComponent]
public class InterceptSoundZone : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;
    public AudioSource Audiosource;
    public BoxCollider collider;
    public List<Color> colors = new List<Color> { new Color(0f, 1f, 0f, 0.3f), new Color(1f, 0.5f, 0f, 0.3f), new Color(1f, 0f, 0f, 0.3f) };

    

    void Start()
    {
        if(!collider)
            collider = GetComponent<BoxCollider>();
        if (!Audiosource)
            Audiosource = GetComponent<AudioSource>();
        if (!Player)
            Player = GameObject.Find("Player").transform;

        if (!collider.bounds.Contains(Player.position))
        {
            Audiosource.mute = true;
        }
        else
        {
            Audiosource.mute = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform == Player)
            Audiosource.mute = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == Player)
            Audiosource.mute = false;
    }

    private void OnDrawGizmosSelected()
    {
        var Bc = GetComponents<BoxCollider>();

        for (var i = 0; i < Bc.Length; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawCube(Bc[i].bounds.center, Bc[i].bounds.size);
        }
    }
}
