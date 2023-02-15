using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEditor;

public class PadManager : MonoBehaviour
{
    public Button prevButton;
    public Button nextButton;
    public Button deleteButton;
    public Button toggleButton;
    public Button addButton;
    public Button cropButton;
    public Button rotateButton;
    public Text actText;
    public ActManager actManager;
    public StageManager stageManager;
    public bool isOpened;
    public ScrollRect scrollRect;
    public Sprite openIcon;
    public Sprite closeIcon;
    public GameObject content;
    public GameObject thumbnail;
    // public GameObject thumb;
    // public Button upButton;
    // public Button downButton;
    public List<GameObject> thumbnailList = new List<GameObject>();
    public bool isDebug = true;

    int invisibleInVR = 8;
    int defaultInVR = 0;

    public RotateManager rotateManager;

    // physical
    public GameObject physicalUI;
    public Button recordButton;
    public Button warningButton;
    public Button refreshButton;
    public Text timerText;
    public RecordManager recordManager;
    public bool isRecording;
    private float time;
    public GameObject timer;
    public Sprite recordIcon;
    public Sprite saveIcon;
    public GameObject outline;
    public GameObject recordingUI;

    // virtual
    public GameObject virtualUI;
    public Slider slider;
    public Text startText;
    public Text endText;
    public Button playButton;
    public Button VRButton;
    public UIManager uIManager;
    public PlaybackManager playbackManager;
    public Sprite playIcon;
    public Sprite pauseIcon;
    public GameObject controllerLUI;
    public GameObject controllerRUI;
    public FadingEffectManager fadingEffectManager;
    public TrackedObjectManager trackedObjectManager;

    void Start()
    {
        recordButton.onClick.AddListener(onRecordButtonClicked);
        VRButton.onClick.AddListener(onVRButtonClicked);
        addButton.onClick.AddListener(onAddButtonClicked);
        prevButton.onClick.AddListener(onPrevButtonClicked);
        nextButton.onClick.AddListener(onNextButtonClicked);
        // upButton.onClick.AddListener(onUpButtonClicked);
        // downButton.onClick.AddListener(onDownButtonClicked);
        toggleButton.onClick.AddListener(onToggleButtonClicked);
        deleteButton.onClick.AddListener(onDeleteButtonClicked);
        playButton.onClick.AddListener(onPlayButtonClicked);
        warningButton.onClick.AddListener(onWarningButtonClicked);
        refreshButton.onClick.AddListener(onRefreshButtonClicked);
        cropButton.onClick.AddListener(onCropButtonClicked);
        rotateButton.onClick.AddListener(onRotateButtonClicked);
    }

    void Update() {
        // physical
        if(isRecording == true){
            time += Time.deltaTime;
            timerText.text = parseTime(time);
        }

        // virtual
        if(!playbackManager.playableDirector) return;
        if(playbackManager.playableDirector.state == PlayState.Playing) {
            slider.value = (float)playbackManager.playableDirector.time;
            startText.text = parseTime((float)playbackManager.playableDirector.time);
        }
    }

    public string parseTime(float t) {
        int minutes = Mathf.FloorToInt(t / 60F);
        int seconds = Mathf.FloorToInt(t % 60F);
        int milliseconds = Mathf.FloorToInt((t * 100F) % 100F);
        return minutes.ToString ("00") + ":" + seconds.ToString ("00") + ":" + milliseconds.ToString("00");
    }

    public void generateThumbnnails() {
        print("PadManager.generateThumbnnails");
        int index = 0;
        foreach(GameObject gObj in actManager.actList) {
            createThumbnail(index, int.Parse(gObj.name));
            index++;
        }
        addButton.transform.SetSiblingIndex(actManager.currentActIndex + 1);
        thumbnailList[actManager.currentActIndex].GetComponentInChildren<Outline>().enabled = true;
        scrollRect.gameObject.SetActive(false);
    }

    public void createThumbnail(int index, int id) {
        GameObject newThumbnail = (GameObject)Instantiate(thumbnail, content.transform);
        ThumbnailHandler thumbnailHandler = newThumbnail.GetComponent<ThumbnailHandler>();
        newThumbnail.name = id.ToString();
        newThumbnail.transform.SetSiblingIndex(index);
        thumbnailHandler.index = index;
        thumbnailHandler.Init();
        thumbnailList.Insert(index, newThumbnail);
    }

    public void destroyThumbnail(int a) {
        Destroy(thumbnailList[actManager.currentActIndex]);
        thumbnailList.RemoveAt(actManager.currentActIndex);
    }

    void onToggleButtonClicked() {
        if(!isOpened) {
            isOpened = true;
            toggleButton.GetComponent<Image>().sprite = closeIcon;
            Vector2 vect2 = toggleButton.GetComponent<RectTransform>().anchoredPosition;
            toggleButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(vect2.x + 180, vect2.y);
            scrollRect.gameObject.SetActive(true);
            thumbnailList[actManager.currentActIndex].GetComponent<ThumbnailHandler>().updateImg();
        } else {
            isOpened = false;
            toggleButton.GetComponent<Image>().sprite = openIcon;
            Vector2 vect2 = toggleButton.GetComponent<RectTransform>().anchoredPosition;
            toggleButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(vect2.x - 180, vect2.y);
            scrollRect.gameObject.SetActive(false);
        }
    }

    void onPrevButtonClicked() {
        fadingEffectManager.StartFadingEffect();
        switchAct(actManager.currentActIndex - 1);
    }
    void onNextButtonClicked() {
        fadingEffectManager.StartFadingEffect();
        switchAct(actManager.currentActIndex + 1);
    }

    void onUpButtonClicked() {
        // swapAct(actManager.currentActIndex, actManager.currentActIndex - 1);
    }
    void onDownButtonClicked() {
        // swapAct(actManager.currentActIndex, actManager.currentActIndex + 1);
    }

    void onVRButtonClicked() {
        isDebug = false;
        onCropButtonClicked();
        stageManager.is3D = true;
        controllerLUI.SetActive(true);
        controllerRUI.SetActive(true);
        uIManager.Init();
        this.gameObject.SetActive(false);
    }

    void onRecordButtonClicked() {
        if(!isRecording) {
            isRecording = true;
            recordManager.startRecording();

            if(isOpened) onToggleButtonClicked();
            recordButton.GetComponent<Image>().sprite = saveIcon;
            enableRecordingUI();
        } else {
            isRecording = false;
            recordManager.saveRecording();

            recordButton.GetComponent<Image>().sprite = recordIcon;
            string id = actManager.actData.id.ToString();
            actManager.actList[actManager.currentActIndex].name = id;
            thumbnailList[actManager.currentActIndex].name = id;
            disableRecordingUI();
        }
    }

    void onRefreshButtonClicked() {
        AssetDatabase.Refresh();
        refreshButton.interactable = false;
        stageManager.toPlaybackStage();
        actManager.initAct(actManager.actList[actManager.currentActIndex], actManager.currentActIndex);
        thumbnailList[actManager.currentActIndex].GetComponent<ThumbnailHandler>().updateImg();
        enableRecordUI();
        switchAct(actManager.currentActIndex);
    }

    void onPlayButtonClicked() {
        if(!playbackManager.isPlaying) {
            playbackManager.Play();
            playButton.GetComponent<Image>().sprite = pauseIcon;
        } else {
            playbackManager.Pause();
            playButton.GetComponent<Image>().sprite = playIcon;
        }
    }

    public void onPlayableDirectorStopped() {
        slider.value = (float)playbackManager.playableDirector.time;
        startText.text = parseTime((float)playbackManager.playableDirector.time);
        playButton.GetComponent<Image>().sprite = playIcon;
    }

    void onAddButtonClicked() {
        actManager.addAct();
        reorderThumbnail();
        switchAct(actManager.currentActIndex + 1);
    }

    void onDeleteButtonClicked() {
        warningButton.interactable = true;
    }

    void onWarningButtonClicked() {
        warningButton.interactable = false;
        thumbnailList[actManager.currentActIndex].GetComponent<ThumbnailHandler>().resetImage();
        actManager.deleteAct(actManager.currentActIndex);
    }

    void onCropButtonClicked() {
        if(isDebug == true) {
            isDebug = false;
            foreach(GameObject a in actManager.actList) {
                foreach (Transform g in a.gameObject.GetComponentsInChildren<Transform>()) {
                    g.gameObject.layer = invisibleInVR; 
                }
            }
            foreach (Transform g in trackedObjectManager.gameObject.GetComponentsInChildren<Transform>()) {
                g.gameObject.layer = invisibleInVR; 
            }
        } else {
            isDebug = true;
            foreach(GameObject a in actManager.actList) {
                foreach (Transform g in a.gameObject.GetComponentsInChildren<Transform>()) {
                    g.gameObject.layer = defaultInVR; 
                }
            }
            foreach (Transform g in trackedObjectManager.gameObject.GetComponentsInChildren<Transform>()) {
                g.gameObject.layer = defaultInVR; 
            }
        }
    }

    void onRotateButtonClicked() {
        rotateManager.changeTargetCamera();
    }

    public void reorderThumbnail() {
        Debug.Log("PadManager.reorderThumbnail");
        int index = 0;
        foreach(GameObject thumbnail in thumbnailList) {
            thumbnail.GetComponent<ThumbnailHandler>().index = index;
            thumbnail.GetComponent<ThumbnailHandler>().updateIndex();
            index++;
        }
    }

    public void switchAct(int targetIndex) {
        Debug.Log("PadManager.switchAct " + targetIndex);
        if(targetIndex < 0 || targetIndex > actManager.actList.Count - 1) return;
        actText.text = "Act " + targetIndex;
        thumbnailList[actManager.currentActIndex].GetComponentInChildren<Outline>().enabled = false;
        thumbnailList[targetIndex].GetComponentInChildren<Outline>().enabled = true;
        // thumb.transform.SetParent(thumbnailList[targetIndex].transform, false);
        addButton.transform.SetSiblingIndex(targetIndex + 1);
        actManager.switchAct(targetIndex);
    }

    public void togglePrevNext() {
        int index = actManager.currentActIndex;
        if(index == 0 && actManager.actList.Count == 1) {
            prevButton.interactable = false;
            nextButton.interactable = false;
        } else if (index == 0) {
            prevButton.interactable = false;
            nextButton.interactable = true;
        } else if (index == actManager.actList.Count - 1) {
            prevButton.interactable = true;
            nextButton.interactable = false;
        } else {
            prevButton.interactable = true;
            nextButton.interactable = true;
        }
    }

    public static void swap<T> (List<T> list, int s, int t) {
        T tmp = list[s];
        list[s] = list[t];
        list[t] = tmp;
    }

    public void swapAct(int sourceIndex, int targetIndex) {
        // addButton.transform.SetSiblingIndex(thumbnailList.Count);
        // thumbnailList[sourceIndex].transform.SetSiblingIndex(targetIndex);
        // addButton.transform.SetSiblingIndex(targetIndex + 1);
        // swap(thumbnailList, sourceIndex, targetIndex);
        // actManager.swapAct(sourceIndex, targetIndex);
        // actText.text = "Act " + targetIndex;
        // reorderThumbnail();
    }

    public void enableRecordUI() {
        toggleButton.interactable = true;
        deleteButton.interactable = true;
        recordButton.interactable = true;
        cropButton.interactable = true;
        refreshButton.interactable = false;
    }

    public void enableRecordingUI() {
        recordingUI.SetActive(true);
        prevButton.interactable = false;
        nextButton.interactable = false;
        toggleButton.interactable = false;
        deleteButton.interactable = false;
        cropButton.interactable = false;
    }
    public void disableRecordingUI() {
        recordingUI.SetActive(false);
        time = 0;
        recordButton.interactable = false;
        refreshButton.interactable = true;
    }

    public void toRecordStage() {
        physicalUI.SetActive(true);
        virtualUI.SetActive(false);
    }

    public void toPlaybackStage() {
        physicalUI.SetActive(false);
        virtualUI.SetActive(true);
    }

    public void changeSource() {
        slider.value = (float)playbackManager.playableDirector.time;
        slider.maxValue = (float)playbackManager.playableDirector.duration;
        startText.text = parseTime((float)playbackManager.playableDirector.time);
        endText.text = parseTime((float)playbackManager.playableDirector.duration);
        playButton.GetComponent<Image>().sprite = playIcon;
    }
}