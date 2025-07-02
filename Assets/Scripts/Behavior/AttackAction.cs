using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackAction", story: "[Self] attacks [TargetGameObject] according to [AttackConfigSO] until [Target] dies.", category: "Action", id: "56d4d7fae08cc3baa3e4d13f8d86b20b")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
    [SerializeReference] public BlackboardVariable<UnitAttackConfigSO> AttackConfigSO;

    private NavMeshAgent navMeshAgent;
    private Transform selfTransform;
    private Animator animator;

    private IDamageable targetUnit;
    private Transform targetTransform;

    private float lastAttackTime;


    protected override Status OnStart()
    {
        if (!HasValidInputs()) return Status.Failure;
        Init();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (TargetGameObject.Value == null || targetUnit.CurrentHealth == 0) return Status.Success; //目標死亡就結束

        if (Vector3.Distance(targetTransform.position, selfTransform.position) >= AttackConfigSO.Value.AttackRange)
        {
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = false;
            return Status.Running;
        }

        navMeshAgent.isStopped = true; //到了目標可攻擊距離位置就停下來

        if (Time.time >= lastAttackTime + AttackConfigSO.Value.AttackDelay)
        {
            lastAttackTime = Time.time;
            targetUnit.TakeDamage(AttackConfigSO.Value.Damage);
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (animator != null)
        {
            animator.SetBool(AnimationConstants.ATTACK, false);
        }
    }
    private bool HasValidInputs()
    {
        return Self.Value != null
            && Self.Value.TryGetComponent(out NavMeshAgent _)
            && Self.Value.TryGetComponent(out IAttacker _)
            && TargetGameObject.Value != null
            && TargetGameObject.Value.TryGetComponent(out IDamageable _)
            && AttackConfigSO != null;
    }
    private void Init()
    {
        selfTransform = Self.Value.transform;
        navMeshAgent = Self.Value.GetComponent<NavMeshAgent>();
        animator = selfTransform.GetComponent<Animator>();

        targetTransform = TargetGameObject.Value.transform;
        targetUnit = TargetGameObject.Value.GetComponent<IDamageable>();

        if (animator != null)
        {
            animator.SetBool(AnimationConstants.ATTACK, true);
        }
    }
}

