using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Target Location", story: "[Agent] moves to [TargetLocation]", category: "Action/Navigation", id: "2855eecac6be09a7bcddee5755de70d6")]
public partial class MoveToTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    private NavMeshAgent agent;

    protected override Status OnStart()
    {
        //沒有agent的話就回傳失敗
        if(!Agent.Value.TryGetComponent(out agent)) return Status.Failure;

        //當到達指定距離時，回傳成功
        if(Vector3.Distance(agent.transform.position, TargetLocation.Value) <= agent.stoppingDistance)
        {
            return Status.Success; //回傳Success，就不會再進入OnUpdate了
                                   //因為會跑到下一個Node!
        }
        //此Action主要執行的行為
        agent.SetDestination(TargetLocation.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(agent.remainingDistance <= agent.stoppingDistance) return Status.Success;
        return Status.Running;
    }

    //每次當我們成功或失敗，後要離開這個節點時
    protected override void OnEnd()
    {
    }
}

