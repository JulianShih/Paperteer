using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrackedObjectManager : MonoBehaviour
{
    public VirtualEditManager virtualEditManager;
    public GameObject trackedObjectPrefab;
    public List<OmnidirectionalObjectData> objectDataList = new List<OmnidirectionalObjectData>();
    public List<VirtualObjectData> vobjDataList = new List<VirtualObjectData>();
    public List<GameObject> trackedObjectList = new List<GameObject>();
    public OmnidirectionalObjectCapturer capturer;
    public ActManager actManager;
    public PadManager padManager;
    // public Material material;
    public Transform cameraAnchor;

    void Start()
    {
        virtualEditManager.Init(); 
        createTrackedObjects();
        actManager.createObjects();
        padManager.switchAct(0);
    }

    void createTrackedObjects() {
        Debug.Log("TrackedObjectManager.createTrackedObjects");
        foreach(OmnidirectionalObjectData objData in objectDataList) {
            GameObject newTrackedObject = (GameObject)Instantiate(trackedObjectPrefab);

            OmnidirectionalTrackedObject oObj = newTrackedObject.GetComponent<OmnidirectionalTrackedObject>();
            oObj.data = objData;
            oObj.capturer = capturer;
            oObj.Init();
            oObj.data.Tracker = newTrackedObject;
            
            GameObject quad = newTrackedObject.transform.Find("Quad").gameObject;
            MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();

            GameObject outline = quad.transform.Find("Outline").gameObject;
            outline.GetComponent<Renderer>().material.color = oObj.data.color;
            // meshRenderer.material = new Material(Shader.Find("Standard"));
            // meshRenderer.material = material;

            quad.transform.localPosition = oObj.data.position * 10;
            // quad.transform.localRotation = Quaternion.Euler(oObj.data.rotation * 10);
            quad.transform.localScale = oObj.data.scale * 10;

            // capturer.omniObjDataList.Add(oObj.data);
            capturer.omniObjList.Add(oObj.gameObject);
            capturer.textureList.Add(meshRenderer);

            newTrackedObject.name = oObj.data.characterName;
            newTrackedObject.transform.SetParent(this.transform);
            trackedObjectList.Add(newTrackedObject);
        }
    }
}