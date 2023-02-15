using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[CreateAssetMenu(fileName = "VirtualObjectData", menuName = "ProtoVR/VirtualObjectData", order = 0)]
public class VirtualObjectData : ScriptableObject
{
    public string objectName;
    public List<Vector3> positionList = new List<Vector3>();
    public List<Quaternion> rotationList = new List<Quaternion>();
    public List<string> parentList = new List<string>();
}