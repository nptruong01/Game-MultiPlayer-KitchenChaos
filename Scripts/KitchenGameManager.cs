using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour {
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    public event EventHandler Slow;





    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameVictory,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;
    [SerializeField] private int salesTarget = 10;
    [SerializeField] private float gamePlayingTimerMax = 90f;
    [SerializeField] private float newMaxTime = 50f;

    //[SerializeField] private int slowDownCount = 0;
    //[SerializeField] private int slowDownThreshold = 5; // Set your desired threshold here

    [SerializeField] private bool enableEvents = false;
    [SerializeField] private bool enableRandomTimePoints = false;
    [SerializeField] private float randomTimePoint1;
    [SerializeField] private float randomTimePoint2;
    [SerializeField] private float randomTimePoint3;
    private bool timePointsSet = false;

    private bool reachedTimePoint1 = false;
    private bool reachedTimePoint2 = false;
    private bool reachedTimePoint3 = false;

    [SerializeField] private List<Vector3> spawnPositionList;

    public List<Vector3> SpawnPositionList {
        get { return spawnPositionList; }
    }


    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    //private float gamePlayingTimerMax = 90f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false); 
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePausedState;






    private void Awake() {
        //Instance = this;

        if (Instance == null) {
            Instance = this;
        }

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
        DeliveryManager.Instance.UpdateMaxTime(newMaxTime);
        SetRandomTimePoints();
        timePointsSet = true;

    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;


    }
    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue) {
        if (isGamePaused.Value) {
            Time.timeScale = 0f;

            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;

            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
    private void State_OnValueChanged(State previousValue, State newValue) {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    //private void Update() {
    //    if (!IsServer) {
    //        return;
    //    }
    //    switch (state.Value) {
    //        case State.WaitingToStart:
    //            break;
    //        case State.CountdownToStart:
    //            countdownToStartTimer.Value -= Time.deltaTime;
    //            if (countdownToStartTimer.Value < 0f) {
    //                state.Value = State.GamePlaying;
    //                gamePlayingTimer.Value = gamePlayingTimerMax;
    //            }
    //            break;
    //        case State.GamePlaying:
    //            gamePlayingTimer.Value -= Time.deltaTime;
    //            if (gamePlayingTimer.Value < 0f) {
    //                state.Value = State.GameOver;
    //            }
    //            break;
    //        case State.GameOver:
    //            break;
    //    }
    //    //Debug.Log(state);
    //}

    private void Update() {
        if (!IsServer) {
            return;
        }

        if (enableRandomTimePoints && !timePointsSet) {
            SetRandomTimePoints();
            timePointsSet = true;
        }

        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                } 

                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f || IsSalesTargetReached()) {
                    state.Value = IsSalesTargetReached() ? State.GameVictory : State.GameOver;
                } else {
                    CheckRandomTimePoints();
                }
                break;
            case State.GameOver:
            case State.GameVictory:
                break;
        }
        //Debug.Log(slowDownCount);
    }

    private void SetRandomTimePoints() {
        if (!enableEvents) {
            return;
        }

        // Ensure that there's enough time for at least 3 intervals with a minimum separation of 10 seconds
        float minSeparation = 10f;
        float totalTimeNeeded = 3 * minSeparation;

        if (gamePlayingTimerMax < totalTimeNeeded) {
            Debug.LogError("Not enough time for 3 intervals with the given minimum separation.");
            return;
        }

        // Generate random intervals between 10 seconds and remaining time
        float remainingTime = gamePlayingTimerMax;

        // Generate the first random time point
        randomTimePoint1 = UnityEngine.Random.Range(0f, remainingTime - (2 * minSeparation));
        remainingTime -= randomTimePoint1;

        // Generate the second random time point with a minimum separation
        randomTimePoint2 = randomTimePoint1 + Mathf.Clamp(UnityEngine.Random.Range(minSeparation, remainingTime - minSeparation), minSeparation, float.MaxValue);
        remainingTime -= (randomTimePoint2 - randomTimePoint1);

        // Generate the third random time point with a minimum separation
        randomTimePoint3 = randomTimePoint2 + Mathf.Clamp(UnityEngine.Random.Range(minSeparation, remainingTime - minSeparation), minSeparation, float.MaxValue);

        // Sort the time points in ascending order
        SortRandomTimePoints();
    }


    private void SortRandomTimePoints() {
        float[] timePoints = { randomTimePoint1, randomTimePoint2, randomTimePoint3 };
        Array.Sort(timePoints);

        randomTimePoint1 = timePoints[0];
        randomTimePoint2 = timePoints[1];
        randomTimePoint3 = timePoints[2];
    }
    private void CheckRandomTimePoints() {
        if (!enableEvents) {
            return;
        }
        if (!reachedTimePoint3 && gamePlayingTimer.Value <= randomTimePoint3) {
            Debug.Log($"Reached time point 3: {randomTimePoint3}");
            Slow?.Invoke(this, EventArgs.Empty);
            reachedTimePoint3 = true;
        }

        if (!reachedTimePoint2 && gamePlayingTimer.Value <= randomTimePoint2) {
            Debug.Log($"Reached time point 2: {randomTimePoint2}");
            Slow?.Invoke(this, EventArgs.Empty);
            reachedTimePoint2 = true;
        }

        if (!reachedTimePoint1 && gamePlayingTimer.Value <= randomTimePoint1) {
            Debug.Log($"Reached time point 1: {randomTimePoint1}");
            Slow?.Invoke(this, EventArgs.Empty);
            reachedTimePoint1 = true;
        }

    }


    public bool IsSalesTargetReached() {
        int totalSalesValue = DeliveryManager.Instance.GetTotalSalesValue();
        //Debug.Log($"Total Sales Value: {totalSalesValue}, Sales Target: {salesTarget}");
        return totalSalesValue >= salesTarget;
    }
    private void LateUpdate() {
        if (autoTestGamePausedState) {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }

    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state.Value == State.CountdownToStart;
    }
    public float GetCountdownToStartTimer() {
        return countdownToStartTimer.Value;
    }
    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }
    public bool IsGameVictoty() {
        return state.Value == State.GameVictory;
    }
    public bool IsWaitingToStart() {
        return state.Value == State.WaitingToStart;
    }

    public bool IsLocalPlayerReady() {
        return isLocalPlayerReady;
    }
    public float GetGamePlayingTimerNormalized() {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public float GetgamePlayingTimer() {
        return gamePlayingTimer.Value;
    }
    public float GetgamePlayingTimerMax() {
        return gamePlayingTimerMax;
    }
    public int GetTarget() {
        return salesTarget;
    }
    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            PauseGameServerRpc();

            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            UnpauseGameServerRpc();

            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }
    private void TestGamePausedState() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId]) {
                // This player is paused
                isGamePaused.Value = true;
                return;
            }
        }

        // All players are unpaused
        isGamePaused.Value = false;
    }


    //public void IncrementSlowDownCount() {
    //    slowDownCount++;
    //}
    //public void ResetSlowDownCount() {
    //    //if (slowDownCount >= slowDownThreshold) {
    //    //    slowDownCount = 0;
    //    //    // Additional slowdown logic if needed
    //    //}
    //    slowDownCount = 0;

    //}
    //public int GetSlowDownCount() {
    //    return slowDownCount;
    //}

    //public int GetslowDownThreshold() {
    //    return slowDownThreshold;
    //}

}
