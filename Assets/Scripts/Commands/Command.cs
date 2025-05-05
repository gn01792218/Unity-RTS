using UnityEngine;

public abstract class Command : ScriptableObject, ICommand
{
    public abstract bool CanHandle(CommandContext context);
    public abstract void Handle(CommandContext context);
}