using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
public abstract class Unit : CommandableUnit, IMoveable
{
    public float AgentRadius => agent.radius; //獲取NavMeshAgent的半徑
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    protected BehaviorGraphAgent behaviorAgent;
    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        agent = GetComponent<NavMeshAgent>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        // 初始化行為樹的黑板
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Stop);
    }
    protected override void Start()
    {
        base.Start(); //呼叫父類的Start方法
        Bus<SpawnUnitEvent>.Publish(new SpawnUnitEvent(this)); //發送自己已經出生的消息
    }

    public void MoveToLocation(Vector3 direction)
    {
        OverridesAvailableCommands(null);
        //不直接操作agent來移動
        // agent.SetDestination(direction); 
        //改透過BehaviorAgent
        behaviorAgent.SetVariableValue<GameObject>("TargetGameObject", null); //讓行為節點樹可以判斷使用的是移動到Location
        behaviorAgent.SetVariableValue("TargetLocation", direction); //"TargetLocation"對應該Behavior中的Blackboard中的變數
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Move);
    }
    public void MoveToGameObject(Transform transform)
    {
        OverridesAvailableCommands(null);
        //改透過BehaviorAgent
        behaviorAgent.SetVariableValue("TargetGameObject", transform.gameObject);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Move);
    }

    public void Stop()
    {
        OverridesAvailableCommands(null);
        behaviorAgent.SetVariableValue("Commands", UnitCommandsEnum.Stop);//"Commands"對應行為黑板中的變數名稱
    }
    protected virtual void OnDestroy()
    {
        //當單位死亡時，發送死亡事件
        Bus<UnitDeathEvent>.Publish(new UnitDeathEvent(this));
    }
}