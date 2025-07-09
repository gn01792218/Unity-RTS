using UnityEngine;
using UnityEngine.AI;

//可被運送的介面
public interface ITransportable
{
    public Transform Transform { get; }
    public int TransportCapacityUsage{ get; } //占多少容量
    public NavMeshAgent navMeshAgent{ get; }
    public Sprite Icon { get; }

    public void LoadInto(ITransporter transporter);
}