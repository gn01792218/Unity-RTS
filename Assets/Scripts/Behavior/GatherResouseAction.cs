using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Gather Resouse", story: "[Unit] gathers [Amount] resources from [GatherableResources]", category: "Action/Units", id: "641913cbf74d71f2ec942098e754999f")]
public partial class GatherResouseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Unit;
    [SerializeReference] public BlackboardVariable<int> Amount;
    [SerializeReference] public BlackboardVariable<GatherableResource> GatherableResources;

    private float enterTime; //碰到資源的時間點

    protected override Status OnStart()
    {
        if(GatherableResources.Value == null)
        {
            return Status.Failure;
        }   
        enterTime = Time.time; //設置開始採集的時間

        GatherableResources.Value.BeginGather(); //開始採集
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //如果採集時間到了，就返回
        if (GatherableResources.Value.resourceSO.BaseGatherTime + enterTime <= Time.time)
        {
            // Amount.Value = GatherableResources.Value.EndGather();
            // Debug.Log($"採集資源: {GatherableResources.Value.name} 數量: {Amount.Value}");
            return Status.Success;
        }
        //時間還沒到繼續採集
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if(GatherableResources.Value == null) return;
        if (CurrentStatus == Status.Success)
        {
            Amount.Value = GatherableResources.Value.EndGather(); //結束採集，並獲取採集的數量
            Debug.Log($"採集資源: {GatherableResources.Value.name} 數量: {Amount.Value}");
        }
        else
        {
            GatherableResources.Value.AbortGather(); //如果採集還在進行中，則中止採集
            Debug.Log($"中止採集資源: {GatherableResources.Value.name}，因為採集失敗或被取消");  
        }
    }
}

