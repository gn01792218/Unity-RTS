using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAgentAvoidance", story: "Set [Agent] avoidance quality to [AvoidanceQuality]", category: "Action/Navigation", id: "ed253bbea4e926eb1ad6af5a72625bcb")]
public partial class SetAgentAvoidanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> AvoidanceQuality;

    protected override Status OnStart()
    {
        if(!Agent.Value.TryGetComponent(out NavMeshAgent agent) || AvoidanceQuality > 4 || AvoidanceQuality<0) //不能超過MeshAgent的Avoidance Enum值
        {
            Debug.LogError($"Agent {Agent.Value.name} does not have a NavMeshAgent component.");
            return Status.Failure;
        }   

        if (agent.TryGetComponent(out Animator animator)) animator.SetFloat(AnimationConstants.SPEED_ID, 0); //停止動畫
        agent.obstacleAvoidanceType = (ObstacleAvoidanceType)AvoidanceQuality.Value;
        return Status.Success;
    }
}

