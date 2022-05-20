using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class PlayerOffice : MonoBehaviour {

    [SerializeField] private List<GameObject> pcInteraction = new List<GameObject>();
    [SerializeField] private Transform camPcParent;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerMovement player;

    private Vector3 camResetPos;
    private Quaternion camResetRot;

    private bool usingPc;

    private void Start() {
        camResetPos = cam.transform.localPosition;
        camResetRot = cam.transform.localRotation;
    }

    public void PcInteraction() {
        if(usingPc) {
            cam.transform.localPosition = camResetPos;
            cam.transform.localRotation = camResetRot;
            Utility.SetTransform(cam.transform, camResetPos, camResetRot, true);

            player.enabled = true;
        } else {
            cam.transform.SetParent(camPcParent);
            Utility.SetTransform(cam.transform, Vector3.zero, Quaternion.identity, true);

            player.enabled = false;
        }

        usingPc = !usingPc;
    }

}