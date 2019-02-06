using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpscareRandomizer))]
public abstract class JumpScare : MonoBehaviour {

    public abstract void Use();
    public abstract void OnEnable();
}
