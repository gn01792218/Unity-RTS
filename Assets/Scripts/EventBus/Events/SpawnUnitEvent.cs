public struct SpawnUnitEvent:IEvent
{
    public Unit SpawnUnit {get; private set;} //出生的單位
    public SpawnUnitEvent(Unit unit)
    {
        SpawnUnit = unit;
    }
}