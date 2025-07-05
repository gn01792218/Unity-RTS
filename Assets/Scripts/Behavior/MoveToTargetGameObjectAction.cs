using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Target GameObject", story: "[Agent] moves to [TargetGameObject]", category: "Action/Navigation", id: "87b9c6012bef119698a06859bd97f217")]
public partial class MoveToTargetGameObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
    [SerializeReference] public BlackboardVariable<float> MoveThreshold = new(0.25f); //超過多遠要追蹤的值

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 lastPosition;

    protected override Status OnStart()
    {
        if (!Agent.Value.TryGetComponent(out agent) || TargetGameObject.Value == null)
        {
            Debug.LogError("Agent 或 TargetGameObject 未正確設置！");
            return Status.Failure;
        }
        Agent.Value.TryGetComponent(out animator);

        Vector3 targetPosition = GetTargetPosition();
        if (Vector3.Distance(agent.transform.position, targetPosition) <= agent.stoppingDistance)
        {
            return Status.Success;
        }
        //SetDestination會自動計算路徑，並開始移動
        //但注意，此時如果馬上呼叫remainingDistance，會發現還是0
        //因為agent還沒有開始移動，這是Unity的bug   
        //所以Update中要加上agent.pathPending來判斷是否還在計算路徑
        agent.SetDestination(targetPosition);
        lastPosition = targetPosition;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (animator != null) animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, agent.velocity.magnitude);
        //切記要加上agent.pathPending，因為SetDestination計算路徑需要時間，所以一開始的remainingDistance會是0
        //如果不加上這個判斷，會導致agent.remainingDistance是就直接被Status.Success返回了，導致無法移動到該目標!
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Debug.Log($"Agent 到達目標位置: {TargetGameObject.Value.name}");
            return Status.Success;
        }
        //超出MoveThreshold的時候要重新設置目標，才能追上去
        Vector3 targetPosition = GetTargetPosition();
        if (Vector3.Distance(targetPosition, lastPosition) >= MoveThreshold)
        {
            agent.SetDestination(targetPosition);
            lastPosition = agent.destination;
            // Debug.Log($"Agent 還沒到MoveThreshold: {TargetGameObject.Value.name}");
            return Status.Running;
        }
        
        // Debug.Log($"Agent 移動中: {TargetGameObject.Value.name}");
        return Status.Running;
    }

    //優化目標點選擇
    //如果目標物件有Collider，則使用ClosestPoint來獲取最近的目標位置
    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition;
        if (TargetGameObject.Value.TryGetComponent(out Collider collider))
        {
            targetPosition = collider.ClosestPoint(agent.transform.position);
        }
        else
        {
            targetPosition = TargetGameObject.Value.transform.position;
        }
        return targetPosition;
    }
}

