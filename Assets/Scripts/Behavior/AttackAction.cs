using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackAction", story: "[Self] attacks [TargetGameObject] according to [AttackConfigSO] until it dies.", category: "Action", id: "56d4d7fae08cc3baa3e4d13f8d86b20b")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
    [SerializeReference] public BlackboardVariable<UnitAttackConfigSO> AttackConfigSO;
    [SerializeReference] public BlackboardVariable<List<GameObject>> NearbyDamageables;

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
        //設置移動的動畫
        if (animator != null)
        {
            animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, navMeshAgent.velocity.magnitude);
        }

        if (!NearbyDamageables.Value.Contains(TargetGameObject.Value)) return Status.Running;

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
            //假設該攻擊不屬於投射型攻擊(如手榴彈)，才馬上給傷害；否則得等到目標位置後，再給傷害(見該類的動畫設置，如Grenadier的AnimateGrenadeMovement)
            if (!selfUnit.unitSO.AttackConfigSO.HasProjectileAttacks)
            {
                targetUnit.TakeDamage(AttackConfigSO.Value.Damage);
            }
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
        //恢復成可以行走的狀態
        navMeshAgent.isStopped = false;
    }
    private bool HasValidInputs()
    {
        return Self.Value != null
            && Self.Value.TryGetComponent(out NavMeshAgent _)
            && Self.Value.TryGetComponent(out MilitaryUnit _)
            && TargetGameObject.Value != null
            && TargetGameObject.Value.TryGetComponent(out IDamageable _)
            && AttackConfigSO != null
            && NearbyDamageables != null;
    }
    private void Init()
    {
        selfTransform = Self.Value.transform;
        navMeshAgent = Self.Value.GetComponent<NavMeshAgent>();
        animator = selfTransform.GetComponent<Animator>();
        selfUnit = Self.Value.GetComponent<MilitaryUnit>();

        targetTransform = TargetGameObject.Value.transform;
        targetUnit = TargetGameObject.Value.GetComponent<IDamageable>();

        //假如原本的目標不包含在附近目標中，追上去
        if (!NearbyDamageables.Value.Contains(TargetGameObject.Value))
        {
            //去追目標
            navMeshAgent.SetDestination(targetTransform.position);
            navMeshAgent.isStopped = false;
            //結束開槍動畫
            if (animator != null)
            {
                animator.SetBool(AnimationConstants.ATTACK, false);
            }
        }
    }
}

