using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

//專門用來顯示，單選到Unit時候的UI容器
//即最左邊的那一塊顯示區域
public class UnitIconUI : MonoBehaviour, IUIElement<CommandableUnit>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI healthText;

    private CommandableUnit commandableUnit;
    private const string HEALTH_TEXT_FORMAT = "{0}/{1}"; //格式化顯示當前生命值/最大生命值
    public void EnableFor(CommandableUnit unit)
    {
        gameObject.SetActive(true); //開啟這個UI容器
        icon.sprite = unit.unitSO.Icon;
        UpdateHealthText(unit);
        commandableUnit = unit;

        //註冊血量更新事件
        commandableUnit.OnHealthUpdated -= OnHealthUpdated;
        commandableUnit.OnHealthUpdated += OnHealthUpdated;
    }

    private void UpdateHealthText(CommandableUnit item)
    {
        healthText.SetText(HEALTH_TEXT_FORMAT, item.CurrentHealth, item.MaxHealth);
    }
    public void Disable()
    {
        gameObject.SetActive(false);

        if (commandableUnit != null)
        {
            commandableUnit.OnHealthUpdated -= OnHealthUpdated;
            commandableUnit = null;
        }
    }

    private void OnHealthUpdated(CommandableUnit unit, int _, int currentHeal)
    {
        healthText.SetText(HEALTH_TEXT_FORMAT, currentHeal, unit.MaxHealth);
    }

}