using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace RecordAndRepeat
{
public class TrackerInput : MonoBehaviour
{
    // Start is called before the first frame update
    // public SteamVR_Behaviour_Pose trackerRole;
    public PadManager padManager;
    public SteamVR_Input_Sources rightFoot;
    private SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;
    private SteamVR_Action_Boolean grip = SteamVR_Actions.default_GrabGrip;
    public bool isTrigger;
    public bool isGrip;


    void Start() {
        trigger.AddOnStateDownListener(onTrigger, rightFoot);
        grip.AddOnStateDownListener(onGrip, rightFoot);
    }
    public void onTrigger(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource){
    }

    public void onGrip(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource){

    }
}
}
