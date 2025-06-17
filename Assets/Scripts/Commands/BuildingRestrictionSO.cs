using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Building Restriction", menuName = "SO/Building RestrictionSO", order = 7)]
public class BuildingRestrictionSO : ScriptableObject
{
    public enum OverlapStyle
    {
        Sphere,
        Box
    }
    [field: SerializeField] public OverlapStyle HitDetectionStyle { get; private set; } = OverlapStyle.Sphere;
    [field: SerializeField] public float Radius { get; private set; } = 1f;
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
    [field: SerializeField] public bool MustBeFullyOnNavMesh { get; private set; } = true;
    [field: SerializeField] public int NavMeshAgentTypeId { get; private set; } //預設下0就是Humanoid
    [field: SerializeField] public float NavMeshTolerance { get; private set; } = 0.1f; // tolerance (公差、偏差值) for navmesh placement

    [field: SerializeField] public Vector3 Extens { get; private set; } = Vector3.one; // the size of the building for placement checks 
                                                                                       //例如該建築物長寬高都是3，則Extens應該設置為(1.5f, 1.5f, 1.5f)
    private readonly Collider[] hitColliders = new Collider[1];
    public bool CanPlace(Vector3 position)
    {
        //使用Alloc效能會比較好，因為不用每偵都重新分配記憶體
        int hits = HitDetectionStyle switch
        {
            OverlapStyle.Sphere => Physics.OverlapSphereNonAlloc(position, Radius, hitColliders, LayerMask),
            OverlapStyle.Box => Physics.OverlapBoxNonAlloc(position, Extens, hitColliders, Quaternion.identity, LayerMask),
            _ => throw new System.ArgumentException($"Unsupported OverlapStyle: {HitDetectionStyle}")
        };
        
        if (MustBeFullyOnNavMesh)
        {
            NavMeshQueryFilter queryFilter = new()
            {
                //在new的時候初始化物件的方法
                areaMask = NavMesh.AllAreas,
                agentTypeID = NavMeshAgentTypeId
            };

            bool isOnNavMesh = IsFullyOnNavMesh(position, queryFilter);
            return hits == 0 && isOnNavMesh;
        }

        return hits == 0;
    }

    private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter queryFilter)
    {
        Vector3[] corners = new[]
        {
            new Vector3(Extens.x, 0, Extens.z),
            new Vector3(Extens.x, 0, -Extens.z),
            new Vector3(-Extens.x, 0, -Extens.z),
            new Vector3(-Extens.x, 0, Extens.z)
        };

        return corners.All(corner => 
            NavMesh.SamplePosition(
                position + corner,
                out NavMeshHit _,
                NavMeshTolerance,
                queryFilter));
    }
}