using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform playerCamera;
    public Transform portal;
    public Transform otherPortal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffset = playerCamera.position - otherPortal.position;
        transform.position = portal.position + playerOffset;

        float angularRotationOffset = Quaternion.Angle(portal.rotation, otherPortal.rotation);
        Quaternion portalRotationOffset = Quaternion.AngleAxis(angularRotationOffset, Vector3.up);
        Vector3 newCameraDirection = portalRotationOffset * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);


    }
}
