using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateManager : MonoBehaviour
{
    public bool isRotating = false;
    // Start is called before the first frame update
    public float speed = 0.1f;
    float x;
    float y;
    public Transform camRigcam;
    public Vector3 lastMousePos;
    public bool isDragging = false;

    void Start() {
    }

    void Update()
    {
        transform.position = camRigcam.position;
        if(isRotating) {
            if (Input.GetMouseButtonDown(0)) {
                isDragging = true;
                lastMousePos = Input.mousePosition;
            }
                
            if (Input.GetMouseButton(0) && isDragging) {
                Vector3 newMousePos = Input.mousePosition;
                Vector3 mousePosDelta = newMousePos - lastMousePos;
                // transform.RotateAround(transform.position, camRigcam.up, -mousePosDelta.x * speed);
                transform.Rotate(mousePosDelta.y * speed, -mousePosDelta.x * speed, camRigcam.up.z);
                lastMousePos = newMousePos;
            }
                
            if(Input.GetMouseButtonUp(0)){
                isDragging = false;
            }
        } else {
            transform.rotation = camRigcam.rotation;
        }
    }
    public void changeTargetCamera() {
        if(isRotating) {
            isRotating = false;
            isDragging = false;
        } else {
            isRotating = true;
        }
    }
}
