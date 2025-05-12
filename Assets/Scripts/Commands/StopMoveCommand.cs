using UnityEngine;
[CreateAssetMenu(fileName = "StopCommand", menuName = "Commands/Actions/StopCommand", order = 101)] //order要高於Move Command唷!
public class StopMoveCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is Unit; //檢查是否為Unit，因為這個類別包含IMoveable和AgentRadius
    }

    public override void Handle(CommandContext context)
    {
        Unit unit = context.Unit as Unit; //將單位轉換為Unit類別，為了獲取AgentRadius
        unit.Stop();
    }
}