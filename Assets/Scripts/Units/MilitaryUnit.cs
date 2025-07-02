using System.Diagnostics;

public class MilitaryUnit : Unit, IAttacker
{
    public void Attack(IDamageable damageable)
    {
        //設置behavior 的 variables
        behaviorAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Attack);
    }
}