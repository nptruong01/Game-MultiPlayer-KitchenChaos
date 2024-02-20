using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter_Liquid : BaseCounter
{
    //public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                // Only accepts Plates
                if (plateKitchenObject.TryAddIngredient(kitchenObjectSO)) {
                    //KitchenObject.DestroyKitchenObject(GetKitchenObject());
                }

                ////KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

                ////DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                ////KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

}
