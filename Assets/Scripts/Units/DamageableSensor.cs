using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]  //需要Collider元件，來偵測trigger
public class DamageableSensor : MonoBehaviour
{

    public List<IDamageable> Damageables => damageables.ToList(); //供外部觀看的，防止元列表被汙染
    [field: SerializeField] public Owner Owner { get; set; }

    //與外溝通的事件
    public delegate void UnitDetectionEvent(IDamageable damageable);
    public event UnitDetectionEvent OnUnitEnter;
    public event UnitDetectionEvent OnUnitExit;

    private HashSet<IDamageable> damageables = new(); //內部操作的
    private new SphereCollider collider;
    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    //只要有任何具有Rigdbody元件者進入，就會觸發此
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable enterUnit) && enterUnit.Owner != Owner)
        {
            damageables.Add(enterUnit);
            OnUnitEnter?.Invoke(enterUnit); //喚起事件
        }
        //監聽單位是否掛掉，只要註冊一次就好了
        if (damageables.Count == 1) Bus<UnitDeathEvent>.Subscribe(HandleUnitDeath);
    }

    //任何具有Rigdbody元件者離開時，就會觸發此
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable enterUnit))
        {
            damageables.Remove(enterUnit);
            OnUnitExit?.Invoke(enterUnit); //喚起事件
        }
        //沒人的時候取消監聽
        if (damageables.Count == 0) Bus<UnitDeathEvent>.Unsubscribe(HandleUnitDeath);
    }
    private void HandleUnitDeath(UnitDeathEvent evt)
    {
        //這種方式比較好
        if (evt.Unit.TryGetComponent(out IDamageable enterUnit))
        {
            damageables.Remove(enterUnit);
            OnUnitExit?.Invoke(enterUnit); //喚起事件
        }
    }
    private void OnDestroy()
    {
        Bus<UnitDeathEvent>.Unsubscribe(HandleUnitDeath);
    }

    // 給外部用的方法
    public void SetupAttackConfig(UnitAttackConfigSO attackConfigSO)
    {
        collider.radius = attackConfigSO.AttackRange;
    }
}
