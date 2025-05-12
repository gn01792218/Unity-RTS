using Unity.Behavior;

public class Worker :Unit 
{
   public void Gather(GatherableResource resource)
   {
     behaviorAgent.SetVariableValue("TargetResource", resource);
     behaviorAgent.SetVariableValue("TargetLocation", resource.transform.position);
     behaviorAgent.SetVariableValue("Commands",UnitCommandsEnum.Gather);
   } 
}
