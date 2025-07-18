using UnityEngine;

[CreateAssetMenu(fileName = "UnLoadUnitCommand", menuName = "Commands/Actions/UnLoadUnitCommand", order = 107)] //order要高於Move Command唷!
public class UnLoadUnitCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is ITransporter transporter
            && transporter.UsedCapacity > 0;
    }

    public override void Handle(CommandContext context)
    {
        ITransporter transporter = context.Unit as ITransporter;

        transporter.UnloadAllUnits();
    }

    public override bool IsAvailable(CommandContext? context = null)
    {
        return context?.Unit is ITransporter transporter && transporter.UsedCapacity > 0;
    }
}