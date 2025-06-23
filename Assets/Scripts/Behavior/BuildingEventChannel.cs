using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/BuildingEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "BuildingEventChannel", message: "[Self] [BuildingEventEnum] on [BuildingUnit]", category: "Events", id: "0b1f43440530ff6ea83dcdcb4802b58d")]
public partial class BuildingEventChannel : EventChannelBase
{
    public delegate void BuildingEventChannelEventHandler(GameObject Self, BuildingEventEnum BuildingEventEnum, BuildingUnit BuildingUnit);
    public event BuildingEventChannelEventHandler Event; 

    public void SendEventMessage(GameObject Self, BuildingEventEnum BuildingEventEnum, BuildingUnit BuildingUnit)
    {
        Event?.Invoke(Self, BuildingEventEnum, BuildingUnit);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<BuildingEventEnum> BuildingEventEnumBlackboardVariable = messageData[1] as BlackboardVariable<BuildingEventEnum>;
        var BuildingEventEnum = BuildingEventEnumBlackboardVariable != null ? BuildingEventEnumBlackboardVariable.Value : default(BuildingEventEnum);

        BlackboardVariable<BuildingUnit> BuildingUnitBlackboardVariable = messageData[2] as BlackboardVariable<BuildingUnit>;
        var BuildingUnit = BuildingUnitBlackboardVariable != null ? BuildingUnitBlackboardVariable.Value : default(BuildingUnit);

        Event?.Invoke(Self, BuildingEventEnum, BuildingUnit);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        BuildingEventChannelEventHandler del = (Self, BuildingEventEnum, BuildingUnit) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Self;

            BlackboardVariable<BuildingEventEnum> var1 = vars[1] as BlackboardVariable<BuildingEventEnum>;
            if(var1 != null)
                var1.Value = BuildingEventEnum;

            BlackboardVariable<BuildingUnit> var2 = vars[2] as BlackboardVariable<BuildingUnit>;
            if(var2 != null)
                var2.Value = BuildingUnit;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as BuildingEventChannelEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as BuildingEventChannelEventHandler;
    }
}

