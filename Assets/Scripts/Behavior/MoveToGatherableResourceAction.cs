using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to GatherableResource", story: "[Agent] moves to [Resource] or nearby not busy resource", category: "Action/Navigation", id: "99022e6ca04c1079ec95a55a77d1c2d4")]
public partial class MoveToGatherableResourceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GatherableResource> Resource;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);

    private NavMeshAgent agent;
    private LayerMask gatherableResourceLayerMask;
    private GatherableResurceSO resourceSO; 

    protected override Status OnStart()
    {
        gatherableResourceLayerMask = LayerMask.GetMask("GatherableResource");
        
        if (!HasValidInputs())
        {
            return Status.Failure;
        }
        agent.SetDestination(GetTargetPosition());
        return Status.Running;
    }

    private bool HasValidInputs()
    {
        if (!Agent.Value.TryGetComponent(out agent) || (Resource.Value == null && resourceSO == null))
        {
            return false;
        }

        if (Resource.Value != null) //如果該資源可以用
        {
            resourceSO = Resource.Value.resourceSO;
        }
        else //如果沒有指定資源，則尋找附近的資源
        {
            Collider[] colliders = FindNearbyNotBusyColliders();
            if (colliders.Length > 0)
            {
                Resource.Value = GetClosestResourceCollider(colliders);
                resourceSO = Resource.Value.resourceSO;
            }
            else
            {
                Debug.LogWarning("沒有找到附近的資源，無法移動到資源位置。");
                return false;
            }
        }

        return true;
    }

    protected override Status OnUpdate()
    {
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
        return Status.Failure;
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
        //找出附近其他沒有在忙碌的資源
        Collider[] colliders = Physics.OverlapSphere(
            agent.transform.position,
            SearchRadius,
            gatherableResourceLayerMask
            ).Where(collider => collider.TryGetComponent(out GatherableResource gatherableResource)
                    && !gatherableResource.IsBusy
                    && gatherableResource.resourceSO.Equals(resourceSO) //資源有不同種的，故要確保是和自己同種的SO
             ).ToArray();
        return colliders;
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

