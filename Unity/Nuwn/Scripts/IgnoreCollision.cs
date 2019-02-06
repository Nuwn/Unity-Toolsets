using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nuwn.Essentials;

public class IgnoreCollision : MonoBehaviour {

    public Collider PlayerCollider;
    public List<Collider> CollidersToIgnore;
    

    public void SetCollider(bool v)
    {
        Nuwn_Colliders.MultiIgnoreCollision(PlayerCollider, CollidersToIgnore, v);
    }

}
