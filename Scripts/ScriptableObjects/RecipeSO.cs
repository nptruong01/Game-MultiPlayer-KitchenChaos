using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {

    public List<KitchenObjectSO> kitchenObjectSOList;
    public List<KitchenObjectSO> requiredOrder;
    public List<KitchenObjectSO> needCookList;
    public List<KitchenObjectSO> needCutList;
    public string recipeName;
    public int price;
    public float currentTime;
}
