using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Building Is In Progress", story: "[BuildingUnit] is being build", category: "Conditions", id: "7123b483cb3e838a1ec2bc9f3f9db9fa")]
public partial class BuildingIsInProgressCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BuildingUnit> BuildingUnit;

    public override bool IsTrue()
    {
        return BuildingUnit.Value != null
            && BuildingUnit.Value.Progress.State == BuildingProgress.BuildingState.Building;
    }
}
