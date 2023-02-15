using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class ControllerInput : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public SteamVR_ActionSet actionSet;
    public SteamVR_Input_Sources leftHand;
    public SteamVR_Input_Sources rightHand;
    public SteamVR_Action_Vector2 leftJoystick = SteamVR_Actions.oculus.LeftJoystick;
    public SteamVR_Action_Boolean rightGrabPinch = SteamVR_Actions.oculus.RightGrabPinch;
    public SteamVR_Action_Boolean rightGrabGrip = SteamVR_Actions.oculus.RightGrabGrip;
    public SteamVR_Action_Boolean buttonA = SteamVR_Actions.oculus.ButtonA;
    public SteamVR_Action_Boolean buttonB = SteamVR_Actions.oculus.ButtonB;
    public SteamVR_Action_Boolean buttonX = SteamVR_Actions.oculus.ButtonX;
    public SteamVR_Action_Boolean buttonY = SteamVR_Actions.oculus.ButtonY;
    public SteamVR_Action_Boolean rightJoystickEast = SteamVR_Actions.oculus.RightJoystickEast;
    public SteamVR_Action_Boolean rightJoystickWest = SteamVR_Actions.oculus.RightJoystickWest;
    public float leftJoystickX;
    public string choosedSkybox;
    public GameObject pointedObj;
    public GameObject selectedCharacter;
    public GameObject selectedVobj;
    public GameObject grabbedObj;
    public float grabDist;
    public Ray ray;
    public GameObject leftController;
    public GameObject rightController;
    public ActManager actManager;
    public UIManager uIManager;
    public HandMenuManager handMenuManager;
    public VirtualEditManager virtualEditManager;
    public CameraManager cameraManager;

    void Start()
    {
        actionSet.Activate();
        laserPointer.PointerClick += pointerClick;
        laserPointer.PointerIn += pointerIn;
        laserPointer.PointerOut += pointerOut;
        laserPointer.PointerUp += pointerUp;
        laserPointer.PointerDown += pointerDown;
        buttonA.AddOnStateDownListener(onButtonADown, rightHand);
        buttonB.AddOnStateDownListener(onButtonBDown, rightHand);
        buttonX.AddOnStateDownListener(onButtonXDown, leftHand);
        buttonY.AddOnStateDownListener(onButtonYDown, leftHand);
        rightGrabGrip.AddOnStateDownListener(onRightGrabGripDown, rightHand);
        // leftJoystick.AddOnAxisListener(onLeftAxisChange, leftHand);
        rightJoystickEast.AddOnStateDownListener(onEastDown, rightHand);
        rightJoystickWest.AddOnStateDownListener(onWestDown, rightHand);
    }

    public void Init() {
        leftController.SetActive(true);
        rightController.SetActive(true);
    }

    void Update()
    {
        if(grabbedObj) {
            grab();
        }
        leftJoystickX = leftJoystick.GetAxis(leftHand).x;
        // rightJoystickX = rightJoystick.GetAxis(rightHand).x;
    }

    // void onRightAxisChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource) {
    //     rightJoystickX = fromAction.GetAxis(fromSource).x;
    //     if(rightJoystickX > 0) {
    //         uIManager.onNextButtonClicked();
    //         selectCharacter(null);
    //         selectVobj(null);
    //     } else if(rightJoystickX < 0) {
    //         uIManager.onPrevButtonClicked();
    //         selectCharacter(null);
    //         selectVobj(null);
    //     }
    // }

    void onButtonBDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        uIManager.onPlayButtonClicked();
    }
    void onButtonADown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        cameraManager.fixCamera = !cameraManager.fixCamera;
    }
    void onButtonYDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        virtualEditManager.groupObjects(selectedCharacter, selectedVobj);
    }
    void onButtonXDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        virtualEditManager.deleteVobj(selectedVobj);
    }

    void onEastDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        selectCharacter(null);
        selectVobj(null);
        uIManager.onNextButtonClicked();
    }
    void onWestDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        selectCharacter(null);
        selectVobj(null);
        uIManager.onPrevButtonClicked();
    }

    void onRightGrabGripDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if(pointedObj != null) {
            // cancel select
            if(pointedObj == selectedCharacter) {
                selectCharacter(null);
            } else if (pointedObj == selectedVobj) {
                selectVobj(null);
            } else {
                if(pointedObj.tag == "Character") {
                    selectCharacter(pointedObj);
                } else if(pointedObj.tag == "Object") {
                    selectVobj(pointedObj);
                }            
            }
        }
    }

    public void pointerClick(object sender, PointerEventArgs e)
    {
        Debug.Log("pointerClick " + e.target.name);
        if(e.target.GetComponent<Button>()) {
            e.target.GetComponent<ButtonHandler>().onButtonClick(e);
        }
        if(e.target.tag == "Tab") {
            // switch tab
            handMenuManager.onTabClicked(e.target);
        }
        // if(e.target.tag == "Icon") {
        //     // click button
        //     if(e.target.name == "Group Button") {
        //         virtualEditManager.groupObjects(selectedCharacter, selectedVobj);
        //     } else if(e.target.name == "Delete Button") {
        //         virtualEditManager.deleteVobj(selectedVobj);
        //     }
        // }
    }

    public void pointerIn(object sender, PointerEventArgs e) {
        // Debug.Log("pointerIn " + e.target.name);
        pointedObj = e.target.gameObject;
        if(e.target.GetComponent<Button>()) {
            e.target.GetComponent<ButtonHandler>().onButtonEnter();
        } else if(e.target.tag == "Character") {
            e.target.GetComponent<MeshRenderer>().enabled = true;
        } else if(e.target.tag == "Object"){
            e.target.Find("Cube").GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void pointerOut(object sender, PointerEventArgs e) {
        // Debug.Log("pointerOut " + e.target.name);
        pointedObj = null;
        if (e.target.GetComponent<Button>()) {
            e.target.GetComponent<ButtonHandler>().onButtonLeave();
        } else if (e.target.tag == "Character" && e.target.gameObject != selectedCharacter) {
            e.target.GetComponent<MeshRenderer>().enabled = false;  
        } else if (e.target.tag == "Object" && e.target.gameObject != selectedVobj) {
            e.target.Find("Cube").GetComponent<MeshRenderer>().enabled = false;  
        }
    }

    public void onCharButtonClick(string charModelSource) {
        if(selectedCharacter) {
            if(charModelSource == "original360")
                virtualEditManager.redoCharModel(selectedCharacter.transform);
            // replace character
            else
                virtualEditManager.replaceCharModel(charModelSource, selectedCharacter.transform);
        }
    }

    public void onObjButtonClick(string vobjModelSource, PointerEventArgs e) {
        grabbedObj = virtualEditManager.addVobj(vobjModelSource);
        grabStart(e, grabbedObj);
    }

    // grab
    public void pointerDown(object sender, PointerEventArgs e)
    {
        Debug.Log("pointerDown " + e.target.name);
        if(e.target.tag == "Object") {
            grabStart(e, e.target.gameObject); 
        }
    }

    public void pointerUp(object sender, PointerEventArgs e)
    {
        // Debug.Log("pointerUp " + e.target.name);
        if(e.target.tag == "Object") {
            grabEnd(); 
        }
    }

    public void grabStart(PointerEventArgs e, GameObject g) {
        Debug.Log("grabStart " + g);
        grabbedObj = g;
        selectVobj(grabbedObj);
        grabDist = e.distance;
    }

    public void grab()
    {
        Ray ray = laserPointer.currentRay;
        Vector3 pos =  ray.origin + (ray.direction * grabDist);
        grabbedObj.transform.position = pos;
        grabbedObj.transform.rotation = rightController.transform.rotation;
    }

    public void grabEnd() {
        Debug.Log("grabEnd");
        virtualEditManager.updateVobjOrientation(grabbedObj);
        selectVobj(null);
        grabbedObj = null;
        grabDist = 0;
    }

    void selectCharacter(GameObject character) {
        if(selectedCharacter)
            selectedCharacter.GetComponent<MeshRenderer>().enabled = false;
        if(character) {
            character.GetComponent<MeshRenderer>().enabled = true;
        }
        selectedCharacter = character;
    }

    void selectVobj(GameObject vobj) {
        if(selectedVobj) {
            selectedVobj.transform.Find("Cube").GetComponent<MeshRenderer>().enabled = false;
        }
        if(vobj) {
            vobj.transform.Find("Cube").GetComponent<MeshRenderer>().enabled = true;
        }
        selectedVobj = vobj;
    }
}