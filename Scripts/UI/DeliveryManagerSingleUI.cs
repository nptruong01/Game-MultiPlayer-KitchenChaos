//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class DeliveryManagerSingleUI : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI recipeNameText;
//    [SerializeField] private Transform iconContainer;
//    [SerializeField] private Transform iconTemplate;

//    private void Awake() {
//        iconTemplate.gameObject.SetActive(false);
//    }

//    public void SetRecipeSO(RecipeSO recipeSO) {
//        recipeNameText.text = recipeSO.recipeName;

//        foreach (Transform child in iconContainer) {
//            if (child == iconTemplate) continue;
//            Destroy(child.gameObject);
//        }

//        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
//            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
//            iconTransform.gameObject.SetActive(true);
//            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
//        }
//    }
//}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private Transform iconNeedRoot; // New Transform variable
    [SerializeField] private Transform iconNeed; // New Transform variable
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private OrderTime orderTime;
    [SerializeField] private Sprite cookingSprite;
    [SerializeField] private Sprite cuttingSprite;
    [SerializeField] private Sprite nullIconSprite;

    private Coroutine autoHideCoroutine;
    //private OrderTime orderTime;
    private RecipeSO recipeSO; // Reference to the RecipeSO associated with this UI


    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
        iconNeed.gameObject.SetActive(false); // Ensure the iconNeed container is initially inactive

    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        this.recipeSO = recipeSO; // Save the reference for later use

        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }
        // Clear the iconNeed container
        foreach (Transform child in iconNeedRoot) {
            if (child == iconNeed) continue;
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconNeed, iconNeedRoot);
            iconTransform.gameObject.SetActive(true);
            if (recipeSO.needCookList.Contains(kitchenObjectSO)) {
                iconTransform.GetComponent<Image>().sprite = cookingSprite;
            }
            if (recipeSO.needCutList.Contains(kitchenObjectSO)) {
                iconTransform.GetComponent<Image>().sprite = cuttingSprite;
            }
            if (!recipeSO.needCookList.Contains(kitchenObjectSO) && !recipeSO.needCutList.Contains(kitchenObjectSO)) {
                iconTransform.GetComponent<Image>().sprite = nullIconSprite; // Set sprite for nullIcon
            }
        }

        priceText.text = recipeSO.price.ToString();



        if (orderTime == null) {
            orderTime = gameObject.AddComponent<OrderTime>();
            orderTime.countdownSlider = GetComponentInChildren<Slider>();
        }

        //StartAutoHideCoroutine(20f);
    }
    public RecipeSO GetRecipeSO() {
        return recipeSO;
    }
    private void StartAutoHideCoroutine(float duration) {
        if (autoHideCoroutine != null) {
            StopCoroutine(autoHideCoroutine);
        }
        autoHideCoroutine = StartCoroutine(AutoHideCoroutine(duration));
    }

    private IEnumerator AutoHideCoroutine(float duration) {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}