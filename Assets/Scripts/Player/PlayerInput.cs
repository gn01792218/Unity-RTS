using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;
using NUnit.Framework;

public class PlayerInput : MonoBehaviour
{
    // 引用現有的 Input System 資源和相機配置類別
    [SerializeField] InputActionAsset inputActionAsset; // 引用現有的 Input System 資源
    [SerializeField] Rigidbody camaraFollowTarget; // 相機跟隨的物件
    [SerializeField] CinemachineCamera cinemachineCamera; // 引用 Cinemachine 攝影機 
    [SerializeField] new Camera camera; // 引用 Unity 的 Camera 類別(給Racast使用), 把Main Camera拖進來
    [SerializeField] CamaraConfig camaraConfig; // 引用相機配置類別
    [SerializeField] private LayerMask selectableLayers; // 可被玩家選擇的圖層有哪些
    [SerializeField] private LayerMask moveableLayers; // 可供移動的圖層
    [SerializeField] private RectTransform selectionRect; // 框選的遮罩

    private Vector2 mouseStartPosition; // 滑鼠開始位置
    private InputAction moveAction;
    private CinemachineFollow cinemachineFollow; // 引用 CinemachineFollow 組件
    private Vector3 startingTrackedObjectOffset; // 初始的 Tracked Object Offset
    private float zoomStartTime; // 縮放開始的時間
    private bool isZoomingIn = false; // 是否正在縮放
    private List<ISelectable> selectUnits = new List<ISelectable>(12); //儲存當前所選的物件
    private HashSet<Unit> dragSelectedUnits = new HashSet<Unit>(12); // 儲存框選時選中的單位
    private HashSet<Unit> aliveUnits = new HashSet<Unit>(100); // 儲存所有存活的單位

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

        selectionRect.gameObject.SetActive(false); // 隱藏框選遮罩
        //訂閱EventBus
        Bus<SelectedEvent>.OnEvent += HandleSelected; // 訂閱選擇事件
        Bus<UnselectedEvent>.OnEvent += HandleUnselected; // 訂閱取消選擇事件
        Bus<SpawnUnitEvent>.OnEvent += HandleUnitSpawn; // 訂閱單位出生事件
    }
    private void OnDestroy()
    {
        Bus<SelectedEvent>.Unsubscribe(HandleSelected);
        Bus<UnselectedEvent>.Unsubscribe(HandleUnselected);
        Bus<SpawnUnitEvent>.Unsubscribe(HandleUnitSpawn);
    }
    private void HandleUnitSpawn(SpawnUnitEvent evt) => aliveUnits.Add(evt.SpawnUnit);//當單位出生時，將其添加到存活單位的集合中
    private void HandleSelected(SelectedEvent evt) => selectUnits.Add(evt.SelectdObject); // 設置當前選中的物件
    private void HandleUnselected(UnselectedEvent evt) => selectUnits.Remove(evt.SelectdObject); // 移除取消選中的物件
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
        HandleRightClick();
        HandleDragSelection();
    }
    private void HandleDragSelection()
    {
        if (selectionRect == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) // 如果左鍵按下
        {
            selectionRect.sizeDelta = Vector2.zero; // 重置框選遮罩的大小
            selectionRect.gameObject.SetActive(true); // 顯示框選遮罩
            mouseStartPosition = Mouse.current.position.ReadValue(); // 記錄滑鼠開始位置
            //每次滑鼠點下去時，都要重置drag選取的物件
            dragSelectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.isPressed) // 如果左鍵持續按下
        {
            Bounds selectBounds = ResizeSelectRect();
            //僅針對活著的單位做處理
            foreach (Unit unit in aliveUnits)
            {
                 // 獲取單位位置的螢幕座標
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
                //如果該單位的位置在框選範圍內
                if(selectBounds.Contains(unitPosition)){
                    //先把他們儲存在dragSelectedUnits，因為要等滑鼠release後，才把他們+進selectedList中
                    dragSelectedUnits.Add(unit); // 添加到框選的單位集合中
                }

            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame) // 如果左鍵釋放
        {
            //取消所有已選的單位
            DeselectAllUnits();
            //選新的單位
            SelectAllDragSelectedUnits();
            selectionRect.gameObject.SetActive(false); // 隱藏框選遮罩
        }
    }
    private void SelectAllDragSelectedUnits(){
        foreach (ISelectable selectable in dragSelectedUnits)
        {
            selectable.OnSelect(); // 調用 ISelectable 接口的 OnSelect 方法
        }
    }
    private void DeselectAllUnits()
    {
        //不能直接這樣寫，因為OnDeselect會操作SelectUnits，把物件從selectUnits中移除。而疊代中的list，一邊移除裡面的東西會報錯!
        // foreach (ISelectable selectable in selectUnits)
        // {
        //     selectable.OnDeselect(); // 調用 ISelectable 接口的 OnDeselect 方法
        // }
        //所以要用for迴圈，從後往前刪除
        for (int i = selectUnits.Count - 1; i >= 0; i--)
        {
            selectUnits[i].OnDeselect(); // 調用 ISelectable 接口的 OnDeselect 方法
        }
    }
    private Bounds ResizeSelectRect()
    {
        Vector2 mouseEndPosition = Mouse.current.position.ReadValue(); // 取得當下的滑鼠位置
        Vector2 start = new Vector2(mouseStartPosition.x, mouseStartPosition.y); // 開始位置
        Vector2 end = new Vector2(mouseEndPosition.x, mouseEndPosition.y); // 結束位置
        selectionRect.anchoredPosition = (start + end) / 2; // 設置框選遮罩的pivit point位置
        selectionRect.sizeDelta = new Vector2(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y)); // 設置框選遮罩的大小
        return new Bounds(selectionRect.anchoredPosition, selectionRect.sizeDelta); // 返回框選遮罩的邊界
    }
    private void HandleLeftClick()
    {
        if (camera == null) return; // 如果相機未設置，則返回
        // 射線從相機發射到滑鼠位置
        // 檢測左鍵點擊事件 
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //暫時註解掉,有點難改
            // Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // if (selectUnits.Count != 0) // 如果已經有選中的物件
            // {
            //     selectUnit.OnDeselect(); // 調用 ISelectable 接口的 OnDeselect 方法
            // }
            // // 射線擊中物體  && 擊中物體是 ISelectable 接口的實現類型
            // if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
            //      && hit.collider.TryGetComponent(out ISelectable selectable))
            // {
            //     // 調用 ISelectable 接口的 OnSelect 方法
            //     // 該方法會透過EventBus發送事件，並將自己傳遞給事件
            //     selectable.OnSelect();
            // }
        }
    }
    private void HandleRightClick()
    {
        if (selectUnits.Count == 0) return;
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, moveableLayers))
            {
                foreach(IMoveable moveable in selectUnits) // 遍歷所有選中的物件
                {
                    moveable.Move(hit.point); // 移動到擊中點
                }
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
