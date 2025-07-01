using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
public abstract class Unit : CommandableUnit, IMoveable
{
    [SerializeField] private DamageableSensor DamageableSensor;
    public float AgentRadius => agent.radius; //獲取NavMeshAgent的半徑
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    protected BehaviorGraphAgent behaviorAgent;
    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        agent = GetComponent<NavMeshAgent>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        // 初始化行為樹的黑板
        behaviorAgent.SetVariableValue("Command", UnitCommandsEnum.Stop);
        behaviorAgent.SetVariableValue("AttackConfig", unitSO.AttackConfigSO);
    }
    protected override void Start()
    {
        base.Start(); //呼叫父類的Start方法
        Bus<SpawnUnitEvent>.Publish(new SpawnUnitEvent(this)); //發送自己已經出生的消息

        //初始化Sensor監聽
        if (DamageableSensor != null)
        {
            DamageableSensor.OnUnitEnter += HandleDamageableEnter;
            DamageableSensor.OnUnitExit += HandleDamageableExit;
            DamageableSensor.SetupAttackConfig(unitSO.AttackConfigSO);
        }
    }

    public void Move(Vector3 direction)
    {
        OverridesAvailableCommands(null);
        //不直接操作agent來移動
        // agent.SetDestination(direction); 
        //改透過BehaviorAgent
        behaviorAgent.SetVariableValue("TargetLocation", direction); //"TargetLocation"對應該Behavior中的Blackboard中的變數
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Move);
    }

    public void Stop()
    {
        OverridesAvailableCommands(null);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Stop);//"Commands"對應行為黑板中的變數名稱
    }
    private void OnDestroy()
    {
        //當單位死亡時，發送死亡事件
        Bus<UnitDeathEvent>.Publish(new UnitDeathEvent(this));
    }
    private void HandleDamageableEnter(IDamageable damageable)
    {
        //DamageableSensor是一個Trigger的物件，會自己偵測到單位，並將之+入或移除
        //因此這裡只是將DamageableSensor的列表，複製到BehaviorTree的Blackboard而已
        behaviorAgent.SetVariableValue("NearbyDamageableUnits", GetSortedNearbyDamageableUnits());
    }
    private void HandleDamageableExit(IDamageable damageable)
    {
        behaviorAgent.SetVariableValue("NearbyDamageableUnits", GetSortedNearbyDamageableUnits());
    }
    private List<GameObject> GetSortedNearbyDamageableUnits() //由於目前blackboard中的List除基本類型、Gameobject以外，無法使用自訂義類型，因此暫時使用Gameobject
    {
        return DamageableSensor.Damageables
            .OrderBy(d => Vector3.Distance(transform.position, d.Transform.position)) 
            .Select(d => d.Transform.gameObject)
            .ToList();
    }
}