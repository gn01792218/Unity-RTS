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
    private MilitaryUnit selfUnit; //為了獲取該攻擊者的類別訊息 : 如粒子系統

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
        //同步開槍動畫的速度和meshAgent一致?
        if (animator != null)
        {
            animator.SetFloat(AnimationConstants.SPEED_ID, navMeshAgent.velocity.magnitude);
        }

        if (Vector3.Distance(targetTransform.position, selfTransform.position) >= AttackConfigSO.Value.AttackRange)
        {
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = false;
            //結束開槍動畫
            if (animator != null)
            {
                animator.SetBool(AnimationConstants.ATTACK, false);
            }
            return Status.Running;
        }

        navMeshAgent.isStopped = true; //到了目標可攻擊距離位置就停下來
        SelfLookAtTarget();//朝向目標
        //播放開槍動畫
        if (animator != null)
        {
            animator.SetBool(AnimationConstants.ATTACK, true);
        }
        if (Time.time >= lastAttackTime + AttackConfigSO.Value.AttackDelay)
        {
            lastAttackTime = Time.time;
            //設置開槍的粒子系統
            if (selfUnit.AttackParticleSystem != null)
            {
                selfUnit.AttackParticleSystem.Play();
            }
            targetUnit.TakeDamage(AttackConfigSO.Value.Damage);
        }
        return Status.Running;
    }

    private void SelfLookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(
                    (targetTransform.position - selfTransform.position).normalized,
                    Vector3.up
                );
        selfTransform.rotation = Quaternion.Euler(
            selfTransform.rotation.eulerAngles.x,
            lookRotation.eulerAngles.y,
            selfTransform.rotation.eulerAngles.z
        );
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
            && Self.Value.TryGetComponent(out MilitaryUnit _)
            && TargetGameObject.Value != null
            && TargetGameObject.Value.TryGetComponent(out IDamageable _)
            && AttackConfigSO != null;
    }
    private void Init()
    {
        selfTransform = Self.Value.transform;
        navMeshAgent = Self.Value.GetComponent<NavMeshAgent>();
        animator = selfTransform.GetComponent<Animator>();
        selfUnit = Self.Value.GetComponent<MilitaryUnit>();

        targetTransform = TargetGameObject.Value.transform;
        targetUnit = TargetGameObject.Value.GetComponent<IDamageable>();
    }
}

