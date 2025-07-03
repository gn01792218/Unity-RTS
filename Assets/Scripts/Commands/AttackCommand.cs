using UnityEngine;
[CreateAssetMenu(fileName = "AttackCommand", menuName = "Commands/Actions/AttackCommand", order = 99)] //order要高於Move Command唷!
public class AttackCommand : Command
{
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is IAttacker
            && context.Hit.collider != null;
    }

    public override void Handle(CommandContext context)
    {
        IAttacker unit = context.Unit as IAttacker; //將單位轉換為Unit類別，為了獲取AgentRadius
        if (context.Hit.collider.TryGetComponent(out IDamageable damageable))
        {
            unit.Attack(damageable);
        }
        else
        {
            unit.MovingAttack(context.Hit.point);
        }
    }
    public override bool IsAvailable(CommandContext? context = null)
    {
        return true;
    }
}