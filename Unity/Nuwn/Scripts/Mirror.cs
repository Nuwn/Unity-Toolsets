using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour {

    public Transform MirrorCam;
    public Transform PlayerCam;

    private void Update()
    {
        CaculateRotation();
    }

    private void CaculateRotation()
    {
        Vector3 dir = (PlayerCam.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        rotation.eulerAngles = transform.eulerAngles - rotation.eulerAngles;

        MirrorCam.localRotation = rotation;
    }
}
