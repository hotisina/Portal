using Cinemachine;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PortalTeleporter : MonoBehaviour
{
    public Transform player;
    public Transform receiver;
    public GameObject fakePlayer;
    public GameObject playerCamera;
    public GameObject playerVC;

    private GameObject vGO;

    private float teleportTime;

    private bool isOverlapping = false;

    private bool fixedrotation = false;
    private bool cameraIsTeleport = false;
    private void Awake() {
        Time.fixedDeltaTime = 0.0125f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (isOverlapping) {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);
            Quaternion playerRotation = player.rotation;
            //GameObject vGO;

            if(dotProduct < 0) {
                //Teleport!
                //Debug.Log("is to teleport");

                //float rotationDiff = - Quaternion.Angle(transform.rotation, receiver.rotation);
                
                Vector3 positionOffset = player.position - transform.position;
                Vector3 playerPosition = player.position;
                player.position = receiver.position + positionOffset;
                vGO = Instantiate(fakePlayer, playerPosition, player.rotation);
                vGO.tag = "Untagged";
                playerVC.GetComponent<CinemachineVirtualCamera>().Follow = vGO.transform;
                playerVC.GetComponent<CinemachineVirtualCamera>().LookAt = vGO.transform;
                

                //player.rotation = playerRotation;
                //player.Rotate(Vector3.up, rotationDiff);

                //Debug.Log("position = (" + player.position.x + ", " + player.position.y + ", " + player.position.z + ")\n");

                isOverlapping = false;

                fixedrotation = true;
                teleportTime = Time.time;
            }
        }

        if (vGO != null) {
            vGO.transform.rotation = player.transform.rotation;
            Vector3 deltaPosition = transform.position - receiver.position;
            vGO.transform.localPosition = player.position + deltaPosition;
        }

        if (fixedrotation) {
            player.rotation = Quaternion.Euler(0, 0, 0);
            if(Time.time - teleportTime > 0.5f) {
                fixedrotation = false;
            }
        }

        if (cameraIsTeleport) {
            playerVC.GetComponent<CinemachineVirtualCamera>().LookAt = player;
            playerVC.GetComponent<CinemachineVirtualCamera>().Follow = player;
            playerVC.GetComponent<CinemachineVirtualCamera>().enabled = true;
            /*CinemachineVirtualCamera cmvc = playerVC.GetComponent<CinemachineVirtualCamera>();
            CinemachineTrackedDolly body = cmvc.GetCinemachineComponent<CinemachineTrackedDolly>();
            CinemachinePOV aim = cmvc.AddCinemachineComponent<CinemachinePOV>();
            body.enabled = true;
            cmvc.enabled = true;*/
            cameraIsTeleport = false;
            Destroy(vGO);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            isOverlapping = true;
        }
        if(other.tag == "MainCamera") {
            //Debug.Log("Camera Crossed");
            vGO = playerVC.GetComponent<CinemachineVirtualCamera>().Follow.gameObject;

            CinemachineVirtualCamera cmvc = playerVC.GetComponent<CinemachineVirtualCamera>();
            CinemachineTrackedDolly body = cmvc.GetCinemachineComponent<CinemachineTrackedDolly>();
            CinemachinePOV aim = cmvc.GetCinemachineComponent<CinemachinePOV>();
            //CinemachineTrackedDolly body1 = cmvc.AddCinemachineComponent();
            //CinemachinePOV aim1 = cmvc.AddCinemachineComponent<CinemachinePOV>();

            Vector3 cameraOffset = playerVC.transform.position - vGO.transform.position;

            cmvc.enabled = false;            

            playerCamera.transform.position = player.position + cameraOffset;
            playerCamera.transform.rotation = player.rotation;
            cameraIsTeleport = true;

            

            Destroy(vGO);
            
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            isOverlapping = false;
        }
    }
}
