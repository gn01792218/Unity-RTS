using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitTransportUI : MonoBehaviour, IUIElement<ITransporter>
{
    [SerializeField] private LoadedUnitButton[] loadedUnitButtons;
    [SerializeField] private TextMeshProUGUI capacityText;

    private ITransporter transporter;
    private const string CAPACITY_TEXT_FORMAT = "{0} / {1}";

    public void EnableFor(ITransporter item)
    {
        gameObject.SetActive(true);
        transporter = item;
        capacityText.SetText(string.Format(CAPACITY_TEXT_FORMAT, transporter.UsedCapacity, transporter.TransportConfigSO.Capacity));
        List<ITransportable> loadedUnits = transporter.GetLoadedUnits();

        //依照loadedUnits啟用按鈕
        for (int i = 0; i < loadedUnitButtons.Length; i++)
        {
            if (i < loadedUnits.Count)
            {
                int index = i;//要記錄起來，否則i直接給callback的話得到是最後一個值!
                loadedUnitButtons[i].EnableFor(loadedUnits[i], () => HandleLoadedButtonClick(loadedUnits[index], index));
            }
            else
            {
                loadedUnitButtons[i].Disable(); //剩下的要禁用
            }
        }
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        foreach (LoadedUnitButton button in loadedUnitButtons)
        {
            button.Disable();
        }
    }
    private void HandleLoadedButtonClick(ITransportable transportable, int index)
    {
        // 卸載該單位 : 
        // UnloadUnit方法會卸載該單位，卸載成功回傳true
        if (transporter.UnloadUnit(transportable))
        {
            loadedUnitButtons[index].Disable();
        }
    }

}