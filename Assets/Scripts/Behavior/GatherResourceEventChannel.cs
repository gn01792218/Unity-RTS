using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/GatherResourceEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "GatherResourceEventChannel", message: "[Self] gatherss [Amount] [Resources]", category: "Events", id: "9a41b51b410900884daf80e269c344eb")]
public partial class GatherResourceEventChannel : EventChannelBase
{
    public delegate void GatherResourceEventChannelEventHandler(GameObject Self, int Amount, GatherableResurceSO Resources);
    public event GatherResourceEventChannelEventHandler Event; 

    public void SendEventMessage(GameObject Self, int Amount, GatherableResurceSO Resources)
    {
        Event?.Invoke(Self, Amount, Resources);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<int> AmountBlackboardVariable = messageData[1] as BlackboardVariable<int>;
        var Amount = AmountBlackboardVariable != null ? AmountBlackboardVariable.Value : default(int);

        BlackboardVariable<GatherableResurceSO> ResourcesBlackboardVariable = messageData[2] as BlackboardVariable<GatherableResurceSO>;
        var Resources = ResourcesBlackboardVariable != null ? ResourcesBlackboardVariable.Value : default(GatherableResurceSO);

        Event?.Invoke(Self, Amount, Resources);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        GatherResourceEventChannelEventHandler del = (Self, Amount, Resources) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Self;

            BlackboardVariable<int> var1 = vars[1] as BlackboardVariable<int>;
            if(var1 != null)
                var1.Value = Amount;

            BlackboardVariable<GatherableResurceSO> var2 = vars[2] as BlackboardVariable<GatherableResurceSO>;
            if(var2 != null)
                var2.Value = Resources;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as GatherResourceEventChannelEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as GatherResourceEventChannelEventHandler;
    }
}

