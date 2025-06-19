using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [field: SerializeField] public RectTransform RectTransform { get; private set; }
    [field: SerializeField][Range(0, 1)] public float HoverDelay { get; private set; } = 0.5f; //滑鼠懸停多久後才顯示提示
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string tooltipText)
    {
        text.SetText(tooltipText);
        //調整提示框的大小以適應文本
        Vector2 preferredSize = text.GetPreferredValues(); //取得unity editor中的FontSize
        RectTransform.sizeDelta = new Vector2(preferredSize.x + 20, preferredSize.y + 20);
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