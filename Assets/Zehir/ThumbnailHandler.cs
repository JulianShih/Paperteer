using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// namespace RecordAndRepeat
// {
public class ThumbnailHandler : MonoBehaviour
{
    public int index;
    public Text text;
    public PadManager padManager;
    public RawImage rawImage;
    public bool hasimage = false;

    public void Init()
    {
        print("ThumbnailHandler.Init " + index);
        padManager = GameObject.Find("UI (pad)").GetComponent<PadManager>();
        GetComponent<Button>().onClick.AddListener(onThumbnailClicked);
        rawImage = GetComponentInChildren<RawImage>();
        text = GetComponentInChildren<Text>();
        updateIndex();
        updateImg();
    }

    public void onThumbnailClicked() {
        Debug.Log("ThumbnailHandler.onThumbnailClicked " + index);
        padManager.fadingEffectManager.StartFadingEffect();
        padManager.switchAct(index);
    }

    public void updateIndex() {
        text.text = index.ToString();
    }

    public void updateImg() {
        if(hasimage) return;
        Texture2D img = null;
        byte[] fileData = null;
        string path = "Assets/Resources/Video/p" + name + ".png";  
        try {
            fileData = System.IO.File.ReadAllBytes(path);
            img = new Texture2D(2, 2);
            img.LoadImage(fileData);
            rawImage.color = Color.white;
            rawImage.texture = img;
            hasimage = true;
        } catch (System.IO.FileNotFoundException) {
            print("Video not found !");
            hasimage = false;
        }
    }

    public void resetImage() {
        rawImage.color = Color.black;
        rawImage.texture = null;
        hasimage = false;
    }
}
// }
