using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PickRandomLocationWithinRendererBounds", story: "Set [TargetLocation] to a random point within [BuindingUnderConstructure]", category: "Action", id: "bc1d5617846d2cc5676226566adf64a2")]
public partial class PickRandomLocationWithinRendererBoundsAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    [SerializeReference] public BlackboardVariable<BuildingUnit> BuindingUnderConstructure;

    protected override Status OnStart()
    {
        if (BuindingUnderConstructure.Value == null || BuindingUnderConstructure.Value.MainRender == null)
        {
            Debug.LogError("BuildingUnderConstructure or its MainRender is not set.");
            return Status.Failure;
        }

        Renderer renderer = BuindingUnderConstructure.Value.MainRender;
        Bounds bounds = renderer.bounds;

        TargetLocation.Value = new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            TargetLocation.Value.y,
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );

        return Status.Success;
    }
}

