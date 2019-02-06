using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {

    public Transform playerCamera;
    public Transform portalA;
    public Transform portalB;

    private void Update()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - portalA.position;
        transform.position = portalB.position + playerOffsetFromPortal;

        float angulardiff = Quaternion.Angle(portalB.rotation, portalA.rotation);
        Quaternion portalDiff = Quaternion.AngleAxis(angulardiff, Vector3.up);
        Vector3 newCameraDir = portalDiff * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDir, Vector3.up);
    }

}
