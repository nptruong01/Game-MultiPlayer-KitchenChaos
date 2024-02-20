using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CounterManager : NetworkBehaviour {
    [SerializeField] private GameObject alertImage;

    [SerializeField] private bool enableEvents = false;
    [SerializeField] private bool firstIntervalPassed = false;
    [SerializeField] private bool secondIntervalPassed = false;

    [SerializeField] private Transform counter1;
    [SerializeField] private Transform counter2;
    [SerializeField] private Transform counter3;

    private NetworkVariable<float> Timesecond = new NetworkVariable<float>(0f);


    private float seconds1;
    private float timemax;

    private void Start() {
        timemax = KitchenGameManager.Instance.GetgamePlayingTimerMax();
        seconds1 = timemax;
        InitializeCountersPosition();

        alertImage.SetActive(false);


    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        if (KitchenGameManager.Instance.IsGamePlaying()) {

            seconds1 -= Time.deltaTime;
            Timesecond.Value = seconds1;
            //Debug.Log(Timesecond.Value);

            UpdateCounterServerRpc(Timesecond.Value);
        }
        
    }

    private void InitializeCountersPosition() {
       
        MoveCounters(counter1, 0);
        MoveCounters(counter2, -9);
        MoveCounters(counter3, -9);
    }

    private void MoveCounters(Transform counter, float newY) {
        Vector3 newPosition = counter.localPosition;
        newPosition.y = newY;
        counter.localPosition = newPosition;
    }

    private IEnumerator WithAlert() {
        // Show alert image
        alertImage.SetActive(true);
        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        // Hide alert image
        alertImage.SetActive(false);
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdateCounterServerRpc(float seconds) {
        UpdateCounterClientRpc(seconds);

    }
    [ClientRpc]
    private void UpdateCounterClientRpc(float seconds) {

        float timemax = KitchenGameManager.Instance.GetgamePlayingTimerMax();

        if (enableEvents && !secondIntervalPassed && seconds <= (2f * timemax / 3f)) {

            StartCoroutine(WithAlert());

            MoveCounters(counter1, -9);
            MoveCounters(counter2, 0);
            MoveCounters(counter3, -9);
            secondIntervalPassed = true;

        }

        // Switch recipe list at 1/3 of remaining time
        if (enableEvents && !firstIntervalPassed && seconds <= (timemax / 3f)) {
            StartCoroutine(WithAlert());

            //StartCoroutine(MoveCountersWithAlert(counter1, -9));

            MoveCounters(counter1, -9);
            MoveCounters(counter2, -9);
            MoveCounters(counter3, 0);
            firstIntervalPassed = true;

        }
    }
}