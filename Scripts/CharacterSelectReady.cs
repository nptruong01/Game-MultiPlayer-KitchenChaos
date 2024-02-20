using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour {
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> playerReadyDictionary;

    private int value;


    private void Awake() {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    //private void Start() {
    //    LevelSelectUI_Update.Instance.OnMap += Instance_OnMap;
    //}

    //private void Instance_OnMap(object sender, EventArgs e) {
    //    UpdateValueMapServerRpc();
    //}

    //[ServerRpc(RequireOwnership = false)]
    //private void UpdateValueMapServerRpc() {
    //    UpdateValueMapClientRpc();
    //}

    //[ClientRpc]
    //private void UpdateValueMapClientRpc() {
    //    value = LevelSelectUI_Update.Instance.GetValueMap();
    //    //Debug.Log(value);
    //}

    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

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
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
            //Debug.Log(value);

            //switch (value) {
            //    case 1:
            //        Loader.LoadNetwork(Loader.Scene.GameScene);
            //        break;
            //    case 2:
            //        Loader.LoadNetwork(Loader.Scene.GameScene2);
            //        break;
            //    case 3:
            //        Loader.LoadNetwork(Loader.Scene.GameScene3);
            //        break;
            //    default:
            //        Debug.LogError("Invalid level selected");
            //        break;
            //}
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId) {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsPlayerReady(ulong clientId) {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
