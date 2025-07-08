using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class MilitaryUnit : Unit, IAttacker, ITransportable
{
    [SerializeField] private DamageableSensor DamageableSensor;
    [field: SerializeField] public ParticleSystem AttackParticleSystem { get; private set; }
    [field: SerializeField] public int TransportCapacityUsage { get; private set; } = 1;
    public NavMeshAgent navMeshAgent { get; private set; }

    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 初始化行為樹的黑板
        behaviorAgent.SetVariableValue("AttackConfig", unitSO.AttackConfigSO);
    }
    protected override void Start()
    {
        base.Start(); //呼叫父類的Start方法

        //初始化Sensor監聽
        if (DamageableSensor != null)
        {
            DamageableSensor.OnUnitEnter += HandleDamageableEnter;
            DamageableSensor.OnUnitExit += HandleDamageableExit;
            DamageableSensor.SetupAttackConfig(unitSO.AttackConfigSO);
        }
    }
    public void Attack(IDamageable damageable)
    {
        behaviorAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Attack);
    }
    public void MovingAttack(Vector3 location)
    {
        behaviorAgent.SetVariableValue<GameObject>("TargetGameObject", null);
        behaviorAgent.SetVariableValue("TargetLocation", location);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Attack);
    }
    private void HandleDamageableEnter(IDamageable incomingDamageable)
    {
        //DamageableSensor是一個Trigger的物件，會自己偵測到單位，並將之+入或移除
        //因此這裡只是將DamageableSensor的列表，複製到BehaviorTree的Blackboard而已
        List<GameObject> nearbyDamageables = GetSortedNearbyDamageableUnits();
        behaviorAgent.SetVariableValue("NearbyDamageableUnits", nearbyDamageables);

        //假設沒有設置TargetGameObject，且附近有敵人
        //就將附近最近的敵人設置為攻擊目標
        if (behaviorAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> targetGameObject)
            && targetGameObject.Value == null
            && nearbyDamageables.Count > 0)
        {
            behaviorAgent.SetVariableValue("TargetGameObject", nearbyDamageables[0]);
        }
    }
    private void HandleDamageableExit(IDamageable leavingDamageable)
    {
        List<GameObject> nearbyDamageables = GetSortedNearbyDamageableUnits();
        behaviorAgent.SetVariableValue("NearbyDamageableUnits", nearbyDamageables);

        //假如原本沒有攻擊的目標，或離開的目標並非攻擊的目標，啥也不做
        if (!behaviorAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> targetGameObject)
            || leavingDamageable.Transform.gameObject != targetGameObject.Value
        ) return;

        //假如偵測到附近有敵人，選一個最近的當攻擊目標
        if (nearbyDamageables.Count > 0) behaviorAgent.SetVariableValue("TargetGameObject", nearbyDamageables[0]);
        else
        {
            behaviorAgent.SetVariableValue<GameObject>("TargetGameObject", null); //取消該目標
            // behaviorAgent.SetVariableValue("TargetLocation", leavingDamageable.Transform.position); //會追上該目標，這很怪!!!!!幹嘛這樣搞?!
        }
    }
    private List<GameObject> GetSortedNearbyDamageableUnits() //由於目前blackboard中的List除基本類型、Gameobject以外，無法使用自訂義類型，因此暫時使用Gameobject
    {
        return DamageableSensor.Damageables
            .OrderBy(d => Vector3.Distance(transform.position, d.Transform.position))
            .Select(d => d.Transform.gameObject)
            .ToList();
    }

    public void LoadInto(ITransporter transporter)
    {
        //跑到Transporter處
        MoveToGameObject(transporter.Transform);
        //叫Transporter裝他
        transporter.LoadUnit(this);
    }
}