using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RecordManager : MonoBehaviour
{
    public ActManager actManager;
    public SocketNetwork SocketNetwork;
    public TrackedObjectManager trackedObjectManager;
    public UnityAnimationRecorder cameraRecorder;
    public RenderTexture tex;

    public void startRecording() {
        actManager.actData.id++;
        Debug.Log("RecordManager.startRecording" + actManager.actData.id);
        actManager.actData.actIDList[actManager.currentActIndex] = actManager.actData.id;
        string savePath = "Assets/Resources/Recordings/";
        string fileName = actManager.actData.id.ToString();
        cameraRecorder.savePath = savePath;
        cameraRecorder.fileName = fileName + "_Camera";
        cameraRecorder.StartRecording();
        foreach(GameObject trackedObject in trackedObjectManager.trackedObjectList) {
            UnityAnimationRecorder recorder = trackedObject.GetComponent<UnityAnimationRecorder>();
            recorder.savePath = savePath;
            recorder.fileName = fileName + "_" + trackedObject.name;
            recorder.StartRecording();
        }
        SocketNetwork.SendRecord(fileName);
        saveThumbnail(fileName);
    }

    public void saveRecording() {
        Debug.Log("RecordManager.saveRecording");
        foreach(GameObject trackedObject in trackedObjectManager.trackedObjectList) {
            trackedObject.GetComponent<UnityAnimationRecorder>().StopRecording();
        }
        cameraRecorder.StopRecording();
        SocketNetwork.SendSave();
    }

    public void saveThumbnail(string id) {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = tex;

        Texture2D img = new Texture2D(tex.width, tex.height);
        img.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        img.Apply();
        RenderTexture.active = rt;

        byte[] bytes = img.EncodeToPNG();
        Destroy(img);
        string path = "Assets/Resources/Video/p" + id + ".png";    
        System.IO.File.WriteAllBytes(path, bytes);
    }
}