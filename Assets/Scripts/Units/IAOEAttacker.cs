using UnityEngine;

public interface IAOEAttacker
{
    public void GiveAoeDamage(Vector3 AoeCenterPoint); //攻擊特定目標
    public int CalculateAOEDamage(Vector3 centerPoint, Vector3 targetPosition);
    public UnitAOEAttackConfigSO AOEAttackConfigSO { get; }
}