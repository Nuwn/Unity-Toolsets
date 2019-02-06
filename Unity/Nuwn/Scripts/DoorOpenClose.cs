using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent, RequireComponent(typeof(Animator), typeof(InteractableCheck))]
public class DoorOpenClose : MonoBehaviour {
     
        
    public bool startOpen = false;
    public bool startLocked = false;
    public bool canLock = false;

    public bool isLocked { get; set; }
    public bool isOpen { get; set; }


    /* 
     * Create event on the item used to unlock 
     * When picked up?
     * TODO: Create lock system
     */
    
    public bool CanUnlock { get; private set; }

    [Serializable]
    public class DoorEvents
    {
        public UnityEvent OnLock, OnUnlock, OnLocked, OnOpening, OnClose;
    }
    public DoorEvents DoorEvent = new DoorEvents();

    private Animator Anim;

    [Serializable]
    public class TriggerNames //names of the animations, which you use for every action
    { 
        [Header("Triggers")]
        public string OpeningAnim = "Open";
        public string CloseAnim = "Close";
        public string LockedAnim = "Locked";
    }
    public TriggerNames AnimationNames = new TriggerNames();

    // Use this for initialization
    void Start () {
        Anim = GetComponent<Animator>();
        if (startOpen)
            OpenCloseDoor();
        if (startLocked)
            isLocked = true;
    }

    public void OpenCloseDoor()
    {
        if (isLocked)
            if (!CanUnlock)
                PlayLockedAnim();
            else
                LockUnlock();
        else
        {
            if (isOpen)
                PlayCloseAnim();
            else
                PlayOpenAnim();
        }
    }

    void LockUnlock()
    {
        if (!isOpen)
        {
            if (canLock)
            {
                isLocked = !isLocked;
                Anim.SetBool("IsLocked", isLocked);
            }
            else
            {
                isLocked = false;
                Anim.SetBool("IsLocked", isLocked);
            }
            if (isLocked)
                DoorEvent.OnLock.Invoke();
            else
                DoorEvent.OnUnlock.Invoke();
        }  
    }


    void PlayLockedAnim()
    {
        DoorEvent.OnLocked.Invoke();
        Anim.SetTrigger(AnimationNames.LockedAnim);
    }
    void PlayOpenAnim()
    {
        DoorEvent.OnOpening.Invoke();
        isOpen = true;
        Anim.SetTrigger(AnimationNames.OpeningAnim);
    }
    void PlayCloseAnim()
    {
        DoorEvent.OnClose.Invoke();
        isOpen = false;
        Anim.SetTrigger(AnimationNames.CloseAnim);
    }
}
