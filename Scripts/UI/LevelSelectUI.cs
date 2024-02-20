using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    public static LevelSelectUI Instance { get; private set; }


    public Level[] levels;
    public GameObject LevelsMenu;
    public Image levelImage;
    private int levelSelected = 1;
    [SerializeField] private Button mainmenu;

    private void Awake() {
        Instance = this;
        levelImage.sprite = levels[levelSelected - 1].image;

        mainmenu.onClick.AddListener(() => {
            KitchenGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    public void SelectLevel(int level) {
        levelSelected = level;
        levelImage.sprite = levels[level - 1].image;
    }

    public void PlaySelectedLevel() {
        // Use ChangeSceneMulti instead of ChangeScene
        //ChangeSceneMulti();
        ChangeScene(levels[levelSelected - 1].name);

    }
    public void ShowLevelMenu() {
        LevelsMenu.SetActive(true);
    }

    public void ChangeScene(string sceneName) {
        Loader.LoadNetwork(sceneName);
    }


    // Updated method to handle multiple scenes using switch case
    //private void ChangeSceneMulti() {
    //    switch (levelSelected) {
    //        case 1:
    //            Loader.LoadNetwork(Loader.Scene.GameScene1);
    //            break;
    //        case 2:
    //            Loader.LoadNetwork(Loader.Scene.GameScene2);
    //            break;
    //        case 3:
    //            Loader.LoadNetwork(Loader.Scene.GameScene);
    //            break;
    //        // Add more cases for additional levels if needed
    //        default:
    //            Debug.LogError("Invalid level selected");
    //            break;
    //    }
    //}

    public void Quit() {
        Application.Quit();
    }


}
