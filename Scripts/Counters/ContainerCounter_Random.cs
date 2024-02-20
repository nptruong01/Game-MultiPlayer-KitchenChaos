//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;

//public class ContainerCounter_Random : BaseCounter {
//    public static ContainerCounter_Random Instance { get; private set; }
//    public event EventHandler OnPlayerGrabbedObject;
//    public event EventHandler Slow;
//    private void Awake() {
//        Instance = this;


//    }
//    [SerializeField] private KitchenObjectSO kitchenObjectSO;
//    [SerializeField] private float slowDownPercentage = 0.9f; // 30% chance to slow down


//    public override void Interact(Player player) {
//        if (!player.HasKitchenObject()) {
//            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

//            if (UnityEngine.Random.value <= slowDownPercentage) {
//                // 30% chance to slow down the player
//                SlowDownPlayerServerRpc();
//                Debug.Log("SLOW");
//            } else {
//                InteractLogicServerRpc();
//            }
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void InteractLogicServerRpc() {
//        InteractLogicClientRpc();
//    }

//    [ClientRpc]
//    private void InteractLogicClientRpc() {
//        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void SlowDownPlayerServerRpc() {
//        SlowDownPlayerClientRpc();
//    }

//    [ClientRpc]
//    private void SlowDownPlayerClientRpc() {

//        Slow?.Invoke(this, EventArgs.Empty);

//    }


//}

//using DG.Tweening.Core.Easing;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;

//public class ContainerCounter_Random : BaseCounter {
//    public static ContainerCounter_Random Instance { get; private set; }
//    public event EventHandler OnPlayerGrabbedObject;
//    public event EventHandler Slow;

//    [SerializeField] private KitchenObjectSO kitchenObjectSO;
//    [SerializeField] private float slowDownPercentage = 0.9f; // 30% chance to slow down

//    //private int slowDownCount = 0;
//    private int slowDownThreshold = 5; // Set your desired threshold here

//    private void Awake() {
//        Instance = this;
//    }

//    public override void Interact(Player player) {
//        if (!player.HasKitchenObject()) {
//            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

//            if (UnityEngine.Random.value <= slowDownPercentage) {
//                // Increment the slowDownCount
//                //slowDownCount++;
//                KitchenGameManager.Instance.IncrementSlowDownCount();

//                // Check if the threshold is reached
//                if (KitchenGameManager.Instance.GetSlowDownCount() >= slowDownThreshold) {

//                    //if (slowDownCount >= slowDownThreshold) {
//                    // Reset the count and apply slowdown
//                    //slowDownCount = 0;
//                    //KitchenGameManager.Instance.ResetSlowDownCount();

//                    SlowDownPlayerServerRpc();
//                    Debug.Log("SLOW");
//                } else {
//                    InteractLogicServerRpc();
//                }
//            } else {
//                InteractLogicServerRpc();
//            }
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void InteractLogicServerRpc() {
//        InteractLogicClientRpc();
//    }

//    [ClientRpc]
//    private void InteractLogicClientRpc() {
//        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void SlowDownPlayerServerRpc() {
//        SlowDownPlayerClientRpc();
//    }

//    [ClientRpc]
//    private void SlowDownPlayerClientRpc() {
//        Slow?.Invoke(this, EventArgs.Empty);
//    }
//    //public int GetSlowDownCount() {
//    //    return slowDownCount;
//    //}
//}
using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter_Random : BaseCounter {
    public static ContainerCounter_Random Instance { get; private set; }
    public event EventHandler OnPlayerGrabbedObject;
    public event EventHandler Slow;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private float slowDownPercentage = 0.9f; // 30% chance to slow down

    //private int slowDownThreshold = 5; // Set your desired threshold here

    private void Awake() {
        Instance = this;
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            InteractLogicServerRpc();

            //if (UnityEngine.Random.value <= slowDownPercentage) {


            //    KitchenGameManager.Instance.IncrementSlowDownCount();

            //    if (KitchenGameManager.Instance.GetSlowDownCount() >= KitchenGameManager.Instance.GetslowDownThreshold()) {
            //        KitchenGameManager.Instance.ResetSlowDownCount();

            //        //slowDownCount = 0;
            //        SlowDownPlayerServerRpc();
            //        //SlowDownPlayer();

            //        Debug.Log("SLOW");
            //        //InteractLogicServerRpc();

            //        KitchenGameManager.Instance.ResetSlowDownCount();

            //    }
            //    //else {

            //    //    InteractLogicServerRpc();
            //    //}
            //} 
            ////else {
            ////    //InteractLogicServerRpc();

            ////}
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }

    //[ServerRpc(RequireOwnership = false)]
    //private void SlowDownPlayerServerRpc() {
    //    SlowDownPlayerClientRpc();
    //}

    //[ClientRpc]
    //private void SlowDownPlayerClientRpc() {
    //    Slow?.Invoke(this, EventArgs.Empty);
    //}

}

