using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    // 引用現有的 Input System 資源和相機配置類別
    [SerializeField] InputActionAsset inputActionAsset; // 引用現有的 Input System 資源
    [SerializeField] Rigidbody camaraFollowTarget; // 相機跟隨的物件
    [SerializeField] CinemachineCamera cinemachineCamera; // 引用 Cinemachine 攝影機 
    [SerializeField] new Camera camera; // 引用 Unity 的 Camera 類別(給Racast使用), 把Main Camera拖進來
    [SerializeField] CamaraConfig camaraConfig; // 引用相機配置類別
    [SerializeField] private LayerMask selectableLayers; // 可被玩家選擇的圖層有哪些
    [SerializeField] private LayerMask interactableLayers; // 可和玩家互動的圖層有哪些 - 如礦點
    [SerializeField] private LayerMask moveableLayers; // 可供移動的圖層
    [SerializeField] private RectTransform selectionRect; // 框選的遮罩

    private Vector2 mouseStartPosition; // 滑鼠開始位置
    private InputAction moveAction;
    private Command activeCommand;
    private GameObject ghostInstance;
    private bool wasMouseDownOnUI; //滑鼠是否點在UI上面?
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
        Bus<SelectedEvent>.Subscribe(HandleSelected); // 訂閱選擇事件
        Bus<UnselectedEvent>.Subscribe(HandleUnselected); // 訂閱取消選擇事件
        Bus<SpawnUnitEvent>.Subscribe(HandleUnitSpawn); // 訂閱單位出生事件
        Bus<CommandSelectedEvent>.Subscribe(HandleCommandSelected); //訂閱指令事件
        Bus<UnitDeathEvent>.Subscribe(HandleUnitDeath); // 訂閱單位死亡事件
    }
    private void OnDestroy()
    {
        Bus<SelectedEvent>.Unsubscribe(HandleSelected);
        Bus<UnselectedEvent>.Unsubscribe(HandleUnselected);
        Bus<SpawnUnitEvent>.Unsubscribe(HandleUnitSpawn);
        Bus<CommandSelectedEvent>.Unsubscribe(HandleCommandSelected);
        Bus<UnitDeathEvent>.Unsubscribe(HandleUnitDeath);
    }
    private void HandleUnitSpawn(SpawnUnitEvent evt) => aliveUnits.Add(evt.SpawnUnit);//當單位出生時，將其添加到存活單位的集合中
    private void HandleUnitDeath(UnitDeathEvent evt)
    {
        aliveUnits.Remove(evt.Unit);
        selectUnits.Remove(evt.Unit);
    }
    private void HandleSelected(SelectedEvent evt)
    {
        //沒包含該單位時才+進去，避免重複+到
        if (!selectUnits.Contains(evt.SelectdObject))
        {
            selectUnits.Add(evt.SelectdObject); // 將被選到的物件添加到selectUnits中
        }
    }
    private void HandleUnselected(UnselectedEvent evt) => selectUnits.Remove(evt.SelectdObject); // 移除取消選中的物件
    private void HandleCommandSelected(CommandSelectedEvent evt)
    {
        activeCommand = evt.SelectdCommand;
        if (!activeCommand.RequiresClickToActive) //處理按下按鈕立即執行的指令, 如生產單位的指令
        {
            ActivateCommand(new RaycastHit());
        }
        else if (activeCommand is BuildBuildingCommand buildCommand) //處理建築類別的
        {
            ghostInstance = Instantiate(buildCommand.GhostPrefab);
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
        HandleGhost(); //建築指令按下去後的顯示
        HandleZooming();
        HandlePanning();
        HandleUnitSelection();
        HandleRightClick();
    }
    private void HandleUnitSelection()
    {
        if (selectionRect == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) // 如果左鍵按下
        {
            HandleLeftClickPressed();
        }
        else if (Mouse.current.leftButton.isPressed) // 如果左鍵持續按下
        {
            HandleLeftDrag();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame) // 如果左鍵釋放
        {
            HandleLeftClickRelease();
        }
    }
    private void HandleLeftClickPressed()
    {
        selectionRect.sizeDelta = Vector2.zero; // 重置框選遮罩的大小
        selectionRect.gameObject.SetActive(true); // 顯示框選遮罩
        mouseStartPosition = Mouse.current.position.ReadValue(); // 記錄滑鼠開始位置
                                                                 //每次滑鼠點下去時，都要重置drag選取的物件
        dragSelectedUnits.Clear();
        wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject(); //標示點到UI上
    }
    private void HandleLeftDrag()
    {
        if (activeCommand != null || wasMouseDownOnUI) return; //如果有指令 或是 滑鼠在UI介面上，就返回
        Bounds selectBounds = ResizeSelectRect();
        //僅針對活著的單位做處理
        foreach (Unit unit in aliveUnits)
        {
            // 獲取單位位置的螢幕座標
            Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
            //如果該單位的位置在框選範圍內
            if (selectBounds.Contains(unitPosition))
            {
                //先把他們儲存在dragSelectedUnits，因為要等滑鼠release後，才把他們+進selectedList中
                dragSelectedUnits.Add(unit); // 添加到框選的單位集合中
            }
        }
    }
    private void HandleLeftClickRelease()
    {
        //取消所有已選的單位
        //只有當沒有按下Shift鍵時，才會取消所有已選的單位
        //且沒有當前指令的時候
        //且沒有點在UI介面上
        if (!Keyboard.current.shiftKey.isPressed && activeCommand == null && !wasMouseDownOnUI)
        {
            DeselectAllUnits();
        }
        //處理左鍵單選,裡面會用雷射射線去選擇被點到的單位，
        //將被選到的也會被添加進selectUnits中
        HandleSelectSingle();
        //處理框選的單位
        SelectAllDragSelectedUnits();
        selectionRect.gameObject.SetActive(false); // 隱藏框選遮罩
    }
    private void SelectAllDragSelectedUnits()
    {
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
    private void HandleSelectSingle()
    {
        // 射線從相機發射到滑鼠位置
        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // 射線擊中物體  && 擊中物體是 ISelectable 接口的實現類型 && 沒有當前指令時
        if (activeCommand == null
            && Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
             && hit.collider.TryGetComponent(out ISelectable selectable))
        {
            // 調用 ISelectable 接口的 OnSelect 方法
            // 該方法會透過EventBus發送事件，並將自己傳遞給事件
            selectable.OnSelect();
        }
        else if (activeCommand != null // 射線擊中地板時 && 不是點在UI上  && 有當前指令時 
                && !wasMouseDownOnUI
                && Physics.Raycast(cameraRay, out hit, float.MaxValue, moveableLayers | interactableLayers))
        {
            ActivateCommand(hit);
        }
    }
    private void ActivateCommand(RaycastHit hit)
    {
        //記得清除建築指令留下的鬼魂instance，否則會一直跟著滑鼠哦
        if (ghostInstance != null)
        {
            DropGhostInstance();
        }
        List<CommandableUnit> units = selectUnits
                        .Where((unit) => unit is CommandableUnit) //Where 方法只會篩選符合條件的元素，但不會自動轉換元素的型態；
                        .Cast<CommandableUnit>() //使用 Cast 方法將篩選後的元素顯式轉換為指定的型態。
                        .ToList();

        for (int i = 0; i < units.Count; i++)
        {
            CommandContext context = new(units[i], hit, i);
            activeCommand.Handle(context);
        }
        activeCommand = null; //執行完之後清除掉, 代表已經執行完成
    }
    private void HandleRightClick()
    {
        if (selectUnits.Count == 0) return;
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, moveableLayers | interactableLayers))
            {
                //由於需要的agent radius在Unit類別裡面
                //這裡暫時轉換一下，之後要重構
                List<Unit> units = new(selectUnits.Count);
                foreach (ISelectable selectable in selectUnits) // 遍歷所有選中的物件
                {
                    if (selectable is Unit unit) // 如果是Unit類別的話
                    {
                        units.Add(unit); // 添加到列表中
                    }
                }
                for (int i = 0; i < units.Count; i++) // 遍歷所有選中的物件
                {
                    CommandContext context = new(units[i], hit, i);
                    foreach (Command command in GetAvailableCommands(units[i])) // 遍歷所有可用的指令
                    {
                        if (command.CanHandle(context)) // 如果該指令可以處理這個單位和擊中點
                        {
                            command.Handle(context); // 執行該指令
                            break; //執行到後就結束
                        }
                    }

                }
            }
        }
    }
    private List<Command> GetAvailableCommands(Unit unit)
    {
        OverrideCommandsCommand[] overrideCommandsCommands = unit.AvailableCommands
            .Where(command => command is OverrideCommandsCommand)
            .Cast<OverrideCommandsCommand>()
            .ToArray();
        List<Command> availableCommands = new();
        foreach(OverrideCommandsCommand overrideCommand in overrideCommandsCommands)
        {
            availableCommands.AddRange(overrideCommand.Commands
                .Where(command => command is not OverrideCommandsCommand)
                ); // 過濾掉 OverrideCommandsCommand 本身
        }
        availableCommands.AddRange(unit.AvailableCommands
                .Where(command => command is not OverrideCommandsCommand)
                ); // 添加非 OverrideCommandsCommand 的指令
        return availableCommands;
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
        Vector2 mouseMovement = GetMouseEdgePanVector();
        moveX += mouseMovement.x;
        moveY += mouseMovement.y;

        //velocity是剛體的速度，linearVelocity是線性速度
        // 這裡的速度是相機跟隨目標的速度
        camaraFollowTarget.linearVelocity = new Vector3(moveX, 0, moveY) * camaraConfig.CamaraMoveSpeed; // 更新相機位置
    }
    private void HandleGhost()
    {
        if (ghostInstance == null) return;
        //按下Esc的時候也要釋放
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DropGhostInstance();
            activeCommand = null;
            return;
        }
        //使用GetMouseMovement方法，更新ghostInstance的位置
        ghostInstance.transform.position = GetMouseGroundPosition();
    }
    private void DropGhostInstance()
    {
        Destroy(ghostInstance);
        ghostInstance = null;
    }
    private Vector3 GetMouseGroundPosition()
    {
        // 從螢幕滑鼠座標發射射線
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // 嘗試射到地面圖層
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, moveableLayers))
        {
            return hit.point; // 回傳地面交點
        }
        // 若沒射到地面，回傳預設值（可依需求調整）
        return Vector3.zero;
    }
    private Vector2 GetMouseEdgePanVector()
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
