using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryPerSalesUI : MonoBehaviour
{
    //[SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeTimeOver += DeliveryManager_OnRecipeTimeOver;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        ShowMessage("-10");
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        ShowMessage("+" + DeliveryManager.Instance.GetPerSalesValue().ToString());
    }

    private void DeliveryManager_OnRecipeTimeOver(object sender, System.EventArgs e) {
        ShowMessage("-20");
    }

    private void ShowMessage(string text) {
        gameObject.SetActive(true);
        messageText.text = text;
        Invoke("HideMessage", 1.2f); // Invoke HideMessage method after 1.2 seconds
    }

    private void HideMessage() {
        gameObject.SetActive(false);
    }
}
