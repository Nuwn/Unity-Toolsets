using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayCaster : MonoBehaviour {

    RaycastHit hit;
    public LayerMask mask;
    public Transform DebugTarget;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, mask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            DebugTarget = hit.transform;
            Debug.Log(hit.collider.name);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        }
    }

    public RaycastHit GetLookingAt()
    {
        return hit;
    }

}
