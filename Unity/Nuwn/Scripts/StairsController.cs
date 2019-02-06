using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StairsController : MonoBehaviour
{
    
    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            var player = PlayerMotor.Instance.GetComponent<Playercontroller>();
            player.ChangeSpeed(Playercontroller.PlayerSpeeds.stairs);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = PlayerMotor.Instance.GetComponent<Playercontroller>();
            player.ChangeSpeed(Playercontroller.PlayerSpeeds.speed);
        }
    }
}
