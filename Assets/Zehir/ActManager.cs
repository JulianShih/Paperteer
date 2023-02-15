using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class ActManager : MonoBehaviour
{
    public GameObject acts;
    public ActData actData;
    public StageManager stageManager;
    public GameObject actPrefab;
    public int currentActIndex = 0;
    public PadManager padManager;
    public TrackedObjectManager trackedObjectManager;
    public VirtualEditManager virtualEditManager;
    public List<GameObject> actList = new List<GameObject>();
    public OmnidirectionalObjectCapturer playbackCapturer;
    public Transform cameraAnchor;

    void Start()
    {
        generateActs();
        padManager.generateThumbnnails();
    }

    public void generateActs() {
        print("ActManager.generateActs");
        int index = 0;
        foreach(int id in actData.actIDList) {
            createAct(index, id);
            index++;
        }
    }

    public void createAct(int index, int id) {
        GameObject newAct = (GameObject)Instantiate(actPrefab, this.transform);
        newAct.name = id.ToString();
        actList.Insert(index, newAct);
        newAct.SetActive(false);
    }

    public void createObjects() {
        int index = 0;
        foreach(GameObject gObj in actList) {
            initAct(gObj, index);
            index++;
        }
    }

    public void initAct(GameObject act, int index) {
        ActHandler actHandler = act.GetComponent<ActHandler>();
        actHandler.trackedObjectList = trackedObjectManager.trackedObjectList;
        actHandler.playbackCapturer = playbackCapturer;
        actHandler.cameraAnchor = cameraAnchor;
        actHandler.Init(index);
    }

    public void destroyAct(int index) {
        Debug.Log("ActManager.destroyAct");
        Destroy(actList[index]);
        actList.RemoveAt(index);
    }

    public void addAct() {
        Debug.Log("ActManager.addAct " + (currentActIndex + 1));
        int index = currentActIndex + 1;
        actData.actIDList.Insert(index, -1);
        actData.actSkyboxList.Insert(index, "");
        createAct(index, -1);
        padManager.createThumbnail(index, -1);
        foreach(GameObject tObj in trackedObjectManager.trackedObjectList) {
            tObj.GetComponentInChildren<OmnidirectionalTrackedObject>().data.modelList.Insert(index, "");
        }
        foreach(VirtualObjectData vObj in trackedObjectManager.vobjDataList) {
            vObj.positionList.Insert(index, Vector3.zero);
            vObj.rotationList.Insert(index, Quaternion.identity);
            vObj.parentList.Insert(index, "");
        }
    }

    public void deleteAct(int targetIndex) {
        Debug.Log("ActManager.deleteAct " + targetIndex);
        if(targetIndex == 0 && actList.Count == 1) {
            // restore act
            foreach (Transform child in actList[targetIndex].transform) {
                GameObject.Destroy(child.gameObject);
            }
            actData.actIDList[targetIndex] = -1;
            actData.actSkyboxList[targetIndex] = "";
            actList[targetIndex].name = "-1";
            actList[targetIndex].GetComponent<ActHandler>().hasVideo = false;
            foreach(GameObject tObj in trackedObjectManager.trackedObjectList) {
                tObj.GetComponentInChildren<OmnidirectionalTrackedObject>().data.modelList[targetIndex] = "";
            }
            foreach(VirtualObjectData vObj in trackedObjectManager.vobjDataList) {
                vObj.positionList[targetIndex] = Vector3.zero;
                vObj.rotationList[targetIndex] = Quaternion.identity;
                vObj.parentList[targetIndex] = "";
            }
        } else {
            if(targetIndex == 0) currentActIndex = 0;
            else currentActIndex = targetIndex - 1;

            destroyAct(targetIndex);
            actData.actIDList.RemoveAt(targetIndex);
            actData.actSkyboxList.RemoveAt(targetIndex);
            foreach(GameObject tObj in trackedObjectManager.trackedObjectList) {
                tObj.GetComponentInChildren<OmnidirectionalTrackedObject>().data.modelList.RemoveAt(targetIndex);
            }
            foreach(VirtualObjectData vObj in trackedObjectManager.vobjDataList) {
                vObj.positionList.RemoveAt(targetIndex);
                vObj.rotationList.RemoveAt(targetIndex);
                vObj.parentList.RemoveAt(targetIndex);
            }
            padManager.destroyThumbnail(targetIndex);
            padManager.reorderThumbnail();
        }
        padManager.switchAct(currentActIndex);
        padManager.addButton.transform.SetSiblingIndex(padManager.addButton.transform.GetSiblingIndex() + 1);
    }

    public void switchAct(int targetIndex) {
        Debug.Log("ActManager.switchAct " + targetIndex);
        if(targetIndex < 0 || targetIndex > actList.Count - 1) return;
        if(actList[currentActIndex])
            actList[currentActIndex].SetActive(false);
        actList[targetIndex].SetActive(true);
        currentActIndex = targetIndex;
        padManager.togglePrevNext();
        stageManager.toggleStage(actList[currentActIndex].GetComponent<ActHandler>().checkVideo());
    }

    public static void swap<T> (List<T> list, int s, int t) {
        T tmp = list[s];
        list[s] = list[t];
        list[t] = tmp;
    }

    public void swapAct(int sourceIndex, int targetIndex) {
        Debug.Log("ActManager.swapAct " + sourceIndex + " " + targetIndex);
        swap(actData.actIDList, sourceIndex, targetIndex);
        swap(actList, sourceIndex, targetIndex);
        currentActIndex = targetIndex;
    }

    public void switchToPrevAct() {
        if(currentActIndex == 0) return;
        switchAct(currentActIndex - 1);
    }

    public void switchToNextAct() {
        if(currentActIndex == actData.actIDList.Count - 1) return;
        switchAct(currentActIndex + 1);
    }
}