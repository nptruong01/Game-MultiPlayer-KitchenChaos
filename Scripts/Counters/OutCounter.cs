using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public ClearCounter nextCounter; // Reference to the next counter

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                TransferKitchenObject(player, player.GetKitchenObject());
            }
            // else: Player not carrying anything
        } else {
            // There is a KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } else {
                    // Player is not carrying Plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        // Counter is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            } else {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    private void TransferKitchenObject(Player player, KitchenObject kitchenObject) {
        // Check if the nextCounter already has a KitchenObject
        if (!nextCounter.HasKitchenObject()) {
            // Transfer the KitchenObject to the next counter
            kitchenObject.SetKitchenObjectParent(nextCounter);
        }
        // If the nextCounter already has a KitchenObject, keep it with the player
        else {
            // You can add your handling logic here, for example, notifying the player
            Debug.Log("Next counter already has a KitchenObject");
            // Optionally, you can keep the KitchenObject with the player
            player.SetKitchenObject(kitchenObject);
        }
    }
}
