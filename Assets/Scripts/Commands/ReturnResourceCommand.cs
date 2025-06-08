using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Return Resource Command", menuName = "Commands/Actions/Return Resource Command", order = 105)]
public class ReturnResourceCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is Worker
        && context.Hit.collider != null
        && context.Hit.collider.TryGetComponent(out CommandPost _); //是command post
    }

    public override void Handle(CommandContext context)
    {
        Worker worker = context.Unit as Worker;

        if (worker.HasResources)
        {
            worker.ReturnResources(context.Hit.collider.gameObject);
        }
        else
        {
            //應該要去找最近的資源進行採集!
        }
    }
}