using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] //掛上此腳本會強制送一個Button唷!
public class CommandButton : MonoBehaviour, IUIElement<Command, UnityAction>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Tooltip tooltip;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        Disable();
    }
    public void EnableFor(Command command, UnityAction onClick)
    {
        button.onClick.RemoveAllListeners(); //確保清除不必要的監聽，因為可能有沒有call Disable的情況
        SetIcon(command.Icon);
        button.interactable = command.IsAvailable();
        button.onClick.AddListener(onClick);

        if(tooltip != null)
        {
            tooltip.SetText(command.name); //假設指令的名稱就是提示文字
        }
        else
        {
            Debug.LogWarning("Tooltip is not assigned for CommandButton.");
        }
    }
    public void Disable()
    {
        SetIcon(null);
        button.interactable = false;
        button.onClick.RemoveAllListeners();
        HideTooltip();
    }

    private void HideTooltip()
    {
        if (tooltip != null) tooltip.Hide();
        CancelInvoke(nameof(ShowTooltip)); //確保取消任何可能的提示顯示
    }

    private void SetIcon(Sprite icon)
    {
        if (icon == null)
        {
            this.icon.enabled = false;
        }
        else
        {
            this.icon.sprite = icon;
            this.icon.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData _)
    {
        //500毫秒後(即0.5f)，註冊function來顯示提示
        Invoke(nameof(ShowTooltip), 0.5f);
    }

    public void OnPointerExit(PointerEventData _)
    {
        HideTooltip();
    }
    private void ShowTooltip()
    {
        if (tooltip != null) tooltip.Show();
    }
}
