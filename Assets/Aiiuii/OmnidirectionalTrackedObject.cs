using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Valve.VR;

public class OmnidirectionalTrackedObject : MonoBehaviour
{
    public OmnidirectionalObjectData data;
    public OmnidirectionalObjectCapturer capturer;
    public Transform cameraAnchor;
    Vector3 localPos;
    Quaternion localRot;
    GameObject trackerAnchor;
    public GameObject trackerModel;

    // Start is called before the first frame update
    public void Init()
    {
        Debug.Log("OmnidirectionalTrackedObject.Init");
        var pose = GetComponent<SteamVR_Behaviour_Pose>();
        if(pose !=  null) {
            pose.inputSource = data.inputSource;
            cameraAnchor = GameObject.Find("Anchor (360)").transform;
        }
        // data.Tracker = gameObject;
        data.CroppedTexture = new RenderTexture(data.rect.width, data.rect.height, 24, GraphicsFormat.R8G8B8A8_UNorm);
        Transform quad = transform.Find("Quad");
        // localPos = quad.localPosition * 10 ;
        // localRot = quad.localRotation;
    }
    
    public void Start() {
        trackerAnchor = (GameObject)Instantiate(trackerModel, transform.position, transform.rotation, transform.parent);
        trackerAnchor.name = data.name + "_rescale";
        Transform quad = transform.Find("Quad");
        quad.SetParent(trackerAnchor.transform, false);
        // quad.localPosition = localPos;
        // quad.rotation = localRot;
    }

    public void Update() {
        Vector3 anchor = (transform.position - cameraAnchor.position) * 10 + cameraAnchor.position;
        trackerAnchor.transform.position = anchor;
        trackerAnchor.transform.rotation = transform.rotation;
    }
}