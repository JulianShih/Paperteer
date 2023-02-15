using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(MeshRenderer))]
public class OmnidirectionalObjectCapturer : MonoBehaviour
{
    public RenderTexture source;
    public Vector2 tilling;
    public Vector2 offset;
    public List<OmnidirectionalObjectData> omniObjDataList = new List<OmnidirectionalObjectData>();
    public List<GameObject> omniObjList = new List<GameObject>();
    MeshRenderer meshRenderer;
    Texture2D dest;
    public Material material;
    public List<MeshRenderer> textureList = new List<MeshRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        // meshRenderer = GetComponent<MeshRenderer>();
        // material = new Material(Shader.Find("Custom/CropOmnidirectionalObject"));
        
        // source = meshRenderer.material.mainTexture as RenderTexture;
        // dest = new Texture2D(source.width, source.height,
        //     GraphicsFormat.R8G8B8A8_UNorm,
        //     TextureCreationFlags.None);

        // meshRenderer.material.mainTexture = dest;
        // temp.material = new Material(Shader.Find("Standard"));
    }


    // Update is called once per frame
    void Update()
    {
        if (source == null) return;

        int i=0;
        foreach (var omniObj in omniObjList)
        {
            var data = omniObj.GetComponent<OmnidirectionalTrackedObject>().data;
            var normals = new List<Vector4>();
            var tracker = omniObj;

            foreach (var offset in data.trackerOffset)
            {
                var pos = tracker.transform.TransformPoint(offset);
                var dir = pos - transform.position;
                // RaycastHit outInfo;
                // if (Physics.Raycast(pos, dir, out outInfo))
                // {
                //     normals.Add(-outInfo.normal);
                // }
                Debug.DrawRay(pos, dir, Color.red);

                normals.Add(transform.InverseTransformDirection(dir));
            }

            if (normals.Count == 4)
            {
                // Debug.Log("Blit");
                material.SetVectorArray("_normals", normals);
                material.SetVector("_scaleAndOffset", new Vector4(tilling.x, tilling.y, offset.x, offset.y));
                // material.SetTextureScale("_MainTex", tilling);
                // material.SetTextureOffset("_MainTex", offset);
                Graphics.Blit(source, data.CroppedTexture, material);
                textureList[i++].material.mainTexture = data.CroppedTexture;
                // data.Tracker.
            }
            // position = (recordClass.position - cameraAnchor.position) * 10 + cameraAnchor.position;
        }
    }

    int Repeat(int value, int modulo)
    {
        int ret = value;
        if (value < 0) ret = modulo - (-value % modulo);
        else if (value > modulo) ret = value % modulo;
        return ret;
    }

    void VisualizeWithSetPixels()
    {
        var oldTex = RenderTexture.active;
        RenderTexture.active = source;
        dest.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        for (int id = 0; id < omniObjDataList.Count; id++)
        {
            var omniObjData = omniObjDataList[id];
            var offset = omniObjData.trackerOffset;
            var rect = omniObjData.rect;
            var color = omniObjData.color;
            var lineSize = omniObjData.lineSize;
            var tracker = omniObjData.Tracker;
            var dir = (tracker.transform.position - transform.position).normalized;

            RaycastHit hit;
            if (!Physics.Raycast(tracker.transform.position, dir, out hit))
                // if (!Physics.Raycast(trackers[id].transform.position + trackers[id].transform.TransformPoint(offset), dir, out hit))
                // if (!Physics.Raycast(trackers[id].transform.TransformPoint(trackers[id].transform.localPosition + offset), dir, out hit))
                continue;

            Debug.DrawRay(tracker.transform.position, dir, color);

            Vector2Int pixelUV = new Vector2Int((int)(hit.textureCoord.x * source.width), (int)(hit.textureCoord.y * source.height));

            var objRect = new RectInt(pixelUV + rect.position - rect.size / 2, rect.size);
            for (int i = objRect.xMin; i <= objRect.xMax; i++)
            {
                int curX = Repeat(i, source.width);

                for (int j = 0; j < lineSize; j++)
                {
                    dest.SetPixel(curX, Mathf.Clamp(objRect.yMin + j, 0, source.height), color);
                    dest.SetPixel(curX, Mathf.Clamp(objRect.yMax - j, 0, source.height), color);
                }
            }

            for (int i = objRect.yMin; i <= objRect.yMax; i++)
            {
                int curY = Mathf.Clamp(i, 0, source.height);

                for (int j = 0; j < lineSize; j++)
                {
                    dest.SetPixel(Repeat(objRect.xMin + j, source.width), curY, color);
                    dest.SetPixel(Repeat(objRect.xMax - j, source.width), curY, color);
                }
            }

            // var tex = planes[map[trackers[id]]].material.mainTexture as Texture2D;
            // tex.ReadPixels(new Rect((int)pixelUV.x + objRect.x - objRect.width / 2, renderTexture.height - (int)pixelUV.y - objRect.y - objRect.height / 2, objRect.width, objRect.height), 0, 0);
            // tex.Apply();

        }
        dest.Apply();
        RenderTexture.active = oldTex;
    }
}