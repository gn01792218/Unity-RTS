using UnityEngine;

public interface IAttacker
{
    public void Attack(IDamageable damageable); //攻擊特定目標
    public void MovingAttack(Vector3 location); //移動到指定地點，並攻擊路徑上的所有目標
    public Transform Transform { get; }
}