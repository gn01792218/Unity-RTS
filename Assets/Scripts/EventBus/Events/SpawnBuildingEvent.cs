public struct SpawnBuildingEvent:IEvent
{
    public BuildingUnit SpawnBuilding {get; private set;} //出生的單位
    public SpawnBuildingEvent(BuildingUnit budling)
    {
        SpawnBuilding = budling;
    }
}