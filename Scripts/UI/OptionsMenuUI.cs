using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    public static OptionsMenuUI Instance { get; private set; }


    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;

    private Action onCloseButtonAction;


    private void Awake() {
        Instance = this;

        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });


    }
    private void Start() {
        //KitchenGameManager.Instance.OnLocalGameUnpaused += KitchenGameManager_OnGameUnpaused;

        UpdateVisual();

        //HidePressToRebindKey();
        //Hide();
    }

    //private void KitchenGameManager_OnGameUnpaused(object sender, System.EventArgs e) {
    //    Hide();
    //}

    private void UpdateVisual() {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        
    }
    //public void Show(Action onCloseButtonAction) {
    //    this.onCloseButtonAction = onCloseButtonAction;

    //    gameObject.SetActive(true);

    //    soundEffectsButton.Select();
    //}

    //private void Hide() {
    //    gameObject.SetActive(false);
    //}
    //private void ShowPressToRebindKey() {
    //    pressToRebindKeyTransform.gameObject.SetActive(true);
    //}

    //private void HidePressToRebindKey() {
    //    pressToRebindKeyTransform.gameObject.SetActive(false);
    //}
    //private void RebindBinding(GameInput.Binding binding) {
    //    ShowPressToRebindKey();
    //    GameInput.Instance.RebindBinding(binding, () => {
    //        HidePressToRebindKey();
    //        UpdateVisual();
    //    });
    //}
}
