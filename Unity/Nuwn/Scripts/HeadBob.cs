using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public Transform Head;

    public Vector3 offset;

    
    public bool UseLerpPosition = false;
    public float PositionLerpAmount = 1f;
    public bool FollowRot = false;
    public bool UseLerpRotation = false;
    public float RotationLerpAmount = 1f;

    private void LateUpdate()
    {
        if (UseLerpPosition)
            transform.position = Vector3.Lerp(transform.position, Head.position + offset, Time.deltaTime * PositionLerpAmount);
        else
            transform.position = Head.position + offset;

        if (FollowRot)
            if (UseLerpRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, Head.rotation, Time.deltaTime * RotationLerpAmount);
            else
                transform.rotation = Head.rotation;
    }

    public void SetCameraFullFollow()
    {
        FollowRot = true;
        UseLerpPosition = false;
    }
    public void ResetCamera()
    {
        FollowRot = false;
        UseLerpPosition = true;
    }

}