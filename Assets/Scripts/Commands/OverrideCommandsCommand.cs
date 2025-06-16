using UnityEngine;
[CreateAssetMenu(fileName = "Override Commands", menuName = "Commands/Actions/OverrideCommandsCommand", order = 110)]
public class OverrideCommandsCommand : Command
{
    [field: SerializeField] public Command[] Commands { get; private set; }
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit != null;
    }

    public override void Handle(CommandContext context)
    {
        context.Unit.OverridesAvailableCommands(Commands);
    }
}