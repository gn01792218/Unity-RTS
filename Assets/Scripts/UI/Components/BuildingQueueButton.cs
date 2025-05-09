using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] //掛上此腳本會強制送一個Button唷!
public class BuildingQueueButton : MonoBehaviour, IUIElement<UnitSO, UnityAction>
{
    [SerializeField] private Image icon;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        Disable();
    }
    public void EnableFor(UnitSO unit, UnityAction callback)
    {
        button.onClick.RemoveAllListeners(); //先確保移除上一個事件監聽
        SetIcon(unit.Icon);
        button.interactable = true;
        button.onClick.AddListener(callback);
    }
    public void Disable()
    {
        SetIcon(null);
        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }

    private void SetIcon(Sprite icon) //和CommandButton的不大一樣
    {
        if (icon == null)
        {
            // this.icon.enabled = false;
            this.icon.gameObject.SetActive(false);
        }
        else
        {
            this.icon.gameObject.SetActive(true);
            this.icon.sprite = icon;
            // this.icon.enabled = true;
        }
    }
}
