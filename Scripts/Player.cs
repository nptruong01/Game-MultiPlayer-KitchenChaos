//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Netcode;
//using UnityEngine;


//public class Player : NetworkBehaviour, IKitchenObjectParent {

//    public static event EventHandler OnAnyPlayerSpawned;
//    public static event EventHandler OnAnyPickedSomething;


//    public static void ResetStaticData() {
//        OnAnyPlayerSpawned = null;
//    }


//    public static Player LocalInstance { get; private set; }

//    public event EventHandler OnPickedSomething;
//    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
//    public class OnSelectedCounterChangedEventArgs : EventArgs {
//        public BaseCounter selectedCounter;
//    }



//    [SerializeField] private float moveSpeed = 7f;
//    [SerializeField] private LayerMask countersLayerMask;
//    [SerializeField] private LayerMask collisionsLayerMask;
//    [SerializeField] private Transform kitchenObjectHoldPoint;
//    [SerializeField] private List<Vector3> spawnPositionList;
//    [SerializeField] private PlayerVisual playerVisual;




//    private bool isWalking;
//    private Vector3 lastInteractDir;
//    private BaseCounter selectedCounter;
//    private KitchenObject kitchenObject;

//    private void Start() {
//        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
//        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

//        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
//        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
//    }

//    public override void OnNetworkSpawn() {
//        if (IsOwner) {
//            LocalInstance = this;
//        }
//        //transform.position = spawnPositionList[(int)(OwnerClientId)];

//        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

//        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

//        if (IsServer) {
//            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
//        }
//    }

//    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
//        if (clientId == OwnerClientId && HasKitchenObject()) {
//            KitchenObject.DestroyKitchenObject(GetKitchenObject());
//        }
//    }
//    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
//        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


//        if (selectedCounter != null) {
//            selectedCounter.InteractAlternate(this);
//        }
//    }

//    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
//        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


//        if (selectedCounter != null) {
//            selectedCounter.Interact(this);
//        }
//    }

//    private void Update() {
//        if (!IsOwner) {
//            return;
//        }
//        HandleMovement();
//        HandleInteractions();
//    }
//    public bool IsWalking() {
//        return isWalking;
//    }
//    private void HandleInteractions() {
//        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

//        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

//        if (moveDir != Vector3.zero) {
//            lastInteractDir = moveDir;
//        }

//        float interactDistance = 2f;
//        //Physics.Raycast(transform.position, moveDir,out RaycastHit raycastHit, interactDistance);
//        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
//            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
//                // Has ClearCounter
//                //clearCounter. Interact();
//                if (baseCounter != selectedCounter) {
//                    SetSelectedCounter(baseCounter);   
//                }
//            } else {
//                SetSelectedCounter(null);
//            }
//        } else {
//            SetSelectedCounter(null);
//        }
//        //Debug.Log(selectedCounter);
//    }

//    private void HandleMovement() {

//        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

//        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

//        float moveDistance = moveSpeed * Time.deltaTime;
//        float playerRadius = .7f;
//        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

//        if (!canMove) {
//            // Khong the di chuyen theo huong moveDir

//            // Thu chi di chuyen theo huong X
//            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
//            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

//            if (canMove) {
//                // Co the di chuyen chi theo truc X
//                moveDir = moveDirX;
//            } else {
//                // Khong the di chuyen chi theo truc X

//                // Thu chi di chuyen theo huong Z
//                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
//                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

//                if (canMove) {
//                    // Co the di chuyen chi theo truc Z
//                    moveDir = moveDirZ;
//                } else {
//                    // Khong the di chuyen theo bat ky huong nao
//                }
//            }
//        }


//        if (canMove) {
//            transform.position += moveDir * moveDistance;
//        }
//        //transform.position += inputVector; 
//        //Bang voi transform.position transform.position + inputVector;

//        // Di chuyen la khi Vecto khac 0

//        isWalking = moveDir != Vector3.zero;


//        //Quay dau theo di chuyen

//        float rotateSpeed = 10f;
//        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

//        //Debug.Log(Time.deltaTime);

//    }
//    private void SetSelectedCounter(BaseCounter selectedCounter) {
//        this.selectedCounter = selectedCounter;

//        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
//            selectedCounter = selectedCounter
//        });
//    }

//    public Transform GetKitchenObjectFollowTransform() {
//        return kitchenObjectHoldPoint;
//    }
//    public void SetKitchenObject(KitchenObject kitchenObject) {
//        this.kitchenObject = kitchenObject;

//        if (kitchenObject != null) {
//            OnPickedSomething?.Invoke(this, EventArgs.Empty);
//            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);

//        }
//    }
//    public KitchenObject GetKitchenObject() {
//        return kitchenObject;
//    }
//    public void ClearKitchenObject() {
//        kitchenObject = null;
//    }
//    public bool HasKitchenObject() {
//        return kitchenObject != null;
//    }
//    public NetworkObject GetNetworkObject() {
//        return NetworkObject;
//    }
//}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour, IKitchenObjectParent {

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;


    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }


    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }



    //[SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float basemoveSpeed = 7f;
    [SerializeField] private float slowmoveSpeed = 3f;
    [SerializeField] private float countdown = 7f;

    [SerializeField] private int slowDownCount = 0;
    [SerializeField] private int slowDownThreshold = 5;

    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    //[SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;



    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        moveSpeed = basemoveSpeed;

    }
    private void ContainerCounter_Slow(object sender, EventArgs e) {
        ResetCountdown();

        StartCoroutine(SlowDownForSeconds(countdown));

        //slowDownCount++;
        //if (slowDownCount >= slowDownThreshold) {

        //    slowDownCount = 0;
        //    ResetCountdown();

        //    StartCoroutine(SlowDownForSeconds(countdown));

        //}
    }
    private void ResetCountdown() {
        StopAllCoroutines(); // Stop all existing coroutines
    }
    private IEnumerator SlowDownForSeconds(float duration) {

        moveSpeed = slowmoveSpeed;

        yield return new WaitForSeconds(duration);

        moveSpeed = basemoveSpeed;
    }
    public float GetcountdownValue() {
        return countdown;
    }
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }
        //transform.position = spawnPositionList[(int)(OwnerClientId)];

        List<Vector3> spawnPositions = KitchenGameManager.Instance.SpawnPositionList;

        transform.position = spawnPositions[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        //transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId && HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }
    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;


        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }
        HandleMovement();
        HandleInteractions();
        //ContainerCounter_Random.Instance.Slow += ContainerCounter_Slow;
        KitchenGameManager.Instance.Slow += ContainerCounter_Slow;


    }
    public bool IsWalking() {
        return isWalking;
    }
    private void HandleInteractions() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        //Physics.Raycast(transform.position, moveDir,out RaycastHit raycastHit, interactDistance);
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                // Has ClearCounter
                //clearCounter. Interact();
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
        //Debug.Log(selectedCounter);
    }

    private void HandleMovement() {

        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove) {
            // Khong the di chuyen theo huong moveDir

            // Thu chi di chuyen theo huong X
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove) {
                // Co the di chuyen chi theo truc X
                moveDir = moveDirX;
            } else {
                // Khong the di chuyen chi theo truc X

                // Thu chi di chuyen theo huong Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove) {
                    // Co the di chuyen chi theo truc Z
                    moveDir = moveDirZ;
                } else {
                    // Khong the di chuyen theo bat ky huong nao
                }
            }
        }


        if (canMove) {
            transform.position += moveDir * moveDistance;
        }
        //transform.position += inputVector; 
        //Bang voi transform.position transform.position + inputVector;

        // Di chuyen la khi Vecto khac 0

        isWalking = moveDir != Vector3.zero;


        //Quay dau theo di chuyen

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        //Debug.Log(Time.deltaTime);

    }
    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);

        }
    }
    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }
    public void ClearKitchenObject() {
        kitchenObject = null;
    }
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }

    public void SetMoveSpeed(float reducedSpeed) {
        moveSpeed = reducedSpeed;
    }
    public int GetSlowDownCount() {
        return slowDownCount;
    }

    public int GetslowDownThreshold() {
        return slowDownThreshold;
    }

}

