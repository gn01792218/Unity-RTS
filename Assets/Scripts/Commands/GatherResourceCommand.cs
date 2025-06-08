using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gather Resource Command", menuName = "Commands/Actions/Gather Resource Command", order = 105)]
public class GatherResourceCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is Worker
        && context.Hit.collider != null
        && context.Hit.collider.TryGetComponent(out GatherableResource _); //只須確保該物件是GatherableResource，但不需要使用它，所以用_代替
    }

    public override void Handle(CommandContext context)
    {
        Worker worker = context.Unit as Worker;
        worker.Gather(context.Hit.collider.GetComponent<GatherableResource>()); //叫工人去採集 
    }
}