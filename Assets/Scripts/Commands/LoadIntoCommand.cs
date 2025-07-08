using UnityEngine;

[CreateAssetMenu(fileName = "LoadIntoCommand", menuName = "Commands/Actions/LoadIntoCommand", order = 107)]
// 注意!此指令的放在Unit的AvilableCommands中的優先度很重要哦!
public class LoadIntoCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is ITransportable
            && context.Hit.collider != null
            && context.Hit.collider.TryGetComponent(out ITransporter _);
    }

    public override void Handle(CommandContext context)
    {
        ITransportable transportable = context.Unit as ITransportable;
        ITransporter transporter = context.Hit.collider.GetComponent<ITransporter>();

        transportable.LoadInto(transporter);
    }

    public override bool IsAvailable(CommandContext? context = null)
    {
        return true;
    }
}