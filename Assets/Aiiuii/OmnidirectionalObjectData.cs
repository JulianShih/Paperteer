using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[CreateAssetMenu(fileName = "OmnidirectionalObjectData", menuName = "ProtoVR/OmnidirectionalObjectData", order = 0)]
public class OmnidirectionalObjectData : ScriptableObject
{
    public SteamVR_Input_Sources inputSource;
    public string characterName;
    // public int deviceIndex;
    // public string serialNumber;
    public Vector3[] trackerOffset;

    #region Visualize Debug info on Omnidirectional View
    public RectInt rect;
    public Color color;
    public int lineSize;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public RenderTexture CroppedTexture { get; set; }
    public GameObject Tracker { get; set; }
    public List<string> modelList = new List<string>();
    #endregion
}