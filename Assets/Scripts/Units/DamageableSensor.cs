using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]  //需要Collider元件，來偵測trigger
public class DamageableSensor : MonoBehaviour
{
    private HashSet<IDamageable> damageables = new(); //內部操作的

    public List<IDamageable> Damageables => damageables.ToList(); //供外部觀看的，防止元列表被汙染

    //與外溝通的事件
    public delegate void UnitDetectionEvent(IDamageable damageable);
    public event UnitDetectionEvent OnUnitEnter;
    public event UnitDetectionEvent OnUnitExit;


    //只要有任何具有Rigdbody元件者進入，就會觸發此
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable enterUnit))
        {
            damageables.Add(enterUnit);
            OnUnitEnter?.Invoke(enterUnit); //喚起事件
        }
    }
    
    //任何具有Rigdbody元件者離開時，就會觸發此
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable enterUnit))
        {
            damageables.Remove(enterUnit);
            OnUnitExit?.Invoke(enterUnit); //喚起事件
        }
    }
}
