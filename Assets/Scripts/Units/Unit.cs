using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit:CommandableUnit, IMoveable
{
    public float AgentRadius => agent.radius; //獲取NavMeshAgent的半徑
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        // Initialization code here
        agent = GetComponent<NavMeshAgent>();
    }
    protected override void Start(){
        base.Start(); //呼叫父類的Start方法
        Bus<SpawnUnitEvent>.Publish(new SpawnUnitEvent(this)); //發送自己已經出生的消息
    }

    public void Move(Vector3 direction)
    {
        agent.SetDestination(direction); 
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }

}