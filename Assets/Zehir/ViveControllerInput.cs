using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class ViveControllerInput : MonoBehaviour
{
    // public SteamVR_LaserPointer laserPointer;
    // public SteamVR_ActionSet actionSet;
    public SteamVR_Input_Sources leftHand;
    public SteamVR_Input_Sources rightHand;
    public SteamVR_Action_Boolean teleportUp =  SteamVR_Input.GetBooleanAction("TeleportUp");
    public SteamVR_Action_Boolean teleportDown = SteamVR_Input.GetBooleanAction("TeleportDown");
    public Ray ray;
    public GameObject leftController;
    public GameObject rightController;
    public GameObject rearMirror;
    public GameObject realityLens;
    public bool isMirror = false;
    public bool isLens = false;

    void Start()
    {
        // actionSet.Activate();
        teleportUp.AddOnStateDownListener(onNorthDown, rightHand);
        teleportDown.AddOnStateDownListener(onSouthDown, rightHand);
    }

    public void Init() {
        leftController.SetActive(true);
        rightController.SetActive(true);
    }

    void Update()
    {
    }

    void onNorthDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        isMirror = !isMirror;
        rearMirror.SetActive(isMirror);
    }

    void onSouthDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        isLens = !isLens;
        realityLens.SetActive(isLens);
    }
}