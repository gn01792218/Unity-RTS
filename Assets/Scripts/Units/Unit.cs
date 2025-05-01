using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit: MonoBehaviour , ISelectable, IMoveable{
    [SerializeField] private DecalProjector onSelectDecal; // 被選中時的標籤貼紙
    public float AgentRadius => agent.radius; //獲取NavMeshAgent的半徑
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    void Awake()
    {
        // Initialization code here
        agent = GetComponent<NavMeshAgent>();
        onSelectDecal.gameObject.SetActive(false); // Disable the decal projector initially
    }
    void Start(){
        Bus<SpawnUnitEvent>.Publish(new SpawnUnitEvent(this)); //發送自己已經出生的消息
    }

     // Worker class that implements ISelectable interface for unit selection and movement
    public void OnSelect()
    {
        onSelectDecal.gameObject.SetActive(true); // Enable the decal projector
        //發送被選到的事件,
        // ps.監聽事件者要負責將該單位添加到選取列表中
        Bus<SelectedEvent>.Publish(new SelectedEvent(this));
    }

    public void OnDeselect()
    {
        onSelectDecal.gameObject.SetActive(false); // Disable the decal projector when deselected
        //發送取消選取的事件
        // ps.監聽事件者要負責將該單位從選取列表中移除
        Bus<UnselectedEvent>.Publish(new UnselectedEvent(this));
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