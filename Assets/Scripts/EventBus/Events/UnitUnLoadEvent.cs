public struct UnitUnLoadEvent : IEvent
{
    public ITransportable Unit { get; private set; }
    public ITransporter Transporter { get; private set; }

    public UnitUnLoadEvent(ITransportable unit, ITransporter transporter)
    {
        Unit = unit;
        Transporter = transporter;
    }
    
}