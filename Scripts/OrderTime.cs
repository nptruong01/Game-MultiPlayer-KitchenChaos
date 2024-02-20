using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OrderTime : NetworkBehaviour {
    public Slider countdownSlider;

    public float currentTime;
    //public float maxTime = 50f;
    public float maxTime;

    public float MaxTime {
        get { return maxTime; }
        private set { maxTime = value; }
    }

    public void UpdateMaxTime(float newMaxTime) {
        MaxTime = newMaxTime;
    }

    //public event EventHandler OnRecipeTimeOver;

    private void Awake() {
        if (countdownSlider != null) {
            countdownSlider.maxValue = maxTime;
            countdownSlider.value = maxTime;
        }
    }

    private void Start() {
        StartCountdown();
    }

    private void Update() {
        if (countdownSlider != null) {
            if (currentTime > 0) {
                currentTime -= Time.deltaTime;
                countdownSlider.value = currentTime;
            } else {
                HideOrder();
                //OnRecipeTimeOver?.Invoke(this, EventArgs.Empty);
                Debug.Log("Recipe time is over!"); // Add this line for debugging
            }
        }
    }

    private void StartCountdown() {
        currentTime = maxTime;
    }

    private void HideOrder() {
        Destroy(gameObject);
    }
}
