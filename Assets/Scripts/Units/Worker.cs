using System;
using Unity.Behavior;
using UnityEngine;

public class Worker : Unit, IBuildingBuilder
{
  public bool IsBuilding => behaviorAgent.GetVariable("Commands", out BlackboardVariable<UnitCommandsEnum> command) && command.Value == UnitCommandsEnum.BuildBuilding;
  //computed properties
  public bool HasResources
  {
    get
    {
      if (behaviorAgent.GetVariable("ResourceAmount", out BlackboardVariable<int> resourceAmount))
      {
        return resourceAmount.Value > 0;
      }
      return false;
    }
  }
  [SerializeField] private Command CancelBuildingCommand; //取消建築的指令，不會出現在第一層指令列表中，是按下建築指令之後才產生的
  protected override void Start()
  {
    base.Start();
    //註冊Behavior事件監聽
    if (behaviorAgent.GetVariable("GatherResourceEvent", out BlackboardVariable<GatherResourceEventChannel> eventChannel))
    {
      eventChannel.Value.Event += OnGatherResourceEvent;
    }
  }
  public void Gather(GatherableResource resource)
  {
    behaviorAgent.SetVariableValue("TargetResource", resource);
    behaviorAgent.SetVariableValue("TargetGameObject", resource.gameObject);
    behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Gather);
  }
  public void ReturnResources(GameObject commandPost)
  {
    behaviorAgent.SetVariableValue("TargetCommandPostBuilding", commandPost); //設定目標建築物
    behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.ReturnResources); //發起返回資源命令
  }
  public GameObject BuildBuilding(BuildingUnitSO buildingSO, Vector3 targetLocation)
  {
    var ghostInstance = Instantiate(buildingSO.Prefab, targetLocation, Quaternion.identity);
    if (!ghostInstance.TryGetComponent(out BuildingUnit _))
    {
      Debug.LogError($"Missing BaseBuilding on Prefab for BuildingSo ${buildingSO.name}! Cannot build");
      return null;
    }
    //set up blackboard to build
    behaviorAgent.SetVariableValue("BuildBuildingSO", buildingSO);
    behaviorAgent.SetVariableValue("TargetLocation", targetLocation);
    behaviorAgent.SetVariableValue("BuildingGhost", ghostInstance);
    behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.BuildBuilding);
    //更新可用的指令，讓Worker多一個取消建築的指令
    OverridesAvailableCommands(new Command[] { CancelBuildingCommand });
    //交錢
    Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(-buildingSO.ResourceCostSO.GasCost, PlayerResources.GasSO));
    Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(-buildingSO.ResourceCostSO.MineralCost, PlayerResources.MineralSO));
    return ghostInstance;
  }

  private void OnGatherResourceEvent(GameObject Self, int Amount, GatherableResurceSO Resources)
  {
    // 發送GatherResourceEvent
    Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(Amount, Resources));
  }

  public void CancelBuilding()
  {
    //釋放資源
    if (behaviorAgent.GetVariable("BuildingGhost", out BlackboardVariable<GameObject> buildingGhost) && buildingGhost.Value != null)
    {
      //如果有建築物的預覽，則銷毀它
      Destroy(buildingGhost.Value);
    }
    if (behaviorAgent.GetVariable("BuildBuildingUnderConstruction", out BlackboardVariable<BuildingUnit> buildingUnderConstruction) && buildingUnderConstruction.Value != null)
    {
      //打75折退款
      var buildingSO = buildingUnderConstruction.Value.unitSO;
      Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(Mathf.FloorToInt(buildingSO.ResourceCostSO.GasCost * 0.75f), PlayerResources.GasSO));
      Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(Mathf.FloorToInt(buildingSO.ResourceCostSO.MineralCost * 0.75f), PlayerResources.MineralSO));
      Destroy(buildingUnderConstruction.Value.gameObject);
    }
    //恢復可用指令
    OverridesAvailableCommands(null);
    //停止移動
    Stop();
  }

  public void ResumeBuilding(BuildingUnit building)
  {
    behaviorAgent.SetVariableValue("TargetLocation", building.transform.position);
    behaviorAgent.SetVariableValue("BuildBuildingUnderConstruction", building);
    behaviorAgent.SetVariableValue("BuildBuildingSO", building.unitSO);
    behaviorAgent.SetVariableValue<GameObject>("BuildingGhost", null);
    behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.BuildBuilding);

    OverridesAvailableCommands(new Command[] { CancelBuildingCommand });
  }
}
