using System.Collections.Generic;
using UnityEngine;
public struct ClosestColliderComparer : IComparer<Collider>
{
    private readonly Vector3 targetPosition;

    public ClosestColliderComparer(Vector3 position)
    {
        targetPosition = position;
    }


    // return <0 表示x是first、0表示相等、>0表示y是first
    public int Compare(Collider x, Collider y)
    {
        float distanceX = (targetPosition- x.transform.position).sqrMagnitude;
        float distanceY = (targetPosition- y.transform.position).sqrMagnitude;
        return distanceX.CompareTo(distanceY);
    }
}