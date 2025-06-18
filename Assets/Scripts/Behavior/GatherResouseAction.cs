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
    [SerializeReference] public BlackboardVariable<GatherableResurceSO> GatherableResourceSO; //由於GatherableResource會在沒有資源時，自動摧毀，所以得將其SO儲存，才不會Null

    private Animator animator; //動畫控制器
    private float enterTime; //碰到資源的時間點

    protected override Status OnStart()
    {
        if(GatherableResources.Value == null)
        {
            return Status.Failure;
        }   
        enterTime = Time.time; //設置開始採集的時間
        if(Unit.Value.TryGetComponent(out animator))
        {
            animator.SetBool(AnimationConstants.IS_GATHERING_ID, true);
        }
        GatherableResources.Value.BeginGather(); //開始採集
        GatherableResourceSO.Value = GatherableResources.Value.resourceSO; //儲存資源的ScriptableObject
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
        if (animator != null)
        {
            animator.SetBool(AnimationConstants.IS_GATHERING_ID, false); //停止採集動畫
        }
        if (GatherableResources.Value == null) return;
        if (CurrentStatus == Status.Success)
        {
            Amount.Value = GatherableResources.Value.EndGather(); //結束採集，並獲取採集的數量
        }
        else
        {
            GatherableResources.Value.AbortGather(); //如果採集還在進行中，則中止採集
        }
    }
}

