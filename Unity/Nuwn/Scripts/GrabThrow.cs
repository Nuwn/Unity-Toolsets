using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(IgnoreCollision))]
public class GrabThrow : MonoBehaviour {


    [TextArea]
    [SerializeField]
    private string descriptionTextArea = 
        "WARNING: If the objects pivot point is not centered, put this on a new cube parent and center it. add the scripts on the new parent" +
        "HOWTO: Set Ignore raycast on root so it ignores the circle area," +
        "the circle collider is only to see when the player is in range, " +
        "Set the LOD 0 collider and player collider in ignore collider or it will push you away. " +
        "In any event set IgnoreCollision method SetCollider to true and false" +
        "Set the targeting point to Layer 'targeting' for player to see it. it can be set on LOD 0, " +
        "but if LOD 0 needs other layer, create a new GameObject with targeting collider";


    [SerializeField] private Transform Player = default;
    [SerializeField] private Transform MainCamera = default;
    [SerializeField] private Transform CameraHolder = default;
    [SerializeField] private Transform PickUpRef = default;
    [SerializeField] private Vector3 HoldOffset = default;
    public float rotationSpeed = 20f;
    private Rigidbody rb = default;

    public bool IsLooking { get; private set; }
    public bool InZone { get; private set; }

    public float ThrowForce = 1000f;

    public bool IsHolding { get; set; }
    public bool canHold { get; set; }

    private bool localDisableMovement;
    public bool DisableMovement {
        get { return PlayerMotor.Instance.DisableMovement; }
        set { PlayerMotor.Instance.DisableMovement = value; }
    }

    [Serializable]
    public class Lookatcontrols
    {
        [Header("Choose target to look at")]
        public Transform TargetTransform;
        [Space(10)]
        public TargetingEvent EnterTargeting, ExitTargeting;
    }
    [Serializable]
    public class Colliderevents
    {
        [Space]
        public ColliderEvent AreaEntered, AreaLeft;
    }
    [Serializable]
    public class Holdingevents
    {
        [Space]
        public UnityEvent OnPickup, OnDrop;
    }
    public Lookatcontrols LookatControls = new Lookatcontrols();
    public Colliderevents ColliderEvents = new Colliderevents();
    public Holdingevents HoldingEvents = new Holdingevents();

    // Use this for initializationx
    void Start () {
        rb = GetComponent<Rigidbody>();
        canHold = true;
    }

    
    // Update is called once per frame
    void LateUpdate () {

        if (InZone && PLayerIsLookingAtObject())
        {
            if (!IsLooking)
            {
                if (!IsHolding)
                {
                    InvokeEnterTarget();
                    IsLooking = true;
                }  
            }
        }
        else
        {
            if (IsLooking)
            {
                if (!IsHolding)
                {
                    InvokeExitTarget();
                    IsLooking = false;
                }
            }
        }

        if (IsLooking)
        {
            if (Input.GetKeyDown(KeyCode.E) && InZone)
            {
                Interact();
            }
        }
        if (!canHold)
        {
            StopInteract();
            return;
        }
        if (IsHolding && canHold)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                StopInteract();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StopInteract();
                rb.AddForce(MainCamera.forward * ThrowForce);
                return;
            }

            Pickup();

            if (Input.GetKey(KeyCode.Mouse1))
            {
                //float rotX = Input.GetAxis("Mouse X")*Time.deltaTime*rotationSpeed*Mathf.Deg2Rad;
                //float rotY = Input.GetAxis("Mouse Y")*Time.deltaTime*rotationSpeed*Mathf.Deg2Rad;

                //transform.Rotate(Vector3.up, -rotX, Space.World);
                //transform.Rotate(Vector3.right, rotY, Space.World);

                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSpeed);
            }
        }
        
            
    }

    private void Update()
    {
        canHold = PlayerStatus.Instance.canHold;
        if (IsHolding)
        {
            if(Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Mouse1))
            {
                if (!localDisableMovement)
                {
                    DisableMovement = true;
                    localDisableMovement = true;
                }
            }
            else
            {
                if (localDisableMovement)
                {
                    DisableMovement = false;
                    localDisableMovement = false;
                }
            }
        }
        else
        {
            if (localDisableMovement)
            {
                DisableMovement = false;
                localDisableMovement = false;
            }
        }
            
    }
    private void Pickup()
    {

        var newPos = PickUpRef.TransformPoint(HoldOffset);
        transform.position = Vector3.MoveTowards(transform.position, newPos, 0.2f);
    }
    private void Interact()
    {
        IsHolding = true;
        rb.isKinematic = true;
        HoldingEvents.OnPickup.Invoke();
        transform.parent = PickUpRef;
    }

    private void StopInteract()
    {
        IsHolding = false;
        rb.isKinematic = false;
        HoldingEvents.OnDrop.Invoke();
        transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        InZone = true;
        InvokeAreaEnter();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
            return;
        InZone = false;
        InvokeAreaLeft();
    }
    void InvokeEnterTarget()
    {
        LookatControls.EnterTargeting.Invoke(LookatControls.TargetTransform);
    }
    void InvokeExitTarget()
    {
        LookatControls.ExitTargeting.Invoke(LookatControls.TargetTransform);
    }
    void InvokeAreaEnter()
    {
        ColliderEvents.AreaEntered.Invoke(LookatControls.TargetTransform);
    }
    void InvokeAreaLeft()
    {
        ColliderEvents.AreaLeft.Invoke(LookatControls.TargetTransform);
    }



    bool PLayerIsLookingAtObject()
    {
        if (CameraHolder.GetComponent<PlayerRayCaster>().GetLookingAtTransform() == LookatControls.TargetTransform)
        {
            return true;
        }
        return false;
    }


    public void DebugLog(Transform target)
    {
        Debug.Log(target.name);
    }
}
