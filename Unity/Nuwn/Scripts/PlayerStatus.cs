using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour {

    public static PlayerStatus Instance { get; private set; }

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

    public bool canHold { get; set; }

    public PlayerKillSO testKill = default;
    [SerializeField]private HeadBob HeadBob = default;
    Animator anim = default;
    Playercontroller PC = default;

    public enum PlayerModes
    {
        NORMAL,
        HURT,
        DEAD,
        PANIC,
        ANGER
    }
    public UnityEvent OnPanic, OnHurt, OnAnger, OnNormal, OnDeath;


    void Start()
    {
        anim = GetComponent<Animator>();
        PC = GetComponent<Playercontroller>();
        PlayerMode(PlayerModes.NORMAL);
    }


    public void Kill(PlayerKillSO so)
    {
        PlayerMode(PlayerModes.HURT);
        DisableMovements(true);
        HeadBob.SetCameraFullFollow();
        anim.SetLayerWeight(anim.GetLayerIndex("Movement"), 0);
        anim.SetLayerWeight(anim.GetLayerIndex("Death"), 1);
        anim.SetTrigger(so.Trigger);
    }
    public void Dead()
    {
        PlayerMode(PlayerModes.DEAD);
    }
    public void Normal()
    {
        PlayerMode(PlayerModes.NORMAL);
        HeadBob.ResetCamera();
        anim.SetLayerWeight(anim.GetLayerIndex("Movement"), 1);
        anim.SetLayerWeight(anim.GetLayerIndex("Death"), 0);
        CameraPostProcessModifier.Instance.SetNormal();
    }
    public void ResetPlayer()
    {
        anim.SetTrigger("Getting Up");
        PlayerMode(PlayerModes.HURT);
    }


    public void PlayerMode(int mode)
    {
        PlayerMode((PlayerModes) mode);
    }

    public void PlayerMode(PlayerModes mode)
    {
        switch (mode)
        {
            case PlayerModes.NORMAL:
                OnNormal.Invoke();
                break;
            case PlayerModes.HURT:
                OnHurt.Invoke();
                break;
            case PlayerModes.PANIC:
                OnPanic.Invoke();
                break;
            case PlayerModes.ANGER:
                OnAnger.Invoke();
                break;
            case PlayerModes.DEAD:
                OnDeath.Invoke();
                break;
        }
    }
    
    public void DisableMovements(bool v)
    {
        PlayerMotor.Instance.enabled = !v;
        if (v)
            anim.SetLayerWeight(anim.GetLayerIndex("Movement"), 0);
        else
            anim.SetLayerWeight(anim.GetLayerIndex("Movement"), 1);
    }
    public void ChangeMovement(MovementSettingSO set)
    {
        if (set == null)
            PC.resetSettings();

        PC.lookSensitivity = set.lookSensitivity;
        PC.speed = set.speed;
    }

    public void CanHold(bool v)
    {
        canHold = v;
    }
}
