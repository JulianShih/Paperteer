using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;
using UnityEditor;

public class TimelineHandler : MonoBehaviour
{
    public VirtualEditManager virtualEditManager;
    public PlayableDirector playableDirector;
    public ActHandler actHandler;
    public PlaybackManager playbackManager;
    public Animator cameraAnimator;
    TimelineAsset timelineAsset;

    public void Init(int index) {
        actHandler = GetComponent<ActHandler>();
        playableDirector = GetComponent<PlayableDirector>();
        playbackManager = GameObject.Find("Playback Stage").GetComponent<PlaybackManager>();
        virtualEditManager = GameObject.Find("Playback Stage").GetComponent<VirtualEditManager>();
        cameraAnimator = GameObject.Find("Tracker (Camera)").GetComponent<Animator>();
        playableDirector.stopped += onPlayableDirectorStopped;
        loadAsset(index);
    }

    public void loadAsset(int index) {
        Debug.Log("TimelineHandler.loadAsset");
        // load timeline
        string path = "Assets/Resources/Timeline/" + gameObject.name + ".playable";
        timelineAsset = (TimelineAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TimelineAsset));
        if (!timelineAsset) { // no existing timeline
            // create timeline
            timelineAsset = new TimelineAsset();
            AssetDatabase.CreateAsset(timelineAsset, "Assets/Resources/Timeline/" + gameObject.name + ".playable");
            playableDirector.playableAsset = timelineAsset;
            // create camera track and bind
            AnimationTrack cameraTrack =  timelineAsset.CreateTrack<AnimationTrack>("Camera");
            playableDirector.SetGenericBinding(cameraTrack, cameraAnimator);
           
            // load camera clip
            path = "Assets/Resources/Recordings/" + gameObject.name + "_Camera.anim";
            AnimationClip cameraAnimation = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
            TimelineClip cameraClip = cameraTrack.CreateClip(cameraAnimation);
            AnimationPlayableAsset cameraAsset = cameraClip.asset as AnimationPlayableAsset;
            cameraAsset.removeStartOffset = false;
            
            foreach (GameObject pObj in actHandler.playbackObjectList) {
                AnimationTrack animationTrack =  timelineAsset.CreateTrack<AnimationTrack>(pObj.name);
                playableDirector.SetGenericBinding(animationTrack, pObj.GetComponent<Animator>());
                path = "Assets/Resources/Recordings/" + gameObject.name + "_" + pObj.name + ".anim";
                AnimationClip pObjAnimation = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
                TimelineClip pObjClip = animationTrack.CreateClip(pObjAnimation);
                AnimationPlayableAsset pObjAsset = pObjClip.asset as AnimationPlayableAsset;
                pObjAsset.removeStartOffset = false;
            }
        } else {
            // load timeline
            playableDirector.playableAsset = timelineAsset;
            var tracks = timelineAsset.GetOutputTracks();
            // bind track
            foreach (var t in tracks) {
                if(t.name == "Camera") {
                    playableDirector.SetGenericBinding(t, cameraAnimator);
                } else {
                    Transform tr = this.transform.Find(t.name);
                    if(!tr) {
                    } else {
                        playableDirector.SetGenericBinding(t, tr.GetComponent<Animator>());
                    }
                }
            }
        }
    }

    public void addObject(GameObject obj) {
        var newTrack = timelineAsset.CreateTrack<ActivationTrack>(null, obj.name);
        playableDirector.SetGenericBinding(newTrack, obj);
        var clip = newTrack.CreateDefaultClip();
        clip.start = playableDirector.time;
        clip.duration = playableDirector.duration - playableDirector.time;
        playableDirector.RebuildGraph();
    }

    public void deleteObject(GameObject obj) {
        var tracks = timelineAsset.GetOutputTracks();
        foreach (var t in tracks) {
            if(t.name == obj.name) {
                var clips = t.GetClips();
                foreach(TimelineClip clip in clips) {
                    clip.duration = (playableDirector.time - 1) - clip.start; 
                    playableDirector.RebuildGraph();
                }
            }
        }
    }

    public void onPlayableDirectorStopped(PlayableDirector p) {
        playbackManager.onPlayableDirectorStopped();
    }
}