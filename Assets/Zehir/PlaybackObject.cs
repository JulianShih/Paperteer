using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackObject : MonoBehaviour
{
    public Transform cameraAnchor;
    Vector3 localPos;
    Quaternion localRot;
    GameObject trackerAnchor;
    public GameObject trackerModel;

    public void Init()
    {
        cameraAnchor = GameObject.Find("Anchor (Camera)").transform;
        Transform quad = transform.Find("Quad");
        localPos = quad.localPosition * 10 ;
        localRot = quad.localRotation;
    }
    
    public void Start() {
        trackerAnchor = (GameObject)Instantiate(trackerModel, transform.position, transform.rotation, transform.parent);
        trackerAnchor.name = transform.name + "_rescale";
        Transform quad = transform.Find("Quad");
        quad.SetParent(trackerAnchor.transform, false);
        quad.localPosition = localPos;
        quad.rotation = localRot;
    }

    public void Update() {
        Vector3 anchor = (transform.position - cameraAnchor.position) * 10 + cameraAnchor.position;
        trackerAnchor.transform.position = anchor;
        trackerAnchor.transform.rotation = transform.rotation;
    }
}