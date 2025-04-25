using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset; // 引用現有的 Input System 資源
    [SerializeField] Transform camaraFollowTarget; // 相機跟隨的物件
    [SerializeField] float camaraMoveSpeed = 20f; // 移動的速度

    private InputAction moveAction;

    private void Awake()    
    {
        // 從資源中獲取 Player Action Map 的 Move Action
        moveAction = inputActionAsset.FindActionMap("Player").FindAction("Move");
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
        Vector2 moveInput = moveAction.ReadValue<Vector2>(); // 讀取移動輸入
        float moveX = moveInput.x;
        float moveY = moveInput.y;

        Debug.Log($"X: {moveX}, Y: {moveY}"); // 顯示使用者的移動值

        camaraFollowTarget.transform.position += new Vector3(moveX, 0, moveY) * camaraMoveSpeed * Time.deltaTime; // 更新相機位置
    }
}
