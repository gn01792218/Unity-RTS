using UnityEngine;
[CreateAssetMenu(fileName = "Transport Config SO", menuName = "SO/Transport Config SO", order = 6)]
public class TransportConfigSO : ScriptableObject
{
    [field: SerializeField] public int Capacity { get; private set; } 
    [field: SerializeField] public TransportSizeEnum TransportSize { get; private set; }
    [field: SerializeField] public LayerMask SafeDropLayers{ get; private set; } //可放置的圖層

    //後面的數字代表容量單位，Small的表示佔據1個單位
    public enum TransportSizeEnum
    {
        Small = 1,
        Medium = 3,
        Large = 4,
    }
}