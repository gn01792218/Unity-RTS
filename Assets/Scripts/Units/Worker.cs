using Unity.Behavior;
using UnityEngine;

public class Worker : Unit, IBuildingBuilder
{
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
    if (ghostInstance.TryGetComponent(out BuildingUnit baseBuilding))
    {
      baseBuilding.ShowGhostVisuals();
    }
    else
    {
      Debug.LogError($"Missing BaseBuilding on Prefab for BuildingSo ${buildingSO.name}! Cannot build");
      return null;
    }
    //set up blackboard to build
    behaviorAgent.SetVariableValue("BuildBuildingSO", buildingSO);
    behaviorAgent.SetVariableValue("TargetLocation", targetLocation);
    behaviorAgent.SetVariableValue("BuildingGhost", ghostInstance);
    behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.BuildBuilding);

    return ghostInstance;
  }

  private void OnGatherResourceEvent(GameObject Self, int Amount, GatherableResurceSO Resources)
  {
    // 發送GatherResourceEvent
    Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(Amount, Resources));
  }
}
