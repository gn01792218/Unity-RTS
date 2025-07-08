using UnityEngine;

public interface IMoveable
{
    void MoveToLocation(Vector3 direction);
    void MoveToGameObject(Transform transform);
    void Stop();
}