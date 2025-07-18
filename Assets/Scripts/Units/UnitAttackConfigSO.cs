using UnityEngine;
//fileName: UnitSO.cs
//menuName: 就是在Unit編輯器右鍵create後，會看到的選單路徑，例如這裡就是SO-->UnitSO
[CreateAssetMenu(fileName = "Unit Attack Config SO", menuName = "SO/Unit Attack Config SO", order = 7)]
public class UnitAttackConfigSO : ScriptableObject
{
    [field: SerializeField] public float AttackRange { get; private set; } = 5f;
    [field: SerializeField] public float AttackDelay { get; private set; } = 1f;
    [field: SerializeField] public int Damage { get; private set; } = 5;
    [field: SerializeField] public bool HasProjectileAttacks { get; private set; } = false; //是否屬於投射型(如手榴彈)的攻擊
    [field: SerializeField] public LayerMask DamageableLayers { get; private set; } // 可傷害的圖層有哪些

}
