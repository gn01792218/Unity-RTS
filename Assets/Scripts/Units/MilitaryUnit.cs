using System.Diagnostics;
using UnityEngine;

public class MilitaryUnit : Unit, IAttacker
{
    [field: SerializeField] public ParticleSystem AttackParticleSystem { get; private set; }
    public void Attack(IDamageable damageable)
    {
        //設置behavior 的 variables
        behaviorAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Attack);
    }
}