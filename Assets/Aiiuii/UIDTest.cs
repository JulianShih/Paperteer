//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;
using System.Text;

public class UIDTest : MonoBehaviour
{
    [SerializeField]
    string m_serialNumber;
    public string SerialNumber
    {
        get
        {
            return m_serialNumber;
        }

        set
        {
            m_serialNumber = value;

            if (m_serialNumber == "")
                return;

            deviceId = 0;
            for (; deviceId < SteamVR.connected.Length; deviceId++)
                if (m_serialNumber == GetSerialNumber((uint)deviceId))
                    return;

        }
    }

    [Tooltip("If not set, relative to parent")]
    public Transform origin;

    public bool isValid { get; private set; }

    public int deviceId{get;private set;}
    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        if (SerialNumber == "")
            return;

        if (this.deviceId == SteamVR.connected.Length)
            return;

        isValid = false;

        if (poses.Length <= deviceId)
            return;

        if (!poses[deviceId].bDeviceIsConnected)
            return;

        if (!poses[deviceId].bPoseIsValid)
            return;

        isValid = true;

        var pose = new SteamVR_Utils.RigidTransform(poses[deviceId].mDeviceToAbsoluteTracking);

        if (origin != null)
        {
            transform.position = origin.transform.TransformPoint(pose.pos);
            transform.rotation = origin.rotation * pose.rot;
        }
        else
        {
            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }

    SteamVR_Events.Action newPosesAction;

    UIDTest()
    {
        newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
    }

    void OnEnable()
    {
        var render = SteamVR_Render.instance;
        if (render == null)
        {
            enabled = false;
            return;
        }

        newPosesAction.enabled = true;
    }

    void OnDisable()
    {
        newPosesAction.enabled = false;
        isValid = false;
    }

    private void Start()
    {
        if (m_serialNumber == "")
            return;

        deviceId = 0;
        for (; deviceId < SteamVR.connected.Length; deviceId++)
            if (m_serialNumber == GetSerialNumber((uint)deviceId))
                return;
    }

    string GetSerialNumber(uint i)
    {
        ETrackedPropertyError error = new ETrackedPropertyError();
        StringBuilder sb = new StringBuilder();
        OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_SerialNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
        var SerialNumber = sb.ToString();

        return SerialNumber;
    }

    [ContextMenu("LogAllDevicesSerialNumber")]
    public void LogAllDevicesSerialNumber()
    {
        Debug.Log("-----------");
        for (int i = 0; i < SteamVR.connected.Length; ++i)
        {
            ETrackedPropertyError error = new ETrackedPropertyError();
            StringBuilder sb = new StringBuilder();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_SerialNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var SerialNumber = sb.ToString();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ModelNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var ModelNumber = sb.ToString();

            if (SerialNumber.Length > 0 || ModelNumber.Length > 0)
                Debug.Log("Device " + i.ToString() + " = " + SerialNumber + " | " + ModelNumber);
        }
        Debug.Log("-----------");
    }
}

