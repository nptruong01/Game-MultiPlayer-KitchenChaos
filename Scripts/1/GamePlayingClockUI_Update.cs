using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI_Update : MonoBehaviour
{
    //[SerializeField] private Image timerImage;
    //[SerializeField] private Canvas canvas;
    [SerializeField] private Animation animation;
    //[SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private Slider slider;
    [SerializeField] private string timerFormat = @"mm\:ss";
    [SerializeField] private Color finalSecondsColor = Color.red;
    [SerializeField] private float finaltimer = 20f;
    //private bool isFinalSeconds;



    private void Update() {
        //timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
        float seconds = KitchenGameManager.Instance.GetgamePlayingTimer();
        
        if (seconds <= finaltimer) {
            timer.color = finalSecondsColor;
            PlayFinalSecondsEffects();
        } else {
            // Reset the color to white if not in the final seconds range
            HandleTimeLimitUpdated();
            timer.color = Color.white;
        }
    }

    private void HandleTimeLimitUpdated() {
        float timemax = KitchenGameManager.Instance.GetgamePlayingTimerMax();

        var time = TimeSpan.FromSeconds(seconds);
        timer.text = time.ToString(timerFormat);

        var normilizedTime = seconds / timemax;
        slider.value = normilizedTime;
    }


    private void PlayFinalSecondsEffects() {
        animation.Play();
        //audioSource.Play();
    }
}

