using UnityEngine;

[CreateAssetMenu(fileName = "LoadUnitCommand", menuName = "Commands/Actions/LoadUnitCommand", order = 106)] //order要高於Move Command唷!
public class LoadUnitCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is ITransporter
            && context.Hit.collider != null
            && context.Hit.collider.TryGetComponent(out ITransportable _);
    }

    public override void Handle(CommandContext context)
    {
        ITransporter transporter = context.Unit as ITransporter;
        ITransportable transportable = context.Hit.collider.GetComponent<ITransportable>();

        transporter.LoadUnit(transportable);
    }

    public override bool IsAvailable(CommandContext? context = null)
    {
        return context?.Unit is ITransporter transporter
            && transporter.UsedCapacity < transporter.TransportConfigSO.Capacity;
    }
}