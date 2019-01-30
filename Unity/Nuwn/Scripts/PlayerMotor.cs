using UnityEngine;
using System.Collections;
using System;
using Nuwn.Essentials;
public class PlayerMotor : MonoBehaviour
{

    public static PlayerMotor Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool DisableMovement { get; set; }

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotation = 0f;
    private float currentCameraRotationX = 0f;
    public Vector2 restrictYLook = new Vector2(50f, -50f);


    private Rigidbody rb;
    [SerializeField] private Transform cameraHolder = default;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        DisableMovement = false;
    }


    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void RotateCamera(float _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if(!DisableMovement)
        if (velocity != Vector3.zero)
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    Vector3 v;
    float v2;
    private void PerformRotation()
    {
        if (!DisableMovement)
        {

            rb.rotation = Quaternion.Euler(Vector3.SmoothDamp(rb.rotation.eulerAngles, rb.rotation.eulerAngles + rotation, ref v, Time.deltaTime));
            //rb.MoveRotation(rb.rotation * (Quaternion.Euler(rotation * Time.deltaTime * 100)));

            if (!cameraHolder)
                return;

            // Set our rotation and clamp it
            currentCameraRotationX = Mathf.SmoothDamp(currentCameraRotationX, currentCameraRotationX - cameraRotation, ref v2, Time.deltaTime);
            //currentCameraRotationX -= cameraRotation;

            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, restrictYLook.x, restrictYLook.y);

            //Apply our rotation to the transform of our camera
            cameraHolder.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }


}
