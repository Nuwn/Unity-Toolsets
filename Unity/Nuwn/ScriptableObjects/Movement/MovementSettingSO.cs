using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "MovementSO", menuName = "ScriptableObjects/Movement/MovementSO", order = 1)]
public class MovementSettingSO: ScriptableObject
{
    public float lookSensitivity;
    public float speed;
}
