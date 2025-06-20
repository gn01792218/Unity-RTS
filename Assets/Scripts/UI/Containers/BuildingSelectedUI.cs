using UnityEngine;

public class BuildingSelectedUI : MonoBehaviour, IUIElement<BuildingUnit>
{
    [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI; //單選時的UI
    [SerializeField] private BuildingBuildUnitUI buildingBuildUnitUI; //應該名為BuildingBuildUnitUI
    [SerializeField] private BuildingUnderConstructionUI buildingUnderConstructionUI;

    private BuildingUnit selectedBuilding;
    public void EnableFor(BuildingUnit building)
    {
        selectedBuilding = building;
        selectedBuilding.OnQueueUpdated -= OnBuildingQueueUpdate;
        selectedBuilding.OnQueueUpdated += OnBuildingQueueUpdate;
        //處理已經蓋好的建築UI顯示
        if (building.Progress.State == BuildingProgress.BuildingState.Completed)
        {
            buildingUnderConstructionUI.Disable();
            OnBuildingQueueUpdate();
        }
        else
        {
            buildingUnderConstructionUI.EnableFor(building); //顯示未蓋好的建築物Progress
            buildingBuildUnitUI.Disable();
            singleUnitSelectedUI.Disable();
            //註冊建築出生事件的監聽
            Bus<SpawnBuildingEvent>.Subscribe(HandleBuildingSpawn);
        }
    }
    public void Disable()
    {
        buildingBuildUnitUI.Disable();
        singleUnitSelectedUI.Disable();
        buildingUnderConstructionUI.Disable();
        Bus<SpawnBuildingEvent>.Unsubscribe(HandleBuildingSpawn);
        if (selectedBuilding != null)
        {
            selectedBuilding.OnQueueUpdated -= OnBuildingQueueUpdate;
            selectedBuilding = null;
        }
    }
    private void OnBuildingQueueUpdate(UnitSO[] _ = null)
    {
        if (selectedBuilding.QueueSize == 0)
        {
            buildingBuildUnitUI.Disable();
            singleUnitSelectedUI.EnableFor(selectedBuilding);
        }
        else
        {
            buildingBuildUnitUI.EnableFor(selectedBuilding);
            singleUnitSelectedUI.Disable();
        }
    }
    private void HandleBuildingSpawn(SpawnBuildingEvent evt)
    {
        if (evt.SpawnBuilding == selectedBuilding)
        {
            Bus<SpawnBuildingEvent>.Unsubscribe(HandleBuildingSpawn);
            OnBuildingQueueUpdate();
            buildingUnderConstructionUI.Disable();
        }
    }
}