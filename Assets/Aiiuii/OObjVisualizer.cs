using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class OObjVisualizer : MonoBehaviour
{
    public OmnidirectionalObjectData omniObjData; 
    public MeshRenderer meshRenderer;
    public OmnidirectionalObjectCapturer capturer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        capturer = GetComponent<OmnidirectionalObjectCapturer>();
    }


    // Update is called once per frame
    void Update()
    {
        omniObjData = capturer.omniObjDataList[capturer.omniObjDataList.Count - 1];
        var tracker = omniObjData.Tracker;
        var normals = new List<Vector4>();
        
        foreach (var offset in omniObjData.trackerOffset)
        {
            var pos = tracker.transform.TransformPoint(offset);
            var dir = pos - transform.position;
            normals.Add(transform.InverseTransformDirection(dir));
            // RaycastHit outInfo;
            Debug.DrawRay(pos, -dir, Color.green);
            // if (Physics.Raycast(pos, -dir, out outInfo))
            // {
            //     Debug.DrawRay(outInfo.point, outInfo.normal, Color.red);
            //     normals.Add(outInfo.normal);
            // //     Debug.Log(outInfo.normal);
            // }
        }

        if (normals.Count == 4)
        {
            meshRenderer.material.SetVectorArray("_normals", normals);
        }
    }
}
