using UnityEngine;
//fileName: UnitSO.cs
//menuName: 就是在Unit編輯器右鍵create後，會看到的選單路徑，例如這裡就是SO-->UnitSO
[CreateAssetMenu(fileName = "Unit AOE Attack Config SO", menuName = "SO/Unit AOE Attack Config SO", order = 7)]
public class UnitAOEAttackConfigSO : ScriptableObject
{
    [field: SerializeField] public float AOERasius { get; private set; } = 2;
    [field: SerializeField] public int MaxAOEHitNumber { get; private set; } = 5;//AOE傷害的最大傷害數量
    [field: SerializeField] public LayerMask DamageableLayers { get; private set; } // AOE可以傷害的圖層有哪些
}
