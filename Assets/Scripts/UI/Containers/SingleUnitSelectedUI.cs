using TMPro;
using UnityEngine;

public class SingleUnitSelectedUI : MonoBehaviour, IUIElement<CommandableUnit>
{
    [SerializeField] private TextMeshProUGUI unitName;

    public void EnableFor(CommandableUnit item)
    {
        gameObject.SetActive(true);
        unitName.SetText(item.unitSO.UnitName);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}