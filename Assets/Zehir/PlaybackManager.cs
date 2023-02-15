using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class PlaybackManager : MonoBehaviour
{
    public bool isPlaying = false;
    public ActManager actManager;
    public PadManager padManager;
    public UIManager uIManager;
    public PlayableDirector playableDirector;
    public TimelineHandler timelineHandler;
    public VideoPlayer videoPlayer;
    public float offset;

    void Start() {
        videoPlayer.prepareCompleted += prepareCompleted;
    }

    public void Play() {
        Debug.Log("PlaybackManager.Play");
        isPlaying = true;
        playableDirector.Play();
        sync();
        videoPlayer.Play();
    }

    public void Pause() {
        Debug.Log("PlaybackManager.Pause");
        isPlaying = false;
        playableDirector.Pause();
        videoPlayer.Pause();
    }

    public void Stop() {
        Debug.Log("PlaybackManager.Stop");
        isPlaying = false;
        playableDirector.Stop();
        videoPlayer.Stop();
    }

    public void Prepare() {
        Debug.Log("videoPlayer.Prepare");
        videoPlayer.Prepare();
        playableDirector.time = 0;
        playableDirector.Evaluate();
    }

    public bool changeSource() {
        Debug.Log("PlaybackManager.changeSource " + actManager.currentActIndex);
        isPlaying = false;
        GameObject currentAct = actManager.actList[actManager.currentActIndex];
        playableDirector = currentAct.GetComponent<PlayableDirector>();
        timelineHandler = currentAct.GetComponent<TimelineHandler>();
        videoPlayer.clip = currentAct.GetComponent<ActHandler>().videoClip;
        if (!videoPlayer.clip) {
            Debug.Log("Video not found !");
            return false;
        } else {
            Prepare();
            padManager.changeSource();
            uIManager.changeSource();
            return true;
        }
    }

    void prepareCompleted(VideoPlayer vp) {
        Debug.Log("videoPlayer.prepareCompleted");
        sync();
        videoPlayer.StepForward();
    }
    public void sync() {
        videoPlayer.time = playableDirector.time + offset;
    }

    public void setTime(float value) {
        if(playableDirector.state == PlayState.Playing) return;
        playableDirector.time = value;
        playableDirector.Evaluate();
        sync();
        videoPlayer.StepForward();
    }

    public void onPlayableDirectorStopped() {
        if(!videoPlayer) return;
        Debug.Log("PlaybackManager.onPlayableDirectorStopped");
        isPlaying = false;
        videoPlayer.Stop();
        padManager.onPlayableDirectorStopped();
        uIManager.onPlayableDirectorStopped();
        Prepare(); 
    }
}