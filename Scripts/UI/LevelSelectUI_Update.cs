using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelSelectUI_Update : NetworkBehaviour {
    public event EventHandler OnMap;
    public static LevelSelectUI_Update Instance { get; private set; }

    public Level[] levels;
    public GameObject LevelsMenu;
    public Image levelImage;
    private int levelSelected = 1;

    [SerializeField] private Button kickButton;
    //[SerializeField] private Button kickButton2;
    //[SerializeField] private Button kickButton3;

    private int value1;
    private NetworkVariable<int> value = new NetworkVariable<int>(0);

    private void Awake() {
        Instance = this;
        levelImage.sprite = levels[levelSelected - 1].image;

        kickButton.onClick.AddListener(() => SelectLevelServerRpc(1));
        //kickButton2.onClick.AddListener(() => SelectLevelServerRpc(2));
        //kickButton3.onClick.AddListener(() => SelectLevelServerRpc(3));
    }

    private void Start() {
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        //kickButton2.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        //kickButton3.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        SelectLevelServerRpc(1);

    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectLevelServerRpc(int level) {
        SelectLevelClientRpc(level);
        //ValueMapClientRpc();
        //Debug.Log(value.Value);

    }

    [ClientRpc]
    private void SelectLevelClientRpc(int level) {
        levelSelected = level;
        levelImage.sprite = levels[level - 1].image;
        ValueMapServerRpc();
        Debug.Log(value.Value);
    }

    public void PlaySelectedLevel() {
        ChangeScene(levels[levelSelected - 1].name);
    }

    public void ShowLevelMenu() {
        LevelsMenu.SetActive(true);
    }

    public void ChangeScene(string sceneName) {
        Loader.LoadNetwork(sceneName);
    }

    //public void ChangeSceneMulti() {
    //    KitchenGameLobby.Instance.DeleteLobby();

    //    switch (levelSelected) {
    //        case 1:
    //            Loader.LoadNetwork(Loader.Scene.GameScene);
    //            break;
    //        case 2:
    //            Loader.LoadNetwork(Loader.Scene.GameScene4);
    //            break;
    //        case 3:
    //            Loader.LoadNetwork(Loader.Scene.GameScene5);
    //            break;
    //        default:
    //            Debug.LogError("Invalid level selected");
    //            break;
    //    }
    //}

    public void Quit() {
        Application.Quit();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ValueMapServerRpc() {
        ValueMapClientRpc();
    }

    [ClientRpc]
    private void ValueMapClientRpc() {
        value1 = levelSelected;
        value.Value = value1;
        OnMap?.Invoke(this, EventArgs.Empty);
    }

    public int GetValueMap() {
        return value.Value;
    }
}