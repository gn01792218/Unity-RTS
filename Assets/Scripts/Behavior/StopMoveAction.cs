using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopMoveAction", story: "[Agent] stops moving", category: "Action/Navigation", id: "fa8a9a507ae66497d247ff4a762d5c42")]
public partial class StopMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        if(Agent.Value.TryGetComponent(out NavMeshAgent agent))
        {
            agent.ResetPath(); //停止並清空移動路徑
            return Status.Success;
        }

        return Status.Failure;
    }
}

