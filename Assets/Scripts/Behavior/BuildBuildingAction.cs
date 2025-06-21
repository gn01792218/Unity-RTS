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
    [SerializeReference] public BlackboardVariable<BuildingUnit> BuildingUnderConstruction;

    private float startBuildTime;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Renderer buildingRenderer; // 為了建築中動畫，移動的MeshRender。 //為何不移動GameObject? 因為我們要讓建築物在升起的初始，也具有Obstacle計算，因此只移動Mesh才不會導致Obstacle計算在地底時不生效
    private float targetHealth;

    protected override Status OnStart()
    {
        if (!HasVaildInputs()) return Status.Failure;
        startBuildTime = Time.time;

        //初始化建築物
        BuildingUnderConstruction.Value = GameObject.Instantiate(BuildingSO.Value.Prefab, TargetLocation.Value, Quaternion.identity).GetComponent<BuildingUnit>();
        //開始建築物的Progress
        BuildingUnderConstruction.Value.StartBuilding(Self.Value.GetComponent<IBuildingBuilder>());
        //依據建造時間從底下升起
        buildingRenderer = BuildingUnderConstruction.Value.MainRender;
        startPosition = TargetLocation.Value - Vector3.up * buildingRenderer.bounds.size.y;
        endPosition = TargetLocation.Value;
        buildingRenderer.transform.position = startPosition;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //更新血量
        targetHealth += Time.deltaTime * (BuildingSO.Value.Health / BuildingSO.Value.BuildTime);
        HealthUpdate();
        //更新建築升起的位置
        //依據所需建造時間，將開始到完成，序列化0-1
        float normalizedTime = (Time.time - startBuildTime) / BuildingSO.Value.BuildTime;
        buildingRenderer.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
        return normalizedTime >= 1 ? Status.Success : Status.Running;
    }

    private void HealthUpdate()
    {
        if (targetHealth >= 1)
        {
            int healAmount = Mathf.FloorToInt(targetHealth);
            BuildingUnderConstruction.Value.Heal(healAmount);
            targetHealth -= healAmount;
        }
    }

    protected override void OnEnd()
    {
        if (CurrentStatus == Status.Success)
        {
            BuildingUnderConstruction.Value.enabled = true; //啟動建築
        }
    }

    private bool HasVaildInputs()
    {
        return Self.Value != null
            && BuildingSO.Value != null
            && BuildingSO.Value.Prefab != null;
    }
}

