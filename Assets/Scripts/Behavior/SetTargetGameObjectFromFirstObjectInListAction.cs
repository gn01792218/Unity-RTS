using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set TargetGameObject from First Object in List", story: "Set [TargetGameObject] to the first item in [List]", category: "Action/Blackboard", id: "97376e092d054efd8e9f12e1d5078f15")]
public partial class SetTargetGameObjectFromFirstObjectInListAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;

    protected override Status OnStart()
    {
        if (List.Value == null || List.Value.Count == 0) return Status.Failure;
        TargetGameObject.Value = List.Value[0];
        return Status.Success;
    }
}

