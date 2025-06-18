using UnityEngine;
[CreateAssetMenu(fileName = "Cancel Building Command", menuName = "Commands/Actions/Cancel Building Command")]
public class CancelBuildingCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is IBuildingBuilder;
    }

    public override bool IsAvailable(CommandContext? context = null)
    {
        return true; 
    }

    public override void Handle(CommandContext context)
    {
        IBuildingBuilder buildingBuilder = (IBuildingBuilder)context.Unit;
        buildingBuilder.CancelBuilding();
    }
}