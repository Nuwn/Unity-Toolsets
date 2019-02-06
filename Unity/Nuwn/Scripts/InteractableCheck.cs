using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class InteractableCheck : MonoBehaviour {

    public enum InteractStyle
    {
        NONE,
        BUTTON,
        AUTOMATIC,
        AUTOMATIC_ONCE
    }
    [Serializable]
    public struct Transformoptions
    {
        [Header("If selected, OnTrigger will send the transform of target")]
        public Transform TargetTransform;
    }
    [Serializable]
    public class Lookatcontrols
    {
        public bool UseTargeting;
        [Header("Choose target to look at")]
        public Transform TargetTransform;
        [Header("Used for checking if target is looking other dir")]
        public Transform secondTargetTransform;
        [Space(10)]
        public TargetingEvent EnterTargeting, ExitTargeting, ObjectBehind;
    }
    [Serializable]
    public class Interactoptions
    {
        [Header("Automatic fires interact on enter and exit")]
        public InteractStyle InteractMethod = InteractStyle.BUTTON;
        public KeyCode InteractButton = KeyCode.E;
        public float Cooldown = 1;
        public bool OneShot = false;


        public Transformoptions TransformOptions;
        public Lookatcontrols TargetingControls = new Lookatcontrols();
    }
    [Serializable]
    public class Colliderevents
    {
        [Space]
        public ColliderEvent AreaEntered, AreaLeft, Interacted;
    }

    public bool CanBeUsedByAI;

    public Interactoptions InteractOptions = new Interactoptions();
    public Colliderevents ColliderEvents = new Colliderevents();

    private Transform Player;
    private Transform MainCamera;
    private Transform CameraHolder;

    public bool InZone { get; private set; }
    public bool Oneshot { get; private set; }
    public bool usedOneshot { get; private set; }
    private float cooldown = 0;
    private bool IsLooking = true;
    private bool IsBehind = false;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        CameraHolder = MainCamera.parent;

        if (InteractOptions.TransformOptions.TargetTransform == null)
            InteractOptions.TransformOptions.TargetTransform = transform;

        if (InteractOptions.TargetingControls.TargetTransform == null)
            InteractOptions.TargetingControls.TargetTransform = transform;

        if (InteractOptions.TargetingControls.secondTargetTransform == null)
            InteractOptions.TargetingControls.secondTargetTransform = InteractOptions.TargetingControls.TargetTransform;

        Oneshot = false;
        usedOneshot = false;
        IsLooking = !InteractOptions.TargetingControls.UseTargeting;
    }

    private void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        if(InteractOptions.TargetingControls.UseTargeting)
            if (InZone && PLayerIsLookingAtObject())
            {
                if (!IsLooking)
                {
                    InvokeEnterTarget();
                    IsLooking = true;
                }                   
            }
            else if(InZone && !PLayerIsLookingAtObject())
            {
                if (IsLooking)
                {
                    InvokeExitTarget();
                    IsLooking = false;
                }
                if (ObjectIsBehindPlayer())
                {
                    if (!IsBehind)
                    {
                        InvokeObjectBehind();
                        IsBehind = true;
                    }
                }
                else { if (IsBehind) IsBehind = false; }
            }

        if (IsLooking)
            if (InteractOptions.InteractMethod == InteractStyle.BUTTON)
                if (Input.GetKeyDown(KeyCode.E) && InZone && !Oneshot && cooldown <= 0)
                {
                    Interact();
                }

    }
    void Interact()
    {
        InvokeInteracted();
        if (InteractOptions.OneShot)
            Oneshot = true;
        cooldown = InteractOptions.Cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" && other.tag != "AI")
            return;

        InZone = true;
        InvokeAreaEnter();
        if (InteractOptions.InteractMethod == InteractStyle.AUTOMATIC || (other.tag == "AI" && CanBeUsedByAI))
            InvokeInteracted();
        else if (InteractOptions.InteractMethod == InteractStyle.AUTOMATIC_ONCE)
        {
            if (!usedOneshot) { InvokeInteracted(); usedOneshot = true; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player" && other.tag != "AI")
            return;

        InZone = false;
        InvokeAreaLeft();
        if (InteractOptions.InteractMethod == InteractStyle.AUTOMATIC || (other.tag == "AI" && CanBeUsedByAI))
            InvokeInteracted();
    }

    void InvokeInteracted()
    {
        ColliderEvents.Interacted.Invoke(InteractOptions.TransformOptions.TargetTransform);
    }
    void InvokeAreaEnter()
    {
        ColliderEvents.AreaEntered.Invoke(InteractOptions.TransformOptions.TargetTransform);
    }
    void InvokeAreaLeft()
    {
        ColliderEvents.AreaLeft.Invoke(InteractOptions.TransformOptions.TargetTransform);
    }
    void InvokeEnterTarget()
    {
        InteractOptions.TargetingControls.EnterTargeting.Invoke(InteractOptions.TargetingControls.TargetTransform);
    }
    void InvokeExitTarget()
    {
        InteractOptions.TargetingControls.ExitTargeting.Invoke(InteractOptions.TargetingControls.TargetTransform);
    }
    void InvokeObjectBehind()
    {
        InteractOptions.TargetingControls.ObjectBehind.Invoke(InteractOptions.TargetingControls.TargetTransform);
    }
    bool PLayerIsLookingAtObject()
    {
        if (CameraHolder.GetComponent<PlayerRayCaster>().GetLookingAtTransform() == InteractOptions.TargetingControls.TargetTransform)
            {
                return true;
            }
            return false;        
    }
    bool ObjectIsBehindPlayer()
    {
        var Target = InteractOptions.TargetingControls.secondTargetTransform;
        
        Vector3 dirFromAtoB = (Target.position - Player.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, Player.transform.forward);

        if (dotProd < -0.8)
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



[Serializable]
public class ColliderEvent : UnityEvent<Transform> { }
[Serializable]
public class TargetingEvent : UnityEvent<Transform> { }
