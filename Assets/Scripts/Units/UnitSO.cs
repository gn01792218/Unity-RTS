using UnityEngine;
//fileName: UnitSO.cs
//menuName: 就是在Unit編輯器右鍵create後，會看到的選單路徑，例如這裡就是SO-->UnitSO
[CreateAssetMenu(fileName = "UnitSO", menuName = "SO/UnitSo")]
public class UnitSO : ScriptableObject
{
    [field: SerializeField] public string UnitName { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float BuildTime { get; private set; }
    [field: SerializeField] public int Health { get; private set; } = 100;
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public UnitResourceCostSO ResourceCostSO { get; private set; }
    [field: SerializeField] public UnitAttackConfigSO AttackConfigSO { get; private set; } //這個應該要移動到IAttacker中!!!!
}
