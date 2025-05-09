using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
public abstract class Unit:CommandableUnit, IMoveable
{
    public float AgentRadius => agent.radius; //獲取NavMeshAgent的半徑
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    private BehaviorGraphAgent behaviorAgent;
    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        agent = GetComponent<NavMeshAgent>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
    }
    protected override void Start(){
        base.Start(); //呼叫父類的Start方法
        Bus<SpawnUnitEvent>.Publish(new SpawnUnitEvent(this)); //發送自己已經出生的消息
        Move(transform.position); //一開始移動到自己出生的位置就好，不然會跑到該物件prefab的初始位置唷!
    }

    public void Move(Vector3 direction)
    {
        //不直接操作agent來移動
        // agent.SetDestination(direction); 

        //改透過BehaviorAgent
        behaviorAgent.SetVariableValue("TargetLocation", direction); //"TargetLocation"對應該Behavior中的Blackboard中的變數
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }
}