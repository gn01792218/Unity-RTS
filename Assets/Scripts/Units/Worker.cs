using System;
using Unity.Behavior;
using UnityEngine;

public class Worker : Unit
{
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
  private void OnGatherResourceEvent(GameObject Self, int Amount, GatherableResurceSO Resources)
  {
    // 發送GatherResourceEvent
    Bus<GatherResourceEvent>.Publish(new GatherResourceEvent(Amount, Resources));
  }
}
