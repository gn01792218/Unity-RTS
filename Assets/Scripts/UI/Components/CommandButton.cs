using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] //掛上此腳本會強制送一個Button唷!
public class CommandButton : MonoBehaviour, IUIElement<Command,IEnumerable<CommandableUnit> ,UnityAction>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Tooltip tooltip;

    private RectTransform rectTransform;
    private Button button;
    private bool isActive;
    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        Disable();
    }
    public void EnableFor(Command command, IEnumerable<CommandableUnit> selectedUnits,UnityAction onClick)
    {
        button.onClick.RemoveAllListeners(); //確保清除不必要的監聽，因為可能有沒有call Disable的情況
        SetIcon(command.Icon);
        button.interactable = selectedUnits.Any(unit=> command.IsAvailable(new CommandContext(unit, new RaycastHit())));
        button.onClick.AddListener(onClick);
        isActive = true;

        if (tooltip != null)
        {
            tooltip.SetText(GetTooltipText(command)); //假設指令的名稱就是提示文字
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
        isActive = false;
        HideTooltip();
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
        if (isActive) Invoke(nameof(ShowTooltip), 0.5f);
    }

    public void OnPointerExit(PointerEventData _)
    {
        HideTooltip();
    }
    private void ShowTooltip()
    {
        if (tooltip != null)
        {
            tooltip.Show();
            //將提示框位置設置在按鈕的上方並且尾部對其按鈕的右邊
            tooltip.RectTransform.position = new Vector2(
                rectTransform.position.x,
                rectTransform.position.y + tooltip.RectTransform.rect.height / 2f
            );
        }
    }
    private void HideTooltip()
    {
        if (tooltip != null) tooltip.Hide();
        CancelInvoke(nameof(ShowTooltip)); //確保取消任何可能的提示顯示
    }
    private string GetTooltipText(Command command)
    {
        //這裡可以根據需要返回更詳細的提示文字
        string tooltipText = command.CommandName; //假設指令的名稱就是提示文字
        UnitResourceCostSO resourceCostSO = command switch
        {
            BuildUnitCommand buildUnitCommand => buildUnitCommand.unitSO.ResourceCostSO,
            BuildBuildingCommand buildBuildingCommand => buildBuildingCommand.buildingSo.ResourceCostSO,
            _ => null  // 處理其他所有情況
        };
        if (!resourceCostSO) return tooltipText;
        if (resourceCostSO.MineralCost > 0) tooltipText += $"\nMineral Cost: {resourceCostSO.MineralCost}";
        if (resourceCostSO.GasCost > 0) tooltipText += $"\nGas Cost: {resourceCostSO.GasCost}";
        return tooltipText;
    }
}
