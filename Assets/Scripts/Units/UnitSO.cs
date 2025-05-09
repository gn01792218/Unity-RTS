using UnityEngine;
//fileName: UnitSO.cs
//menuName: 就是在Unit編輯器右鍵create後，會看到的選單路徑，例如這裡就是SO-->UnitSO
[CreateAssetMenu(fileName = "UnitSO", menuName = "SO/UnitSo")]
public class UnitSO : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float BuildTime { get; private set; }
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [field: SerializeField] public Sprite Icon { get; private set; }
}
