using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] //掛上此腳本會強制送一個Button唷!
public class CommandButton : MonoBehaviour, IUIElement<Command, UnityAction>
{
    [SerializeField] private Image icon;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void EnableFor(Command command, UnityAction onClick)
    {
        SetIcon(command.Icon);
        button.interactable = true;
        button.onClick.AddListener(onClick);
    }
    public void Disable()
    {
        SetIcon(null);
        button.interactable = false;
        button.onClick.RemoveAllListeners();
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
}
