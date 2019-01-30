using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class InterceptSoundRaycaster : MonoBehaviour
{
    public LayerMask layer;
    AudioSource Asource;
    [Range(0, 1)]
    public float Level1 = 0.6f;
    [Range(0, 1)]
    public float Level2 = 0.25f;
    [Range(0, 1)]
    public float Level3 = 0.005f;

    public Transform Player;

    

    // Start is called before the first frame update
    void Start()
    {
        Asource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Intercept()
    {
        var count = LineCastCheck();
        ChangeVol(count);
    }


    int LineCastCheck()
    {
        RaycastHit[] hits;

        Vector3 fromPosition = transform.position;
        Vector3 toPosition = Player.position;
        Vector3 direction = toPosition - fromPosition;
        float dist = Vector3.Distance(Player.position, transform.position);

        hits = Physics.RaycastAll(transform.position, direction, dist, layer);
        return hits.Length;
    }
    public void ChangeVol(int count)
    {
        switch (count)
        {
            default:
                Asource.volume = 0;
                break;
            case 0:
                break;
            case 1:
                Asource.volume *= Level1;
                break;
            case 2:
                Asource.volume *= Level2;
                break;
            case 3:
                Asource.volume *= Level3;
                break;  
        }
    }
}
