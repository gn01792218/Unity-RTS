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
        enterTime = Time.time; //設置開始採集的時間

        GatherableResources.Value.BeginGather(); //開始採集
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //如果採集時間到了，就返回
        if(GatherableResources.Value.resourceSO.BaseGatherTime + enterTime <= Time.time)
        {
            int amountGathered = GatherableResources.Value.EndGather();
            return Status.Success;
        }
        //時間還沒到繼續採集
        return Status.Running;
    }
}

