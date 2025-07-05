using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class TransportUnit : Unit, ITransporter
{
    [field: SerializeField] public TransportConfigSO TransportConfigSO { get; private set; }

    public int UsedCapacity { get; private set; }
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
        throw new System.NotImplementedException();
    }

    public void LoadUnit(ITransportable unit)
    {
        if (!CanLoadUnit(unit)) return;
        //設置行為樹
        behaviorAgent.SetVariableValue("TargetGameObject", unit.Transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.LoadUnits);
    }

    public void LoadUnits(ITransportable[] units)
    {
        throw new System.NotImplementedException();
    }

    public bool UnloadAllUnits()
    {
        throw new System.NotImplementedException();
    }

    public bool UnloadUnit(ITransportable unit)
    {
        throw new System.NotImplementedException();
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
        UsedCapacity += targetGameObject.GetComponent<ITransportable>().TransportCapacityUsage; //3.更新容量
    }
}