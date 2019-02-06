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
            //Debug.Log(hit.collider.transform.name);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
        }
    }

    public Transform GetLookingAtTransform()
    {
        try
        {
            return hit.collider.transform;
        }
        catch (System.NullReferenceException)
        {
            return null;
        }
    }
    public Collider GetLookingAtColl()
    {
        try
        {
            return hit.collider;
        }
        catch (System.NullReferenceException)
        {
            return null;
        }
    }
}
