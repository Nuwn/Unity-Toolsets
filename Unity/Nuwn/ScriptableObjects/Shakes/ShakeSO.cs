using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ShakeSo", menuName = "ScriptableObjects/Shake/ShakeSO", order = 1)]
public class ShakeSO : ScriptableObject
{
    public float magnitude;
    public float roughness;
    public float fadeintime;
    [Header("continues does not use fade out")]
    public float fadeouttime;

}
