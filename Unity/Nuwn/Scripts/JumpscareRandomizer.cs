using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class JumpscareRandomizer : MonoBehaviour {

    JumpScare ActiveJumpscare;
    private JumpScare[] LocalScripts;
    // True if its to trigger this run
    bool active;
    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            if(active)
                Activated();
        }
    }

    private void Awake()
    {
        LocalScripts = GetComponents<JumpScare>();
    }
    
    void Activated () {
        int rand = new System.Random().Next(0,LocalScripts.Length);
        LocalScripts[rand].enabled = true;
        ActiveJumpscare = LocalScripts[rand];
    }

    void Use()
    {
        ActiveJumpscare.Use();
    }
}
