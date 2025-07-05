using System.Collections.Generic;
using UnityEngine;

public interface ITransporter
{
    public Transform Transform { get; }
    public TransportConfigSO TransportConfigSO { get; }
    public int UsedCapacity { get; } //已使用的容量

    public List<ITransportable> GetLoadedUnits();

    public void LoadUnit(ITransportable unit);
    public void LoadUnits(ITransportable[] units);

    public bool UnloadUnit(ITransportable unit);
    public bool UnloadAllUnits();
    public bool CanLoadUnit(ITransportable unit);

}