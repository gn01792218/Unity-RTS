using UnityEngine;
//fileName: UnitSO.cs
//menuName: 就是在Unit編輯器右鍵create後，會看到的選單路徑，例如這裡就是SO-->UnitSO
[CreateAssetMenu(fileName = "Unit Attack Config SO", menuName = "SO/Unit Attack Config SO", order = 7)]
public class UnitAttackConfigSO : ScriptableObject
{
    [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
    [field: SerializeField] public float AttackDelay { get; private set; } = 1f;
    [field: SerializeField] public int Damage { get; private set; } = 5;
}
