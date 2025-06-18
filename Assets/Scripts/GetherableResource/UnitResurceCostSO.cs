using UnityEngine;

[CreateAssetMenu(fileName = "UnitResourceCostSO", menuName = "SO/UnitResourceCostSO", order = 5)]
public class UnitResourceCostSO : ScriptableObject
{
   [field: SerializeField] public int MineralCost { get; private set; } = 50;
   [field: SerializeField] public int GasCost { get; private set; } = 10;
}