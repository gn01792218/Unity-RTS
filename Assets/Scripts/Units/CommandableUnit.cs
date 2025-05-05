using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class CommandableUnit : MonoBehaviour, ISelectable
{
    [field: SerializeField] public Command[] AvailableCommands { get; private set; } //裝載各種指令
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [SerializeField] private UnitSO unitSO; // 這個單位的數據
    [SerializeField] private DecalProjector onSelectDecal; // 被選中時的標籤貼紙
    protected virtual void Awake()
    {
        onSelectDecal.gameObject.SetActive(false); // 初始化時隱藏標籤貼紙
    }
    protected virtual void Start() //提醒子類呼叫這個方法將會造成覆寫
    {
        // 初始化當前血量
        CurrentHealth = unitSO.Health;
        MaxHealth = unitSO.Health;
    }
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
}