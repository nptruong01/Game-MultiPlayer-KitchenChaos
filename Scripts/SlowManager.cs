//using System;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;

//public class SlowManager : NetworkBehaviour {
//    [SerializeField] private List<ContainerCounter_Random> containerCounters = new List<ContainerCounter_Random>();
//    [SerializeField] private int slowDownCount;
//    //[SerializeField] private int slowDownThreshold = 5;
//    [SerializeField] private float slowDownPercentage = 0.9f;

//    public static SlowManager Instance { get; private set; }
//    public event EventHandler Lamcham;

//    private void Update() {
//            Instance = this;
//        ContainerCounter_Random.Instance.Slow += ContainerCounter_OnPlayerGrabbedObject;

//    }

//    //private void AddContainerCounters() {
//    //    foreach (ContainerCounter_Random containerCounter in containerCounters) {
//    //        AddContainerCounter(containerCounter);
//    //    }
//    //}

//    //public void AddContainerCounter(ContainerCounter_Random containerCounter) {
//    //    if (!containerCounters.Contains(containerCounter)) {
//    //        containerCounters.Add(containerCounter);
//    //        containerCounter.Slow += ContainerCounter_OnPlayerGrabbedObject;
//    //    }
//    //}

//    private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e) {
//            slowDownCount = KitchenGameManager.Instance.GetSlowDownCount();
//            KitchenGameManager.Instance.IncrementSlowDownCount();

//        if (KitchenGameManager.Instance.GetSlowDownCount() >= KitchenGameManager.Instance.GetslowDownThreshold()) {
//                slowDownCount = 0;
//                SlowDownPlayerServerRpc();
//            }
        
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void SlowDownPlayerServerRpc() {
//        SlowDownPlayerClientRpc();
//    }

//    [ClientRpc]
//    private void SlowDownPlayerClientRpc() {
//            Lamcham?.Invoke(this, EventArgs.Empty);
        
//    }

//    //public int GetSlowDownCount() {
//    //    return slowDownCount;
//    //}

//    //public int GetslowDownThreshold() {
//    //    return slowDownThreshold;
//    //}
//}
