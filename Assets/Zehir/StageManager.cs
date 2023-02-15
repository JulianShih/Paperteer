using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public bool is3D = false;
    public bool currentStage = false;
    public GameObject recordStage;
    public GameObject playbackStage;
    public ActManager actManager;
    public PlaybackManager playbackManager;
    public PadManager padManager;
    public GameObject rearCamera;
    public Transform tracker360;
    public Transform trackerCamera;

    void Start()
    {
    }

    public void toggleStage(bool b) {
        Debug.Log("StageManager.toggleStage");
        if(b) {
            padManager.toPlaybackStage();
            toPlaybackStage();
        } else {
            padManager.toRecordStage();
            toRecordStage();
        }
    }

    public void toRecordStage() {
        Debug.Log("toRecordStage");
        currentStage = false;
        recordStage.SetActive(true);
        playbackStage.SetActive(false);
    }

    public void toPlaybackStage() {
        Debug.Log("toPlaybackStage");
        currentStage = true;
        recordStage.SetActive(false);
        playbackStage.SetActive(true);
        playbackManager.changeSource();
    }
}
