using UnityEngine;

public interface IAttacker
{
    public void Attack(IDamageable damageable);
    public Transform Transform { get; }
}