using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// // [RequireComponent(typeof(Camera))]
public class FollowTrackerCam : MonoBehaviour
{
    public GameObject target;
    public List<GameObject> trackers;
    //     public Klak.Spout.SpoutReceiver spoutReceiver;
    //     public GameObject hmdset;
    //     public Vector3 offset;
    //     public Vector3 targetOffset;
    //     // Start is called before the first frame update
    //     void Start()
    //     {

    //     }

    //     // Update is called once per frame
    //     void Update()
    //     {
    //         //Make sure the target and this object don't have the same position. This can happen before the cameras are initialized.
    //         //Calling Quaternion.LookRotation in this case spams the console with errors. 
    //         // // if (target == null  || (transform.position - target.transform.position == Vector3.zero))
    //         // {
    //         //     return;
    //         // }

    //         // transform.eulerAngles = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles + offset;    
    //         // // target.transform.eulerAngles = Quaternion.LookRotation(transform.position - hmdset.transform.position).eulerAngles + Vector3.one * 90f;    
    //         // target.transform.LookAt(hmdset.transform, Vector3.up);
    //         // target.transform.eulerAngles += targetOffset;

    //         var vec = target.transform.position - transform.position;
    //         RaycastHit hit;
    //         if (!Physics.Raycast(transform.position, vec, out hit))
    //             return;

    //         Debug.DrawRay(transform.position, vec, Color.red);

    //         Renderer rend = hit.transform.GetComponent<Renderer>();
    //         MeshCollider meshCollider = hit.collider as MeshCollider;

    //         if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
    //             return;

    //         var mainTex = rend.material.mainTexture as RenderTexture;
    //         var old_tex = RenderTexture.active;
    //         RenderTexture.active = mainTex;

    //         Texture2D tex2D = new Texture2D(mainTex.width, mainTex.height, TextureFormat.RGB24, false);
    //         // ReadPixels looks at the active RenderTexture.
    //         tex2D.ReadPixels(new Rect(0, 0, tex2D.width, tex2D.height), 0, 0);
    //         Vector2 pixelUV = hit.textureCoord;
    //         pixelUV.x *= mainTex.width;
    //         pixelUV.y *= mainTex.height;



    //         tex2D.SetPixels((int)pixelUV.x, (int)pixelUV.y, 2, 2, new Color[]{Color.red, Color.red, Color.red, Color.red});

    //         tex2D.Apply();
    //         RenderTexture.active = old_tex;
    //     }
    // }
    public RenderTexture renderTexture; // renderTextuer that you will be rendering stuff on
    public Renderer targetRenderer; // renderer in which you will apply changed texture
    public bool useSpoutRT;
    [Range(0, 200)]
    public int outRange;
    [Range(0, 100)]
    public int inRange;
    public Dictionary<GameObject, Color> colors = new Dictionary<GameObject, Color>();
    Texture2D texture;
    public List<MeshRenderer> planes;
    public Dictionary<GameObject, int> map = new Dictionary<GameObject, int>();
    List<Texture2D> textures = new List<Texture2D>();

    public Vector3 stickOffset;
    public RectInt stickRect;
    public Vector3 backgroundBoardOffset;
    public RectInt backgroundBoardRect;

    void Start()
    {

        texture = new Texture2D(renderTexture.width, renderTexture.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        // targetRenderer.material.mainTexture = texture;
        //make texture2D because you want to "edit" it. 
        //however this is not a way to apply any post rendering effects because
        //this way, you are reading it through CPU(slow).
        colors[trackers[0]] = Color.red;
        colors[trackers[1]] = Color.blue;
        map[trackers[0]] = 0;
        map[trackers[1]] = 1;

        var tex = new Texture2D(stickRect.width, stickRect.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        textures.Add(tex);
        tex = new Texture2D(backgroundBoardRect.width, backgroundBoardRect.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        textures.Add(tex);
        planes[map[trackers[0]]].material = new Material(Shader.Find("Diffuse"));
        planes[map[trackers[0]]].material.mainTexture = textures[0];
        planes[map[trackers[1]]].material = new Material(Shader.Find("Diffuse"));
        planes[map[trackers[1]]].material.mainTexture = textures[1];


    }

    void Update()
    {
        if (useSpoutRT)
        {
            targetRenderer.material.mainTexture = renderTexture;
            return;
        }
        targetRenderer.material.mainTexture = texture;

        var dirs = new List<Vector3>();
        for (int id = 0; id < trackers.Count; id++)
        {
            var offset = map[trackers[id]] == 0 ? stickOffset : backgroundBoardOffset;
            // dirs.Add(target.transform.position - trackers[id].transform.TransformPoint(trackers[id].transform.localPosition + offset));
            // dirs.Add(target.transform.position - trackers[id].transform.position - trackers[id].transform.TransformPoint(offset));
            dirs.Add(target.transform.position - trackers[id].transform.position);
        }
        dirs.Sort((x, y) => -x.sqrMagnitude.CompareTo(y.sqrMagnitude));
        trackers.Sort((x, y) => -(target.transform.position - x.transform.position).sqrMagnitude.CompareTo((target.transform.position - y.transform.position).sqrMagnitude));

        var oldTex = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        for (int id = 0; id < trackers.Count; id++)
        {
            var offset = map[trackers[id]] == 0 ? stickOffset : backgroundBoardOffset;
            var objRect = map[trackers[id]] == 0 ? stickRect : backgroundBoardRect;
            var dir = dirs[id];
            var tracker = trackers[id];

            RaycastHit hit;
            if (!Physics.Raycast(trackers[id].transform.position, dir, out hit))
            // if (!Physics.Raycast(trackers[id].transform.position + trackers[id].transform.TransformPoint(offset), dir, out hit))
            // if (!Physics.Raycast(trackers[id].transform.TransformPoint(trackers[id].transform.localPosition + offset), dir, out hit))
                continue;
            // Debug.DrawRay(trackers[id].transform.TransformPoint(trackers[id].transform.localPosition + offset), dir, colors[tracker]);
            Debug.DrawRay(trackers[id].transform.position, dir, colors[tracker]);

            //don't forget that you need to specify rendertexture before you call readpixels
            //otherwise it will read screen pixels.

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= renderTexture.width;
            pixelUV.y *= renderTexture.height;
            for (int i = (int)pixelUV.x + objRect.x - objRect.width / 2; i <= (int)pixelUV.x + objRect.x + objRect.width / 2; i++)
            // for (int i = (int)pixelUV.x - outRange / 2; i <= (int)pixelUV.x + outRange / 2; i++)
            {
                int curX = i;
                if (i < 0) curX = renderTexture.width - (-i % renderTexture.width);
                else if (i > renderTexture.width) curX = i % renderTexture.width;

                for (int j = 0; j < inRange; j++)
                {
                    texture.SetPixel(curX, (int)pixelUV.y + objRect.y - objRect.height / 2 + j, colors[tracker]);
                    texture.SetPixel(curX, (int)pixelUV.y + objRect.y + objRect.height / 2 - j, colors[tracker]);
                }
            }
            for (int i = (int)pixelUV.y + objRect.y - objRect.height / 2; i <= (int)pixelUV.y + objRect.y + objRect.height / 2; i++)
            // for (int i = (int)pixelUV.y + objRect.y - objRect.height / 2; i <= (int)pixelUV.y + objRect.y + objRect.height / 2; i++)
            // for (int i = (int)pixelUV.y - outRange / 2; i <= (int)pixelUV.y + outRange / 2; i++)
            {
                int curY = i;
                if (i < 0) curY = renderTexture.height - (-i % renderTexture.height);
                else if (i > renderTexture.height) curY = i % renderTexture.height;

                for (int j = 0; j < inRange; j++)
                {
                    texture.SetPixel((int)pixelUV.x + objRect.x - objRect.width / 2 + j, curY, colors[tracker]);
                    texture.SetPixel((int)pixelUV.x + objRect.x + objRect.width / 2 - j, curY, colors[tracker]);
                }
            }

            var tex = planes[map[trackers[id]]].material.mainTexture as Texture2D;
            tex.ReadPixels(new Rect((int)pixelUV.x + objRect.x - objRect.width / 2, renderTexture.height - (int)pixelUV.y - objRect.y - objRect.height / 2, objRect.width, objRect.height), 0, 0);
            tex.Apply();

        }
        texture.Apply();
        RenderTexture.active = oldTex;
    }
}