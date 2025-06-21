using UnityEngine;

public interface IBuildingBuilder
{
    public bool IsBuilding { get; }
    public GameObject BuildBuilding(BuildingUnitSO building, Vector3 targetLocation);
    public void ResumeBuilding(BuildingUnit building);
    public void CancelBuilding();
}