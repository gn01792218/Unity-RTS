using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BuildBuilding", story: "[Self] build [BuildingSO] at [TargetLocation]", category: "Action/Units", id: "68ab6350d62def178aa67aab36a8a3a2")]
public partial class BuildBuildingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<BuildingUnitSO> BuildingSO;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    private float startBuildTime;
    private BuildingUnit building;
    private Vector3 startPosition;

    protected override Status OnStart()
    {
        if (!HasVaildInputs()) return Status.Failure;
        startBuildTime = Time.time;

        //初始化建築物
        building = GameObject.Instantiate(BuildingSO.Value.Prefab).GetComponent<BuildingUnit>();

        //依據建造時間從底下升起
        var buildingRender = building.MainRender;
        startPosition = TargetLocation.Value - Vector3.up * buildingRender.bounds.size.y;
        building.transform.position = startPosition;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //更新建築升起的位置
        //依據所需建造時間，將開始到完成，序列化0-1
        float normalizedTime = (Time.time - startBuildTime) / BuildingSO.Value.BuildTime;
        Debug.Log($"normalizedTime : {normalizedTime}; build Time: {BuildingSO.Value.BuildTime}");
        building.transform.position = Vector3.Lerp(startPosition, TargetLocation.Value, normalizedTime);
        return normalizedTime >=1 ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        if (CurrentStatus == Status.Success)
        {
            building.enabled = true; //啟動建築
        }
    }

    private bool HasVaildInputs()
    {
        return Self.Value != null
            && BuildingSO.Value != null
            && BuildingSO.Value.Prefab != null;
    }
}

