using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsMonitor : MonoBehaviour
{
    public float fps;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fps = 1 / Time.deltaTime;
    }
}
