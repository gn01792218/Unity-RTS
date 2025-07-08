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