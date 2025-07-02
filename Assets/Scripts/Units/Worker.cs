using System;
using Unity.Behavior;
using UnityEngine;

public class Worker : Unit, IBuildingBuilder, IAttacker
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
    //註冊建築行為的事件監聽
    if (behaviorAgent.GetVariable("BuildingEvent", out BlackboardVariable<BuildingEventChannel> buildingEvent))
    {
      buildingEvent.Value.Event += OnBuildingEvent;
    }
  }

  public override void OnDeselect()
  {
    onSelectDecal.gameObject.SetActive(false); // Disable the decal projector when deselected
    IsSelected = false;
    if(!IsBuilding) OverridesAvailableCommands(null); //傳入null會恢復到該單位的初始化指令列表
    //發送取消選取的事件
    // ps.監聽事件者要負責將該單位從選取列表中移除
    Bus<UnselectedEvent>.Publish(new UnselectedEvent(this));
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

  //為了處理不同Worker之間的行為，需要監聽由行為樹中發射的建築的事件
  private void OnBuildingEvent(GameObject Self, BuildingEventEnum buildingEvent, BuildingUnit building)
  {
    switch (buildingEvent)
    {
      case BuildingEventEnum.ArrivedAt:
        //若有人正在蓋
        if (building != null && building.Progress.State == BuildingProgress.BuildingState.Building)
        {
          Stop();
          break;
        }
        //若沒有人在蓋
        OverridesAvailableCommands(new Command[] { CancelBuildingCommand });
        break;
      case BuildingEventEnum.Begin:
        OverridesAvailableCommands(new Command[] { CancelBuildingCommand });
        break;
      case BuildingEventEnum.Cancel:
      case BuildingEventEnum.Abort:
      case BuildingEventEnum.Completed:
        OverridesAvailableCommands(null); //恢復原廠指令設定
        break;
      default:
        break;
    }
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
  }

    public void Attack(IDamageable damageable)
    {
    Debug.Log(damageable);
        //設置behavior 的 variables
        behaviorAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Attack);
    }
}
