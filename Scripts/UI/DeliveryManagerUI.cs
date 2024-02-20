//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DeliveryManagerUI : MonoBehaviour
//{
//    [SerializeField] private Transform container;
//    [SerializeField] private Transform recipeTemplate;


//    private void Awake() {
//        recipeTemplate.gameObject.SetActive(false);
//    }

//    private void Start() {
//        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
//        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

//        UpdateVisual();
//    }

//    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
//        UpdateVisual();
//    }

//    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
//        UpdateVisual();
//    }

//    private void UpdateVisual() {
//        foreach (Transform child in container) {
//            if (child == recipeTemplate) continue;
//            Destroy(child.gameObject);
//        }

//        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
//            Transform recipeTransform = Instantiate(recipeTemplate, container);
//            recipeTransform.gameObject.SetActive(true);
//            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);

//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;
    [SerializeField] private TMPro.TextMeshProUGUI currentSalesValueText;



    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeTimeOver += DeliveryManager_OnRecipeTimeOver;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;


        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        currentSalesValueText.text = DeliveryManager.Instance.GetTotalSalesValue().ToString() + "  \\  " + KitchenGameManager.Instance.GetTarget().ToString();
    }

    private void DeliveryManager_OnRecipeTimeOver(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    //private void UpdateVisual() {
    //    //foreach (Transform child in container) {
    //    //    if (child == recipeTemplate) continue;
    //    //    Destroy(child.gameObject);
    //    //}
    //    //
    //    Debug.Log("Instante -------------");
    //    if (DeliveryManager.Instance.GetWaitingRecipeSOList().Count < 1) return;
    //    RecipeSO recipeSO = DeliveryManager.Instance.GetWaitingRecipeSOList()[DeliveryManager.Instance.GetWaitingRecipeSOList().Count - 1];
    //    Transform recipeTransform = Instantiate(recipeTemplate, container);
    //    recipeTransform.gameObject.SetActive(true);
    //    recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
    //    //
    //    //foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
    //    //    Transform recipeTransform = Instantiate(recipeTemplate, container);
    //    //    recipeTransform.gameObject.SetActive(true);
    //    //    recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);

    //    //}
    //}
    private void UpdateVisual() {
        foreach (Transform child in container) {
            if (child == recipeTemplate) {
                continue;
            } else {
                Destroy(child.gameObject);
            }
        }


        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }

        currentSalesValueText.text = DeliveryManager.Instance.GetTotalSalesValue().ToString() + "  \\  " + KitchenGameManager.Instance.GetTarget().ToString();
    }


}