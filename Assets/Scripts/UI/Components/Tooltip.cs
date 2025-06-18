using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [field: SerializeField][Range(0, 1)] public float HoverDelay { get; private set; } = 0.5f; //滑鼠懸停多久後才顯示提示
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string tooltipText)
    {
        text.SetText(tooltipText);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}