using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

//標示需要NavMeshAgent組件
//添加了之後，我們就不需要做一大堆的null檢查了
[RequireComponent(typeof(NavMeshAgent))]
public class Worker : MonoBehaviour , ISelectable, IMoveable
{
    [SerializeField] private DecalProjector onSelectDecal; // 被選中時的標籤貼紙
    private NavMeshAgent agent; //獲取NavMeshAgent組件
    void Awake()
    {
        // Initialization code here
        agent = GetComponent<NavMeshAgent>();
        onSelectDecal.gameObject.SetActive(false); // Disable the decal projector initially
    }

     // Worker class that implements ISelectable interface for unit selection and movement
    public void OnSelect()
    {
        onSelectDecal.gameObject.SetActive(true); // Enable the decal projector
    }

    public void OnDeselect()
    {
        onSelectDecal.gameObject.SetActive(false); // Disable the decal projector when deselected
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
