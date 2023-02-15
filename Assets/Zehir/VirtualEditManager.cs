using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace RecordAndRepeat
// {
public class VirtualEditManager : MonoBehaviour
{
    public ActManager actManager;
    public TrackedObjectManager trackedObjectManager;
    public Object[] character2DList;
    public Object[] object2DList;
    // public Object[] character3DList;
    // public Object[] object3DList;
    public Object[] effectList;
    public Object[] skyboxList;
    // public List<VirtualObjectData> objectDataList = new List<VirtualObjectData>();
    public GameObject playbackSkybox;


    public void Init()
    {
        Debug.Log("VirtualEditManager.Init");
        character2DList = Resources.LoadAll("2D Character", typeof(GameObject));
        object2DList = Resources.LoadAll("2D Object", typeof(GameObject));
        // character3DList = Resources.LoadAll("3D Character", typeof(GameObject));
        // object3DList = Resources.LoadAll("3D Object", typeof(GameObject));
        effectList = Resources.LoadAll("Effect", typeof(GameObject));
        skyboxList = Resources.LoadAll("Skybox", typeof(Material));
    }

    public GameObject findCharacter(string s) {
        foreach(GameObject g in character2DList) {
            if(g.name == s)  return g;
        }
        return null;
    }

    public GameObject findObject(string s) {
        foreach(GameObject g in object2DList) {
            if(g.name == s)  return g;
        }
        return null;
    }
    public GameObject findEffect(string s) {
        foreach(GameObject g in effectList) {
            if(g.name == s)  return g;
        }
        return null;
    }

     public Material findSkybox(string s) {
        foreach(Material mat in skyboxList) {
            if(mat.name == s)  return mat;
        }
        return null;
    }

    public VirtualObjectData findObjectData(string s) {
        foreach(VirtualObjectData d in trackedObjectManager.vobjDataList) {
            if(d.objectName == s)  return d;
        }
        return null;
    }

    public void redoCharModel(Transform selectedCharacter) {
        Debug.Log("redoCharModel");
        // destroy old model
        if(selectedCharacter.childCount > 0){
            Destroy(selectedCharacter.GetChild(0).gameObject);
        }
        GameObject targetObj = selectedCharacter.parent.parent.gameObject;
        targetObj.transform.Find("Quad").GetComponent<MeshRenderer>().enabled = true;
        // ObjectReplayer objRep = targetObj.GetComponentInChildren<ObjectReplayer>();
        // objRep.omniData.modelList[actManager.currentActIndex] = "";
    }

    public void replaceCharModel(string charModelSource, Transform selectedCharacter) {
        // destroy old model
        Debug.Log("replaceCharModel");
        if(selectedCharacter.childCount > 0){
            Destroy(selectedCharacter.GetChild(0).gameObject);
        }
        GameObject targetObj = selectedCharacter.parent.parent.gameObject;
        targetObj.transform.Find("Quad").GetComponent<MeshRenderer>().enabled = false;
        // Debug.Log("replaceModel " + targetObj + " " + charModelSource);
        // ObjectReplayer objRep = targetObj.GetComponentInChildren<ObjectReplayer>();
        // objRep.omniData.modelList[actManager.currentActIndex] = charModelSource;
        GameObject model = findCharacter(charModelSource);
        GameObject newModel = Object.Instantiate(model, selectedCharacter);
        newModel.name = charModelSource;
    }

    public GameObject createVobj(string vobjModelSource, Transform parent, int index) {
        GameObject obj = findObject(vobjModelSource);
        GameObject newObj = Object.Instantiate(obj, parent);
        newObj.name = vobjModelSource;
        VirtualObjectData vd = findObjectData(vobjModelSource);
        if(vd) {
            newObj.transform.position = vd.positionList[index];
            newObj.transform.rotation = vd.rotationList[index];
            if(vd.parentList[index] != "") {
                newObj.transform.SetParent(parent.Find(vd.parentList[index]));
            }
        }
        return newObj;
    }

    public GameObject addVobj(string vobjModelSource) {
        Debug.Log("addVobj " + vobjModelSource);
        GameObject obj = findObject(vobjModelSource);
        GameObject newObj = Object.Instantiate(obj, actManager.actList[actManager.currentActIndex].transform);
        newObj.name = vobjModelSource;
        actManager.actList[actManager.currentActIndex].GetComponent<TimelineHandler>().addObject(newObj);
        updateVobjOrientation(newObj);
        return newObj;
    }

    public void updateVobjOrientation(GameObject vobj) {
        VirtualObjectData vd = findObjectData(vobj.name);
        if(vd) {
            vd.positionList[actManager.currentActIndex] = vobj.transform.position;
            vd.rotationList[actManager.currentActIndex] = vobj.transform.rotation;
        }
    }

    public void deleteVobj(GameObject vobj) {
        Debug.Log("deleteVobj " + vobj.name);
        actManager.actList[actManager.currentActIndex].GetComponent<TimelineHandler>().deleteObject(vobj);
    }

    public GameObject addEffect(string effectSource) {
        Debug.Log("addEffect " + effectSource);
        GameObject effect = findEffect(effectSource);
        GameObject newEffect = Object.Instantiate(effect, actManager.actList[actManager.currentActIndex].transform);
        newEffect.name = effectSource;
        actManager.actList[actManager.currentActIndex].GetComponent<TimelineHandler>().addObject(newEffect);
        return newEffect;
    }

    public void groupObjects(GameObject character, GameObject vobj) {
        Debug.Log("groupObjects " + character + " " + vobj);
        vobj.transform.SetParent(character.transform.parent);
        VirtualObjectData vd = findObjectData(vobj.name);
        vd.parentList[actManager.currentActIndex] = character.name;
    }

    public void replaceSkybox(string s) {
        if(s == "original360") {
            actManager.actData.actSkyboxList[actManager.currentActIndex] = "";
            playbackSkybox.SetActive(true);
        }
        else {
            Material mat = findSkybox(s);
            if(mat) {
                actManager.actData.actSkyboxList[actManager.currentActIndex] = s;
                RenderSettings.skybox = mat;
                playbackSkybox.SetActive(false);
            }
        }
    }

    public void changeSkybox(string s) {
        Material mat = findSkybox(s);
        if(mat) {
            RenderSettings.skybox = mat;
            playbackSkybox.SetActive(false);
        }
    }
}
// }