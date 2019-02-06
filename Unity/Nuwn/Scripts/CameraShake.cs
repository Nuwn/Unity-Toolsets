using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraShake : MonoBehaviour
{
    public void Play(ShakeSO SO)
    {
        CameraShaker.Instance.ShakeOnce( SO.magnitude, SO.roughness, SO.fadeintime, SO.fadeouttime);
    }
    public void ContinuousPlay(ShakeSO SO)
    {
        CameraShaker.Instance.StartShake(SO.magnitude, SO.roughness, SO.fadeintime);
    }
    public void StopShake()
    {
        CameraShaker.Instance.StopShaking();
    }
}