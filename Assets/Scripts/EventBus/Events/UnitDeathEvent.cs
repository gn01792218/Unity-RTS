public struct UnitDeathEvent:IEvent
{
    public Unit Unit {get; private set;} //出生的單位
    public UnitDeathEvent(Unit unit)
    {
        Unit = unit;
    }
}