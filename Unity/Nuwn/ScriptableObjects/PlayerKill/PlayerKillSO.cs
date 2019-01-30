using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "PlayerKill", menuName = "ScriptableObjects/PlayerKill/PlayerkillSO", order = 1)]
public class PlayerKillSO : ScriptableObject
{
    public string Trigger;


    //public List<Particles> particles = new List<Particles>();
    //[Serializable]
    //public class Particles
    //{
    //    public GameObject particle;
    //    public float lifeTime;
    //}
}
