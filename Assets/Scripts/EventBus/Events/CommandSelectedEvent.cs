public struct CommandSelectedEvent:IEvent
{
    public Command SelectdCommand {get; private set;}
    public CommandSelectedEvent(Command command)
    {
        SelectdCommand = command;
    }
}