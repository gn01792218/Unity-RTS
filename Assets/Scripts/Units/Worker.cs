using Unity.Behavior;

public class Worker :Unit 
{
   public void Gather(GatherableResource resource)
   {
     behaviorAgent.SetVariableValue("TargetResource", resource);
     behaviorAgent.SetVariableValue("TargetGameObject", resource.gameObject);
     behaviorAgent.SetVariableValue("Commands",UnitCommandsEnum.Gather);
   } 
}
