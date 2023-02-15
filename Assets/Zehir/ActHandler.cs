using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ActHandler : MonoBehaviour
{
    public VirtualEditManager virtualEditManager;
    public TimelineHandler timelineHandler;
    public GameObject playbackObjectPrefab;
    public List<GameObject> trackedObjectList = new List<GameObject>();
    public List<GameObject> playbackObjectList = new List<GameObject>();
    public bool hasVideo = false;
    public VideoClip videoClip;
    public GameObject skybox;
    public OmnidirectionalObjectCapturer playbackCapturer;
    public Transform cameraAnchor;

    public void Start() {
        
    }

    public void Init(int index) {
        Debug.Log("ActHandler.Init " + index);
        timelineHandler = GetComponent<TimelineHandler>();
        virtualEditManager = GameObject.Find("Playback Stage").GetComponent<VirtualEditManager>();
        if(checkVideo()) {
            createPlaybackObjects(index);
            timelineHandler.Init(index);
            // timelineHandler.loadAsset(index);
        }
    }

    public void createPlaybackObjects(int index) {
        skybox = GameObject.Find("Playback View");
        foreach(GameObject trackedObj in trackedObjectList) {
            GameObject newPlaybackObj = (GameObject)Instantiate(playbackObjectPrefab, this.transform);
            OmnidirectionalTrackedObject oObj  = trackedObj.GetComponent<OmnidirectionalTrackedObject>();
            OmnidirectionalTrackedObject pObj  = newPlaybackObj.GetComponent<OmnidirectionalTrackedObject>();
            newPlaybackObj.name = trackedObj.name;
            playbackObjectList.Add(newPlaybackObj);
            pObj.data = oObj.data;
            pObj.capturer = playbackCapturer;
            pObj.cameraAnchor = cameraAnchor;
            pObj.Init();


            Transform quad = newPlaybackObj.transform.Find("Quad");
            MeshRenderer meshRenderer = quad.GetComponent<MeshRenderer>();
            // quad.localPosition = oObj.data.position * 10;
            // quad.localRotation = Quaternion.Euler(oObj.data.rotation);
            // quad.localScale = oObj.data.scale * 10;
            
            // playbackCapturer.omniObjDataList.Add(pObj.data);
            playbackCapturer.omniObjList.Add(pObj.gameObject);
            playbackCapturer.textureList.Add(meshRenderer);
            // ObjectReplayer objectReplayer = newPlaybackObj.GetComponentInChildren<ObjectReplayer>();
            // objectReplayer.omniData = oObj.data;
            // objectReplayer.skybox = skybox;

            string charModelSource = oObj.data.modelList[index];
            // replace Model
            if(charModelSource != "") {
                quad.GetComponent<MeshRenderer>().enabled = false;
                GameObject model = virtualEditManager.findCharacter(charModelSource);
                Transform box = quad.Find("Model");
                GameObject newModel = Object.Instantiate(model, box);
                newModel.name = charModelSource;
            }
        }
    }

    public bool checkVideo() {
        if(hasVideo) return hasVideo;
        videoClip = (VideoClip)Resources.Load("Video/" + this.name);
        if(!videoClip) {
            Debug.Log("Video not found !");
            hasVideo = false;
        } else {
            hasVideo = true;
        }
        return hasVideo;
    }
}
