using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cleanup : MonoBehaviour
{
    public ActData actData;
    public List<OmnidirectionalObjectData> oObjList;
    public List<VirtualObjectData> vObjList;
    public int count;

    [ContextMenu("ProtoVR/Clean")]
    void Clean() {
        if(count > 0) {
            actData.id = -1;
            actData.actIDList = new List<int>();
            actData.actIDList.Add(-1);
            actData.actSkyboxList = new List<string>();
            for(int i=0; i<count; i++) actData.actSkyboxList.Add("");

            foreach(OmnidirectionalObjectData oObjData in oObjList) {
                oObjData.modelList = new List<string>();
                for(int i=0; i<count; i++) oObjData.modelList.Add("");
            }

            foreach(VirtualObjectData vObjData in vObjList) {
                vObjData.positionList = new List<Vector3>();
                vObjData.rotationList = new List<Quaternion>();
                vObjData.parentList = new List<string>();
                for(int i=0; i<count; i++) {
                    vObjData.positionList.Add(Vector3.zero);
                    vObjData.rotationList.Add(Quaternion.identity);
                    vObjData.parentList.Add("");
                }
            }
            return;
        }

        actData.id = -1;
        actData.actIDList = new List<int>();
        actData.actIDList.Add(-1);
        actData.actSkyboxList = new List<string>();
        actData.actSkyboxList.Add("");

        foreach(OmnidirectionalObjectData oObjData in oObjList) {
            oObjData.modelList = new List<string>();
            oObjData.modelList.Add("");
        }

        foreach(VirtualObjectData vObjData in vObjList) {
            vObjData.positionList = new List<Vector3>();
            vObjData.positionList.Add(Vector3.zero);
            vObjData.rotationList = new List<Quaternion>();
            vObjData.rotationList.Add(Quaternion.identity);
            vObjData.parentList = new List<string>();
            vObjData.parentList.Add("");
        }

        string[] afolders = {"Assets/Resources/Act"};
        foreach (var asset in AssetDatabase.FindAssets("", afolders))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        string[] rfolders = {"Assets/Resources/Recording"};
        foreach (var asset in AssetDatabase.FindAssets("", rfolders))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        string[] vfolders = {"Assets/Resources/Video"};
        foreach (var asset in AssetDatabase.FindAssets("", vfolders))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
    }
}