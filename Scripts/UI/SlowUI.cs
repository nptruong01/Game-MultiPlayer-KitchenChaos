using DG.Tweening.Core.Easing;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlowUI : MonoBehaviour {
    [SerializeField] private TMPro.TextMeshProUGUI countdownText;
    [SerializeField] private Slider slider;
    //[SerializeField] private TMPro.TextMeshProUGUI slowDownCountText; // New TextMeshProUGUI for slowDownCount
    [SerializeField] private Image frozenscreen;

    private void Awake() {
        Show();
        frozenscreen.gameObject.SetActive(false); // Hide the frozenscreen

    }
    private void Start() {
        KitchenGameManager.Instance.Slow += Instance_Slow;

    }
    //private void OnEnable() {
    //    if (KitchenGameManager.Instance != null) {
    //        KitchenGameManager.Instance.Slow += Instance_Slow;
    //    } else {
    //        Debug.LogError("KitchenGameManager.Instance is null.");
    //    }
    //}

    //private void OnDisable() {
    //    KitchenGameManager.Instance.Slow -= Instance_Slow;
    //}

    private void Update() {
        //int slowDownCount = ContainerCounter_Random.Instance.GetSlowDownCount(); // Get slowDownCount from ContainerCounter_Random
        //int slowDownCount = KitchenGameManager.Instance.GetSlowDownCount();

        //UpdateSlowDownCountText(slowDownCount);
    }

    private void Instance_Slow(object sender, System.EventArgs e) {
        StopAllCoroutines(); // Stop all existing coroutines
        float newCountdownValue = Player.LocalInstance.GetcountdownValue();
        StartCoroutine(CountdownCoroutine(newCountdownValue));
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void UpdateCountdownText(float countdownValue) {
        int countdownNumber = Mathf.CeilToInt(countdownValue);
        countdownText.text = countdownNumber.ToString();

        var normalizedTime = countdownNumber / Player.LocalInstance.GetcountdownValue();
        slider.value = normalizedTime;
    }

    //private void UpdateSlowDownCountText(int slowDownCount) {
    //    slowDownCountText.text = slowDownCount.ToString();
    //}

    private IEnumerator CountdownCoroutine(float initialCountdown) {
        float countdown = initialCountdown;

        while (countdown > 0) {
            UpdateCountdownText(countdown);
            frozenscreen.gameObject.SetActive(true); // Show the frozenscreen

            yield return null; // Wait for the next frame
            countdown -= Time.deltaTime;
        }
        frozenscreen.gameObject.SetActive(false); // Hide the frozenscreen

        UpdateCountdownText(0);
    }
}
