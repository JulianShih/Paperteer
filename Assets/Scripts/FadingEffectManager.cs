using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingEffectManager : MonoBehaviour
{
    public static FadingEffectManager instance;
    public float fadeDuration = 1.5f; // transition 時間 // 是不是要在 game manager 裡面 或是 transition manager 裡面設定阿
                                    // 不然 transition 有沒有結束在 fade effect manager 處理也很奇怪
    public Material fadeEffectMaterial; // 這個用來控制透明度 也就是 fade effect 的主要來源
    float fadeProcessPct; // 淡入淡出的時間軸
    float fadeTransparencyPct; // 透明度
    float fadeEndTime; // 紀錄淡入淡出的結束時間
    bool fadeTransitionFlag; // 
    bool activeFlagRequested = true;

    void Start()
    {
        FadingEffectManager.instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyUp(KeyCode.F)){
            StartFadingEffect(); // For Debugging
        }

        // TriggerFadingEffectOnTime();

        RefreshFadePercentage();
        UpdateFadeMaterial();
        UpdateTransitionFlag();
    }

    
    // void TriggerFadingEffectOnTime(){
    //     if(TransitButtonManager.instance.OnTransitBtnPressed){
    //         StartFadingEffect();
    //     }
    //     else if(TransitButtonManager.instance.OnTransitBtnReleased){
    //         StartFadingEffect();
    //     }
    // }

    void RefreshFadePercentage(){
        fadeProcessPct = Mathf.Clamp((Time.time - (fadeEndTime - fadeDuration)) / fadeDuration, 0, 1);
        fadeTransparencyPct = 1 - 2 * Mathf.Abs(fadeProcessPct - 0.5f);
    }

    void UpdateFadeMaterial(){
        Color _color = fadeEffectMaterial.color;
        // Multiply with a constant so the fade effect remain black for a while.
        // Clamp it since the zed camera will render it transparent when alpha is over 1. 
        _color.a = Mathf.Clamp(fadeTransparencyPct * 1.5f, 0, 1);
        fadeEffectMaterial.color = _color;
    }

    void UpdateTransitionFlag(){
        if(fadeProcessPct > 0.5f && !activeFlagRequested){
            // 在 fade 剛好的一半的時候，設為 true (是為了讓 打開或關閉 sphere)
            fadeTransitionFlag = true;
        }
        else if(activeFlagRequested){
            // fadeTransitionFlag 會維持原樣直到 有被要求過
            fadeTransitionFlag = false;
        }
    }

    public void StartFadingEffect(){
        // 如過現在還在 transition，那 fading 的過程就要從一半開始

        float _processPct = 1 - fadeTransparencyPct / 2; // Continue the fade process if not finished.
        fadeEndTime = Time.time + fadeDuration * _processPct;

        fadeTransitionFlag = false;
        activeFlagRequested = false;
    }

    public bool GetTransitionFlag(){
        // 如果是 false 就是 false
        // 如果是 true 要看是不是第一次被要，是的話才會傳 true

        if(fadeTransitionFlag) activeFlagRequested = true; // In each fading process, an active transitionFlag can only be requested once.
        return fadeTransitionFlag;
    }

    public bool IsTransitioning(){
        return Time.time < fadeEndTime;
    }
}
