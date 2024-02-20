//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;


//public class DeliveryManager : NetworkBehaviour {


//    public event EventHandler OnRecipeSpawned;
//    public event EventHandler OnRecipeCompleted;
//    public event EventHandler OnRecipeSuccess;
//    public event EventHandler OnRecipeFailed;

//    public static DeliveryManager Instance { get; private set; }


//    [SerializeField] private RecipeListSO recipeListSO;

//    private List<RecipeSO> waitingRecipeSOList;
//    private float spawnRecipeTimer = 4f;
//    private float spawnRecipeTimerMax = 4f;
//    private int waitingRecipesMax = 4;
//    private int successfulRecipesAmount;



//    private void Awake() {
//        Instance = this;
//        waitingRecipeSOList = new List<RecipeSO>();
//    }

//    private void Update() {
//        if (!IsServer) {
//            return;
//        }

//        spawnRecipeTimer -= Time.deltaTime;
//        if (spawnRecipeTimer <= 0f) {
//            spawnRecipeTimer = spawnRecipeTimerMax;

//            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
//                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
//                //Debug.Log(waitingRecipeSO.recipeName);

//                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

//            }
//        }
//    }

//    [ClientRpc]
//    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
//        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

//        waitingRecipeSOList.Add(waitingRecipeSO);

//        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
//    }

//    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
//        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
//            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

//            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
//                // Has the same number of ingredients
//                bool plateContentsMatchesRecipe = true;
//                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
//                    // Cycling through all ingredients in the Recipe
//                    bool ingredientFound = false;
//                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
//                        // Cycling through all ingredients in the Plate
//                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
//                            // Ingredient matches!
//                            ingredientFound = true;
//                            break;
//                        }
//                    }
//                    if (!ingredientFound) {
//                        // This Recipe ingredient was not found on the Plate
//                        plateContentsMatchesRecipe = false;
//                    }
//                }

//                if (plateContentsMatchesRecipe) {
//                    // Player delivered the correct recipe!

//                    DeliverCorrectRecipeServerRpc(i);

//                    return;
//                }
//            }
//        }

//        // No matches found!
//        // Player did not deliver a correct recipe
//        //Debug.Log("Player did not deliver a correct recipe?");
//        DeliverIncorrectRecipeServerRpc();
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void DeliverIncorrectRecipeServerRpc() {
//        DeliverIncorrectRecipeClientRpc();
//    }

//    [ClientRpc]
//    private void DeliverIncorrectRecipeClientRpc() {
//        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
//        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
//    }

//    [ClientRpc]
//    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
//        successfulRecipesAmount++;

//        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

//        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
//        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
//    }

//    public List<RecipeSO> GetWaitingRecipeSOList() {
//        return waitingRecipeSOList;
//    }

//    public int GetSuccessfulRecipesAmount() {
//        return successfulRecipesAmount;
//    }
//}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;

//public class DeliveryManager : NetworkBehaviour {
//    public event EventHandler OnRecipeSpawned;
//    public event EventHandler OnRecipeCompleted;
//    public event EventHandler OnRecipeSuccess;
//    public event EventHandler OnRecipeFailed;
//    public event EventHandler OnRecipeTimeOver;


//    public static DeliveryManager Instance { get; private set; }

//    [SerializeField] private RecipeListSO recipeListSO;

//    private List<RecipeSO> waitingRecipeSOList;
//    private float spawnRecipeTimer = 0f;
//    private float spawnRecipeTimerMax = 0f;
//    private int waitingRecipesMax = 1;
//    private int successfulRecipesAmount;
//    private int totalSalesValue = 0;
//    private float debugTimer = 0f;
//    private float debugInterval = 1f;
//    //[SerializeField] private float maxTime = 50f;  // Maximum time a recipe can exist
//    private NetworkVariable<float> gamecurrentTime = new NetworkVariable<float>(0f);
//    private float maxTime;

//    public float MaxTime {
//        get { return maxTime; }
//        private set { maxTime = value; }
//    }

//    public void UpdateMaxTime(float newMaxTime) {
//        MaxTime = newMaxTime;
//    }

//    private void Awake() {
//        Instance = this;
//        waitingRecipeSOList = new List<RecipeSO>();
//    }

//    private void Update() {
//        if (!IsServer) {
//            return;
//        }

//        List<int> expiredRecipeIndices = new List<int>();

//        // Decrease the currentTime for each waiting recipe
//        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
//            waitingRecipeSOList[i].currentTime -= Time.deltaTime;
//            gamecurrentTime.Value = waitingRecipeSOList[i].currentTime;
//            //gamecurrentTime.Value -= Time.deltaTime;
//            // Check if the recipe's time has expired
//            if (gamecurrentTime.Value <= 0f) {
//                Debug.Log("Time Over");
//                TimeOverServerRpc(i);
//                //expiredRecipeIndices.Add(i);
//            }
//        }

//        //// Remove the expired recipes
//        //foreach (int expiredIndex in expiredRecipeIndices) {

//        //    waitingRecipeSOList.RemoveAt(expiredIndex);
//        //    OnRecipeTimeOver?.Invoke(this, EventArgs.Empty);

//        //}

//        //debugTimer += Time.deltaTime;
//        //if (debugTimer >= debugInterval) {
//        //    DebugCurrentTimeServerRpc();
//        //}

//        // Spawn a new recipe if there is room
//        spawnRecipeTimer -= Time.deltaTime;
//        if (spawnRecipeTimer <= 0f) {
//            spawnRecipeTimer = spawnRecipeTimerMax;

//            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
//                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
//                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

//                // Reset the current time for the newly spawned recipe
//                waitingRecipeSOList[waitingRecipeSOList.Count - 1].currentTime = maxTime;
//                //waitingRecipeSOList[waitingRecipeSOList.Count - 1].currentTime = waitingRecipeSOList[waitingRecipeSOList.Count - 1].maxTime;

//            }
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void DebugCurrentTimeServerRpc() {
//        DebugCurrentTimeClientRpc();

//    }
//    [ClientRpc]
//    private void DebugCurrentTimeClientRpc() {
//        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
//            Debug.Log($"Recipe {i + 1} currentTime: {gamecurrentTime.Value}");
//        }
//    }



//    [ClientRpc]
//    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
//        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
//        waitingRecipeSOList.Add(waitingRecipeSO);

//        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
//    }
//    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
//        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
//            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

//            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
//                // Has the same number of ingredients
//                bool plateContentsMatchesRecipe = true;
//                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
//                    // Cycling through all ingredients in the Recipe
//                    bool ingredientFound = false;
//                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
//                        // Cycling through all ingredients in the Plate
//                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
//                            // Ingredient matches!
//                            for (int j = 0; j < waitingRecipeSO.requiredOrder.Count; j++) {
//                                if (waitingRecipeSO.requiredOrder[j] != plateKitchenObject.GetKitchenObjectSOList()[j]) {
//                                    ingredientFound = false;
//                                    //break;
//                                } else {
//                                    ingredientFound = true;
//                                    break;
//                                }
//                            }

//                        }
//                    }
//                    if (!ingredientFound) {
//                        // This Recipe ingredient was not found on the Plate
//                        plateContentsMatchesRecipe = false;
//                    }
//                }

//                if (plateContentsMatchesRecipe) {
//                    // Player delivered the correct recipe!

//                    DeliverCorrectRecipeServerRpc(i);

//                    return;
//                }
//            }
//        }

//        // No matches found!
//        // Player did not deliver a correct recipe
//        //Debug.Log("Player did not deliver a correct recipe?");
//        DeliverIncorrectRecipeServerRpc();
//    }
//    [ServerRpc(RequireOwnership = false)]
//    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
//        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);

//    }
//    [ClientRpc]
//    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
//        successfulRecipesAmount++;
//        RecipeSO waitingRecipeSO = waitingRecipeSOList[waitingRecipeSOListIndex];
//        //Debug.Log(gamecurrentTime.Value);
//        // Calculate the total price with tip
//        float totalPrice = waitingRecipeSO.price + (gamecurrentTime.Value * 0.5f);

//        // Update the total sales value
//        totalSalesValue += Mathf.RoundToInt(totalPrice);

//        // Remove the delivered recipe
//        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

//        // Invoke events
//        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
//        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    private void TimeOverServerRpc(int waitingRecipeSOListIndex) {
//        TimeOverClientRpc(waitingRecipeSOListIndex);

//    }
//    [ClientRpc]
//    private void TimeOverClientRpc(int waitingRecipeSOListIndex) {

//        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

//        // Invoke events
//        OnRecipeTimeOver?.Invoke(this, EventArgs.Empty);

//    }


//    [ServerRpc(RequireOwnership = false)]
//    private void DeliverIncorrectRecipeServerRpc() {
//        DeliverIncorrectRecipeClientRpc();
//    }

//    [ClientRpc]
//    private void DeliverIncorrectRecipeClientRpc() {
//        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
//    }

//    public List<RecipeSO> GetWaitingRecipeSOList() {
//        return waitingRecipeSOList;
//    }

//    public int GetSuccessfulRecipesAmount() {
//        return successfulRecipesAmount;
//    }

//    public int GetTotalSalesValue() {
//        return totalSalesValue;
//    }
//}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeTimeOver;


    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 0f;
    private float spawnRecipeTimerMax = 0f;
    private int waitingRecipesMax = 1;
    private int successfulRecipesAmount;
    private int pertotalSale;
    private int totalSalesValue = 0;
    private float debugTimer = 0f;
    private float debugInterval = 1f;
    //[SerializeField] private float maxTime = 50f;  // Maximum time a recipe can exist
    private NetworkVariable<float> gamecurrentTime = new NetworkVariable<float>(0f);
    private float maxTime;

    [SerializeField] private RecipeListSO recipeListSO1;
    [SerializeField] private RecipeListSO recipeListSO2;
    [SerializeField] private bool enableEvents = false;
    [SerializeField] private bool firstIntervalPassed = false;
    [SerializeField] private bool secondIntervalPassed = false;
    //private float seconds;
    private NetworkVariable<float> Timesecond = new NetworkVariable<float>(0f);
    private float seconds1;

    public float MaxTime {
        get { return maxTime; }
        private set { maxTime = value; }
    }

    public void UpdateMaxTime(float newMaxTime) {
        MaxTime = newMaxTime;
    }

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Start() {
        float timemax = KitchenGameManager.Instance.GetgamePlayingTimerMax();

        seconds1 = timemax;
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        List<int> expiredRecipeIndices = new List<int>();

        // Decrease the currentTime for each waiting recipe
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            waitingRecipeSOList[i].currentTime -= Time.deltaTime;
            gamecurrentTime.Value = waitingRecipeSOList[i].currentTime;
            //gamecurrentTime.Value -= Time.deltaTime;
            // Check if the recipe's time has expired
            if (gamecurrentTime.Value <= 0f) {
                Debug.Log("Time Over");
                TimeOverServerRpc(i);
                //expiredRecipeIndices.Add(i);
            }
        }

        //// Remove the expired recipes
        //foreach (int expiredIndex in expiredRecipeIndices) {

        //    waitingRecipeSOList.RemoveAt(expiredIndex);
        //    OnRecipeTimeOver?.Invoke(this, EventArgs.Empty);

        //}

        //debugTimer += Time.deltaTime;
        //if (debugTimer >= debugInterval) {
        //    DebugCurrentTimeServerRpc();
        //}

        if (KitchenGameManager.Instance.IsGamePlaying()) {

            seconds1 -= Time.deltaTime;
            Timesecond.Value = seconds1;
            //Debug.Log(Timesecond.Value);

            UpdateThucDonServerRpc(Timesecond.Value);
        }

        // Spawn a new recipe if there is room
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

                // Reset the current time for the newly spawned recipe
                waitingRecipeSOList[waitingRecipeSOList.Count - 1].currentTime = maxTime;
                //waitingRecipeSOList[waitingRecipeSOList.Count - 1].currentTime = waitingRecipeSOList[waitingRecipeSOList.Count - 1].maxTime;

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DebugCurrentTimeServerRpc() {
        DebugCurrentTimeClientRpc();

    }
    [ClientRpc]
    private void DebugCurrentTimeClientRpc() {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            Debug.Log($"Recipe {i + 1} currentTime: {gamecurrentTime.Value}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateThucDonServerRpc(float seconds) {
        UpdateThucDonClientRpc(seconds);

    }
    [ClientRpc]
    private void UpdateThucDonClientRpc(float seconds) {

        float timemax = KitchenGameManager.Instance.GetgamePlayingTimerMax();

        if (enableEvents && !secondIntervalPassed && seconds <= (2f * timemax / 3f)) {
            recipeListSO = recipeListSO1;
            secondIntervalPassed = true;
            for (int i = 0; i < waitingRecipeSOList.Count; i++) {

                Debug.Log("Time Over");
                UpdateServerRpc(i);
            }
        }

        // Switch recipe list at 1/3 of remaining time
        if (enableEvents && !firstIntervalPassed && seconds <= (timemax / 3f)) {
            recipeListSO = recipeListSO2;
            firstIntervalPassed = true;
            for (int i = 0; i < waitingRecipeSOList.Count; i++) {

                Debug.Log("Time Over");
                UpdateServerRpc(i);
            }
        }
    }


    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            // Ingredient matches!
                            for (int j = 0; j < waitingRecipeSO.requiredOrder.Count; j++) {
                                if (waitingRecipeSO.requiredOrder[j] != plateKitchenObject.GetKitchenObjectSOList()[j]) {
                                    ingredientFound = false;
                                    //break;
                                } else {
                                    ingredientFound = true;
                                    break;
                                }
                            }

                        }
                    }
                    if (!ingredientFound) {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!

                    DeliverCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        //Debug.Log("Player did not deliver a correct recipe?");
        DeliverIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);

    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
        successfulRecipesAmount++;
        RecipeSO waitingRecipeSO = waitingRecipeSOList[waitingRecipeSOListIndex];
        //Debug.Log(gamecurrentTime.Value);
        // Calculate the total price with tip
        float totalPrice = waitingRecipeSO.price + (gamecurrentTime.Value * 0.5f);

        pertotalSale = Mathf.RoundToInt(totalPrice);

        // Update the total sales value
        totalSalesValue += Mathf.RoundToInt(totalPrice);

        // Remove the delivered recipe
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        // Invoke events
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TimeOverServerRpc(int waitingRecipeSOListIndex) {
        TimeOverClientRpc(waitingRecipeSOListIndex);

    }
    [ClientRpc]
    private void TimeOverClientRpc(int waitingRecipeSOListIndex) {

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        // Deduct 20 from totalSalesValue
        totalSalesValue -= 20;

        // Ensure totalSalesValue is not less than 0
        if (totalSalesValue < 0) {
            totalSalesValue = 0;
        }

        // Invoke events
        OnRecipeTimeOver?.Invoke(this, EventArgs.Empty);

    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateServerRpc(int waitingRecipeSOListIndex) {
        UpdateClientRpc(waitingRecipeSOListIndex);

    }
    [ClientRpc]
    private void UpdateClientRpc(int waitingRecipeSOListIndex) {

        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);


    }


    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        // Deduct 10 from totalSalesValue
        totalSalesValue -= 10;

        // Ensure totalSalesValue is not less than 0
        if (totalSalesValue < 0) {
            totalSalesValue = 0;
        }

        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

    public int GetTotalSalesValue() {
        return totalSalesValue;
    }
    public int GetPerSalesValue() {
        return pertotalSale;
    }
}