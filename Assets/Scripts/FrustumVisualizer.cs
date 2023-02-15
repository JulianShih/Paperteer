using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This scripts draws a frustum of the avatar
/// with near plane, far plane and also the vertical and horizontal fov.
/// </summary>

public class FrustumVisualizer : MonoBehaviour
{
    public Color color = Color.white;
    public float thickness = 0.002f;
    public float vFov = 60;
    public float hFov = 80;
    public float near = 0.05f;
    public float far = 0.4f;
    LineRenderer line;
    Vector3[] vertices;
    
    void Start()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        Material mat = new Material(shader) {color = this.color};
        line = this.gameObject.AddComponent<LineRenderer>().GetComponent<LineRenderer>();
        line.material = mat;
        line.startWidth = this.thickness;
        line.endWidth = this.thickness;
    }
    
    void Update()
    {
        far = Mathf.Clamp(far, 0, 1);
        near = Mathf.Clamp(near, 0, far);

        float nearWidth = Mathf.Tan(hFov * Mathf.Deg2Rad / 2) * near;
        float nearHeight = Mathf.Tan(vFov * Mathf.Deg2Rad / 2) * near;
        float farWidth = Mathf.Tan(hFov * Mathf.Deg2Rad / 2) * far;
        float farHeight = Mathf.Tan(vFov * Mathf.Deg2Rad / 2) * far;
        
        Vector3 origin = this.transform.position;
        Vector3 nearTopLeft = origin + near * this.transform.forward - nearWidth * this.transform.right - nearHeight * this.transform.up;
        Vector3 nearTopRight = origin + near * this.transform.forward + nearWidth * this.transform.right - nearHeight * this.transform.up;
        Vector3 nearBottomLeft = origin + near * this.transform.forward - nearWidth * this.transform.right + nearHeight * this.transform.up;
        Vector3 nearBottomRight = origin + near * this.transform.forward + nearWidth * this.transform.right + nearHeight * this.transform.up;
        Vector3 farTopLeft = origin + far * this.transform.forward - farWidth * this.transform.right - farHeight * this.transform.up;
        Vector3 farTopRight = origin + far * this.transform.forward + farWidth * this.transform.right - farHeight * this.transform.up;
        Vector3 farBottomLeft = origin + far * this.transform.forward - farWidth * this.transform.right + farHeight * this.transform.up;
        Vector3 farBottomRight = origin + far * this.transform.forward + farWidth * this.transform.right + farHeight * this.transform.up;

        vertices = new Vector3[17];
        // Near
        vertices[0] = nearTopLeft;
        vertices[1] = nearTopRight;
        vertices[2] = nearBottomRight;
        vertices[3] = nearBottomLeft;
        vertices[4] = nearTopLeft;
        // Top
        vertices[5] = farTopLeft;
        vertices[6] = farTopRight;
        vertices[7] = nearTopRight;
        // Right
        vertices[8] = farTopRight;
        vertices[9] = farBottomRight;
        vertices[10] = nearBottomRight;
        // Bottom
        vertices[11] = farBottomRight;
        vertices[12] = farBottomLeft;
        vertices[13] = nearBottomLeft;
        // Left
        vertices[14] = farBottomLeft;
        vertices[15] = farTopLeft;
        vertices[16] = nearTopLeft;

        line.positionCount = vertices.Length;
        line.SetPositions(vertices);
    }
}
