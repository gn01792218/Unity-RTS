using System;
using UnityEngine;

public class ProgressBar : MonoBehaviour, IUIElement
{
    [SerializeField] private RectTransform mask; //進度條的讀條遮罩
    [SerializeField] private Vector2 padding = new(9,8); //進度條的padding

    private RectTransform maskParentTransform;
    private void Awake()
    {
        if(mask == null)
        {
            Debug.Log("Progress Bar miss mask");
            return;
        }
        maskParentTransform = mask.parent.GetComponent<RectTransform>();
    }
    public void Disable() //目前沒用
    {
        
    }

    public void EnableFor() //目前沒用
    {
        // SetProgress(100);
    }
    public void SetProgress(float progress)
    {
        Vector2 parentSize = maskParentTransform.sizeDelta;
        Vector2 targetSize = parentSize - padding * 2; //要扣掉padding，然後*2是因為padding要算兩邊的

        targetSize.x *= Mathf.Clamp01(progress); //將數值鎖定在0和1之間
        // Debug.Log("x的進度"+targetSize.x);
        
        mask.offsetMin = padding;
        mask.offsetMax = new Vector2(padding.x + targetSize.x - parentSize.x , -padding.y);
        // Debug.Log("最大值"+mask.offsetMax.x);
    }
}