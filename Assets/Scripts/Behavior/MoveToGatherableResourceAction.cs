using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to GatherableResource", story: "[Agent] moves to [Resource] or nearby not busy resource, and set [HasNearByResource]", category: "Action/Navigation", id: "99022e6ca04c1079ec95a55a77d1c2d4")]
public partial class MoveToGatherableResourceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GatherableResource> Resource;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);
    [SerializeReference] public BlackboardVariable<bool> HasNearByResource;

    private NavMeshAgent agent;
    private Animator animator;
    private LayerMask gatherableResourceLayerMask;
    private GatherableResurceSO resourceSO;

    protected override Status OnStart()
    {
        gatherableResourceLayerMask = LayerMask.GetMask("GatherableResource");

        if (!HasValidInputs())
        {
            return Status.Failure;
        }
        //若有附近資源，去採；若無移動到CommandPost
        Collider[] colliders = FindNearbyNotBusyColliders();
        if (colliders.Length > 0)
        {
            Resource.Value = GetClosestResourceCollider(colliders);
            resourceSO = Resource.Value.resourceSO;
            HasNearByResource.Value = true;
        }
        else
        {
            Debug.Log("沒有找到附近的資源，無法移動到資源位置。");
            HasNearByResource.Value = false;
        }
        Agent.Value.TryGetComponent(out agent);
        agent.SetDestination(GetTargetPosition());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (animator != null) animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, agent.velocity.magnitude);
        if (agent.remainingDistance >= agent.stoppingDistance)
        {
            return Status.Running;
        }
        if (Resource.Value != null && !Resource.Value.IsBusy && Resource.Value.Amount > 0)
        {
            //如果資源不忙碌且數量大於0，則回傳成功
            return Status.Success;
        }
        Collider[] colliders = FindNearbyNotBusyColliders();
        if (colliders.Length > 0)
        {
            Resource.Value = GetClosestResourceCollider(colliders);
            agent.SetDestination(GetTargetPosition());
            return Status.Running;
        }
        else
        {
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {
        if (animator != null) animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, 0);
    }

    private bool HasValidInputs()
    {
        if (!Agent.Value.TryGetComponent(out agent))
        {
            return false;
        }

        if (Resource.Value != null) //如果該資源可以用
        {
            resourceSO = Resource.Value.resourceSO;
        }
        return true;
    }

    private GatherableResource GetClosestResourceCollider(Collider[] colliders)
    {
        Collider closest = colliders[0];
        float minSqrDist = (closest.transform.position - agent.transform.position).sqrMagnitude;
        for (int i = 1; i < colliders.Length; i++)
        {
            float sqrDist = (colliders[i].transform.position - agent.transform.position).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = colliders[i];
            }
        }
        return closest.GetComponent<GatherableResource>();
    }

    private Collider[] FindNearbyNotBusyColliders()
    {
        // 找出附近其他沒有在忙碌的資源，若 resourceSO 不為 null 則比對資源種類
        return Physics.OverlapSphere(
                agent.transform.position,
                SearchRadius,
                gatherableResourceLayerMask
            )
            .Where(collider =>
                collider.TryGetComponent(out GatherableResource gatherableResource)
                && !gatherableResource.IsBusy
                && (resourceSO == null || gatherableResource.resourceSO.Equals(resourceSO))
            )
            .ToArray();
    }

    //優化目標點選擇
    //如果目標物件有Collider，則使用ClosestPoint來獲取最近的目標位置
    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition;
        if (Resource.Value.TryGetComponent(out Collider collider))
        {
            targetPosition = collider.ClosestPoint(agent.transform.position);
        }
        else
        {
            targetPosition = Resource.Value.transform.position;
        }
        return targetPosition;
    }
}

