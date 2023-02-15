using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public Slider slider;
    public Text actText;
    public Text countText;
    public ActManager actManager;
    public StageManager stageManager;
    public VirtualEditManager virtualEditManager;
    public ControllerInput controllerInput;
    public PlaybackManager playbackManager;
    public FadingEffectManager fadingEffectManager;
    public HandMenuManager handMenuManager;
    public GameObject playButton;
    public Sprite playIcon;
    public Sprite pauseIcon;

    public void Init() {
        controllerInput.Init();
        slider.onValueChanged.AddListener(onSliderValueChanged);
        actText.text = "Act " + actManager.currentActIndex;
        countText.text = actManager.currentActIndex + 1 + " / " + actManager.actList.Count;
        handMenuManager.Init();
        if(actManager.actData.actSkyboxList[actManager.currentActIndex] != "") {
            virtualEditManager.changeSkybox(actManager.actData.actSkyboxList[actManager.currentActIndex]);
        } else {
            virtualEditManager.playbackSkybox.SetActive(true);
        }
    }

    void Update()
    {
        if(!playbackManager.playableDirector) return;
        if(playbackManager.playableDirector.state == PlayState.Playing) {
            slider.value = (float)playbackManager.playableDirector.time;
        } else {
             if(controllerInput.leftJoystickX != 0.0) {
                slider.value += controllerInput.leftJoystickX * (slider.maxValue / 100);
            }
        }
    }

    public void switchAct(int targetIndex) {
        if(targetIndex < 0 || targetIndex > actManager.actData.actIDList.Count - 1) return;
        actText.text = "Act " + targetIndex;
        countText.text = targetIndex + 1 + " / " + actManager.actList.Count;
        if(actManager.actData.actSkyboxList[targetIndex] != "") {
            virtualEditManager.changeSkybox(actManager.actData.actSkyboxList[targetIndex]);
        } else {
            virtualEditManager.playbackSkybox.SetActive(true);
        }
        actManager.switchAct(targetIndex);
    }

    public void changeSource() {
        slider.value = (float)playbackManager.playableDirector.time;
        slider.maxValue = (float)playbackManager.playableDirector.duration;
    }

    void onSliderValueChanged(float value)
    {
        playbackManager.setTime(value);
    }

    public void onStageButtonClicked() {
        stageManager.toRecordStage();
    }

    public void onPlayButtonClicked() {
        if(!playbackManager.isPlaying) {
            playbackManager.Play();
            playButton.GetComponent<Image>().sprite = pauseIcon;
        } else {
            playbackManager.Pause();
            playButton.GetComponent<Image>().sprite = playIcon;
        }
    }

    public void onPrevButtonClicked() {
        fadingEffectManager.StartFadingEffect();
        switchAct(actManager.currentActIndex - 1);
    }

    public void onNextButtonClicked() {
        fadingEffectManager.StartFadingEffect();
        switchAct(actManager.currentActIndex + 1);
    }
    public void onPlayableDirectorStopped() {
        slider.value = (float)playbackManager.playableDirector.time;
        playButton.GetComponent<Image>().sprite = playIcon;
    }
}