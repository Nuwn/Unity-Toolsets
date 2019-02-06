using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portalteleportal : MonoBehaviour {

    public Transform player;
    public Transform reciever;

    bool playerIsOverlaping = false;
	
	// Update is called once per frame
	void Update () {
		if (playerIsOverlaping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if(dotProduct < 0)
            {
                float rotDiff = Quaternion.Angle(transform.rotation, reciever.rotation);
                rotDiff += 180;
                player.Rotate(Vector3.up, rotDiff);

                Vector3 posOffset = Quaternion.Euler(0, rotDiff, 0) * portalToPlayer;
                player.position = reciever.position + posOffset;

                playerIsOverlaping = false;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == player.tag)
        {
            playerIsOverlaping = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == player.tag)
        {
            playerIsOverlaping = false;
        }
    }
}
