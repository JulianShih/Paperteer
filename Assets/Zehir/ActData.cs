using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActData", menuName = "ProtoVR/ActData", order = 0)]
public class ActData : ScriptableObject
{
    #region
    public int id;
    public List<int> actIDList;
    public List<string> actSkyboxList;
    #endregion
}