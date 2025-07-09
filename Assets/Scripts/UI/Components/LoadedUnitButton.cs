using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadedUnitButton : MonoBehaviour, IUIElement<ITransportable, UnityAction>
{
    [SerializeField] private Image icon;

    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        Disable();
    }

    public void EnableFor(ITransportable item, UnityAction callback)
    {
        button.onClick.RemoveAllListeners(); //確保清除其他監聽
        gameObject.SetActive(true);
        icon.sprite = item.Icon;
        button.onClick.AddListener(callback);
    }
    public void Disable()
    {
        button.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

}