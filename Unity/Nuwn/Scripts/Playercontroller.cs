using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PlayerMotor))]
public class Playercontroller : MonoBehaviour
{
    private float stSpeed;
    private float stLook;
    public float speed = 5f;
    public float StairBreak = 1f;
    public float lookSensitivity = 1f;
    

    private PlayerMotor motor;

    private CustomGravity gravity;
    private float distToGround;

    public static class PlayerSpeeds
    {
        public static float speed;
        public static float look;
        public static float stairs;
    }

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        gravity = GetComponent<CustomGravity>();
        distToGround = GetComponent<Collider>().bounds.extents.y;


        PlayerSpeeds.speed = speed;
        PlayerSpeeds.look = lookSensitivity;
        PlayerSpeeds.stairs = speed - StairBreak;
    }

    private void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        Move(xMove, yMove);
        Rotate(xMouse);
        RotateCamera(yMouse);
        //Crouch();

    }


    public void ChangeSpeed(float val)
    {
        speed = val;
    }

    private void Crouch()
    {
        throw new NotImplementedException();
    }

    private void RotateCamera(float yMouse)
    {
        float cameraRotation = yMouse * lookSensitivity;
        motor.RotateCamera(cameraRotation);
    }

    private void Rotate(float xMouse)
    {
        Vector3 rotation = new Vector3(0, xMouse, 0) * lookSensitivity;
        motor.Rotate(rotation);
    }

    public void Move(float xMove, float yMove)
    {
        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * yMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);
    }

    public void resetSettings()
    {
        speed = PlayerSpeeds.speed;
        lookSensitivity = PlayerSpeeds.look;
    }
}


