using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Closest Command Post", story: "[Unit] finds nearest [TargetCommandPostBuilding]", category: "Action/Units", id: "4808415a36bdf3e58ef56bede06c86d6")]
public partial class FindClosestCommandPostAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Unit;
    [SerializeReference] public BlackboardVariable<GameObject> TargetCommandPostBuilding;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new (10);

    protected override Status OnStart()
    {
        Collider[] colliders = Physics.OverlapSphere(Unit.Value.transform.position, SearchRadius, LayerMask.GetMask("Building"));
        List<CommandPost> nearbyCommandPosts = new();

        foreach(Collider collider in colliders)
        {
            if(collider.TryGetComponent(out CommandPost building)
                && building.Progress.State == BuildingProgress.BuildingState.Completed ) 
            {
                nearbyCommandPosts.Add(building);
            }
        }

        if(nearbyCommandPosts.Count == 0)
        {
            return Status.Failure;
        }

        var closest = nearbyCommandPosts
            .OrderBy(post => Vector3.Distance(Unit.Value.transform.position, post.transform.position))
            .First();

        TargetCommandPostBuilding.Value = closest.gameObject;
        return Status.Success;
    }
}

