// MIT License

// Copyright (c) 2018 Felix Lange 

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public bool fixCamera = true;
    public Transform anchor360;
    public Transform anchorCam;
    public Transform eye;
    public StageManager stageManager;

    void Update() {
        if(!fixCamera) return;
        if(stageManager.currentStage) {
            fix(anchorCam);
        } else {
            fix(anchor360);
        }
    }

    public void fix(Transform target) {
        Vector3 offset = eye.position - transform.position;
        transform.position = target.position - offset;
    }
}