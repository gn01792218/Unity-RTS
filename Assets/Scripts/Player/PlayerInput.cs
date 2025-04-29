using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerInput : MonoBehaviour
{
    // 引用現有的 Input System 資源和相機配置類別
    [SerializeField] InputActionAsset inputActionAsset; // 引用現有的 Input System 資源
    [SerializeField] Rigidbody camaraFollowTarget; // 相機跟隨的物件
    [SerializeField] CinemachineCamera cinemachineCamera; // 引用 Cinemachine 攝影機 
    [SerializeField] new Camera camera; // 引用 Unity 的 Camera 類別(給Racast使用), 把Main Camera拖進來
    [SerializeField] CamaraConfig camaraConfig; // 引用相機配置類別
    [SerializeField] private LayerMask selectableLayers; // 可被玩家選擇的圖層有哪些

    private InputAction moveAction;
    private CinemachineFollow cinemachineFollow; // 引用 CinemachineFollow 組件
    private Vector3 startingTrackedObjectOffset; // 初始的 Tracked Object Offset
    private float zoomStartTime; // 縮放開始的時間
    private bool isZoomingIn = false; // 是否正在縮放
    private ISelectable selectUnit; //儲存當前所選的物件

    private void Awake()
    {
        // 從資源中獲取 Player Action Map 的 Move Action
        moveAction = inputActionAsset.FindActionMap("Player").FindAction("Move");

        // 獲取 Cinemachine 的初始 Tracked Object Offset
        if (cinemachineCamera != null)
        {
            InitCinemachineFollow();
        }
        else
        {
            Debug.LogError("CinemachineCamera is not assigned!");
        }
    }

    private void InitCinemachineFollow()
    {
        cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
        if (cinemachineFollow != null)
        {
            startingTrackedObjectOffset = cinemachineFollow.FollowOffset;
        }
        else
        {
            Debug.LogError("CinemachineCamera does not have a CinemachineFollow component!");
        }
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        HandleZooming();
        HandlePanning();
        HandleLeftClick();
    }
    private void HandleLeftClick()
    {
        if (camera == null) return; // 如果相機未設置，則返回
        // 射線從相機發射到滑鼠位置
        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // 檢測左鍵點擊事件 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (selectUnit != null) // 如果已經有選中的物件
            {
                selectUnit.OnDeselect(); // 調用 ISelectable 接口的 OnDeselect 方法
                selectUnit = null; // 清除選中的物件
            }
            // 射線擊中物體  && 擊中物體是 ISelectable 接口的實現類型
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
                 && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.OnSelect(); // 調用 ISelectable 接口的 OnSelect 方法
                selectUnit = selectable; // 設置當前選中的物件
            }
        }
    }

    private void HandleZooming()
    {
        if (cinemachineCamera == null) return;

        if (cinemachineFollow == null) return;

        // 檢測是否按下或釋放縮放按鍵（例如 "G" 鍵）
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            isZoomingIn = true;
            zoomStartTime = Time.time;
        }
        else if (Keyboard.current.gKey.wasReleasedThisFrame)
        {
            isZoomingIn = false;
            zoomStartTime = Time.time;
        }

        // 計算目標 Tracked Object Offset
        Vector3 targetOffset = startingTrackedObjectOffset;
        if (isZoomingIn)
        {
            targetOffset.y = camaraConfig.MinZoomDistance; // 縮放到最小距離
        }
        else
        {
            targetOffset.y = camaraConfig.MaxZoomDistance; // 恢復到最大距離
        }

        // 使用 Slerp 平滑過渡 Tracked Object Offset
        //當前時間-開始zoom的時間乘以縮放速度，並限制在0到1之間
        float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * camaraConfig.ZoomSpeed); //用Clamp01限制時間在0到1之間
        cinemachineFollow.FollowOffset = Vector3.Slerp(startingTrackedObjectOffset, targetOffset, zoomTime);
    }

    private void HandlePanning()
    {
        // 讀取移動輸入
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float moveX = moveInput.x;
        float moveY = moveInput.y;

        // 取得滑鼠移動輸入
        // 用+= 才不會覆蓋掉鍵盤輸入的值
        Vector2 mouseMovement = GetMouseMovement();
        moveX += mouseMovement.x;
        moveY += mouseMovement.y;

        //velocity是剛體的速度，linearVelocity是線性速度
        // 這裡的速度是相機跟隨目標的速度
        camaraFollowTarget.linearVelocity = new Vector3(moveX, 0, moveY) * camaraConfig.CamaraMoveSpeed; // 更新相機位置
    }

    private Vector2 GetMouseMovement()
    {
        if (!camaraConfig.EnableEdgePan) return Vector2.zero; // 如果未啟用邊緣平移，返回零向量

        Vector2 mousePosition = Mouse.current.position.ReadValue(); // 讀取滑鼠位置
        Vector2 screenSize = new(Screen.width, Screen.height); // 螢幕大小

        // 計算滑鼠位置與螢幕邊緣的距離
        float distanceToLeft = mousePosition.x; // 距離左邊緣
        float distanceToRight = screenSize.x - mousePosition.x; // 距離右邊緣
        float distanceToBottom = mousePosition.y; // 距離下邊緣
        float distanceToTop = screenSize.y - mousePosition.y; // 距離上邊緣

        float moveX = 0f;
        float moveY = 0f;

        // 如果滑鼠在邊緣，則計算平移速度
        if (distanceToLeft < camaraConfig.EdgePanDistance)
        {
            moveX -= camaraConfig.EdgePanSpeed * Time.deltaTime; // 向左平移
        }
        else if (distanceToRight < camaraConfig.EdgePanDistance)
        {
            moveX += camaraConfig.EdgePanSpeed * Time.deltaTime; // 向右平移
        }

        if (distanceToBottom < camaraConfig.EdgePanDistance)
        {
            moveY -= camaraConfig.EdgePanSpeed * Time.deltaTime; // 向下平移
        }
        else if (distanceToTop < camaraConfig.EdgePanDistance)
        {
            moveY += camaraConfig.EdgePanSpeed * Time.deltaTime; // 向上平移
        }

        return new Vector2(moveX, moveY); // 返回滑鼠移動的向量
    }
}
