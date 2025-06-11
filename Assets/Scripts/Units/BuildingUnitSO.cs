using UnityEngine;
[CreateAssetMenu(fileName = "BuildingUnitSO", menuName = "SO/BuildingUnitSo")]
public class BuildingUnitSO : UnitSO
{
    [field: SerializeField] public Material PlacementMaterial { get; private set; }
}
