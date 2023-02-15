using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveSR_SetCameraPose : MonoBehaviour {
    private GameObject render_pose;
    public float offset_x;
    public float offset_y;
    public Vector3 camera_pose;

    public Transform anchor360;
    public Transform anchorCam;
    // public Transform eye;
    // public Transform camRig;
    public StageManager stageManager;

	// Update is called once per frame
	void Update () {
        if(stageManager.currentStage) {
            fix(anchorCam);
        } else {
            fix(anchor360);
        }
    }

    public void fix(Transform target) {
        Vector3 offset = Vive.Plugin.SR.ViveSR_DualCameraRig.Instance.DualCameraLeft.transform.position - transform.position;
        transform.position = target.position - offset;
    }
}

    