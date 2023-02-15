using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class OCameraViewPortVisualizer : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public List<Vector4> normals;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        normals.Clear();
        var camera = Camera.main;

        normals.Add((camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, camera.farClipPlane)) - 
                    camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, camera.nearClipPlane)))
                    .normalized);
        normals.Add((camera.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, camera.farClipPlane)) - 
                    camera.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, camera.nearClipPlane)))
                    .normalized);
        normals.Add((camera.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, camera.farClipPlane)) - 
                    camera.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, camera.nearClipPlane)))
                    .normalized);
        normals.Add((camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.farClipPlane)) - 
                    camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.nearClipPlane)))
                    .normalized);

        meshRenderer.material.SetVectorArray("_normals", normals);
    }

    Vector3 helper(float a, float b)
    {
        var camera = Camera.main;
        return (new Vector3(a, b, camera.farClipPlane) - new Vector3(a, b, camera.nearClipPlane)).normalized;
    }
}
