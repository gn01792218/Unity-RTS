using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class TransportUnit : Unit, ITransporter
{
    [field: SerializeField] public TransportConfigSO TransportConfigSO { get; private set; }

    public int UsedCapacity { get; private set; }
    private List<ITransportable> loadedUnits = new(8);
    protected override void Start()
    {
        base.Start();

        //註冊監聽行為樹事件
        if (behaviorAgent.GetVariable("LoadUnitEventChannel", out BlackboardVariable<LoadUnitEventChannel> loadEvent))
        {
            loadEvent.Value.Event += HandleLoadUnit;
        }
    }
    public List<ITransportable> GetLoadedUnits()
    {
        return loadedUnits.ToList();
    }

    public void LoadUnit(ITransportable unit)
    {
        if (!CanLoadUnit(unit)) return;
        //先取得行為數列表，將該單位加入列表中
        if (behaviorAgent.GetVariable("TargetUnits", out BlackboardVariable<List<GameObject>> targetUnits))
        {
            targetUnits.Value.Add(unit.Transform.gameObject);
            behaviorAgent.SetVariableValue("TargetUnits", targetUnits.Value);
        }
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.LoadUnits);
    }

    public void LoadUnits(ITransportable[] units)
    {

    }

    public bool UnloadAllUnits()
    {
        for (int i = loadedUnits.Count - 1; i >= 0; i--)
        {
            UnloadUnit(loadedUnits[i]);
        }
        return true;
    }

    public bool UnloadUnit(ITransportable unit)
    {
        //查詢該單位可以行走的Layer
        NavMeshQueryFilter queryFilter = new()
        {
            areaMask = unit.navMeshAgent.areaMask, 
            agentTypeID = unit.navMeshAgent.agentTypeID
        };
        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit raycastHit,
            float.MaxValue,
            TransportConfigSO.SafeDropLayers)
            && NavMesh.SamplePosition(raycastHit.point, out NavMeshHit hit, 1, queryFilter))
        {
            UsedCapacity -= unit.TransportCapacityUsage;
            unit.Transform.SetParent(null); //放離開
            unit.Transform.position = hit.position; //放到目標點為上
            unit.Transform.gameObject.SetActive(true); //啟動該unit
            unit.navMeshAgent.Warp(hit.position); //讓unit的MeshAgent和position同步打包移動(做瞬間移動)
            if (unit is IMoveable moveable)
            {
                moveable.MoveToLocation(hit.position);
            }
            loadedUnits.Remove(unit);
            return true;
        }
        return false;
    }
    public bool CanLoadUnit(ITransportable unit)
    {
        return UsedCapacity + unit.TransportCapacityUsage <= TransportConfigSO.Capacity;
    }
    private void HandleLoadUnit(GameObject self, GameObject targetGameObject)
    {
        Debug.Log($"Load {targetGameObject.name}");
        targetGameObject.SetActive(false); //1.先禁用該單位
        targetGameObject.transform.SetParent(self.transform); //2.貼到我們運輸單位下
        ITransportable transportable = targetGameObject.GetComponent<ITransportable>();
        UsedCapacity += transportable.TransportCapacityUsage; //3.更新容量
        loadedUnits.Add(transportable); //4.加到列表中

        //從行為樹列表中移除
        if (behaviorAgent.GetVariable("TargetUnits", out BlackboardVariable<List<GameObject>> targetUnits))
        {
            targetUnits.Value.Remove(targetGameObject);
            behaviorAgent.SetVariableValue("TargetUnits", targetUnits.Value);
        }

        //如果使用量已經操過了容量
        if (UsedCapacity >= TransportConfigSO.Capacity)
        {
            behaviorAgent.SetVariableValue("Command", UnitCommandsEnum.Stop);
            behaviorAgent.SetVariableValue("TargetUnits", new List<GameObject>(TransportConfigSO.Capacity));
        }
    }
}