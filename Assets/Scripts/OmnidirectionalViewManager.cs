using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep the state of the system (i.e. Mode).
/// Place the omnidirectional view at the lens of the camera,
/// and fix the rotational offset while the camera is being moved.
/// </summary>

public class OmnidirectionalViewManager : MonoBehaviour
{
    public Transform avatarAnchor; // Position reference for where the view should be placed.
    public Transform tracker; // Rotation reference for where the view is orienting.
    const float orientationOffset = -90; // The offset between the 360 camera and the tracker.

    void Update()
    {
        this.transform.position = avatarAnchor.position;
        this.transform.localEulerAngles = Vector3.up * (tracker.localEulerAngles.y + orientationOffset);
    }
}
