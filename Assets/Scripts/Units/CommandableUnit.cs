using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class CommandableUnit : MonoBehaviour, ISelectable, IDamageable
{
    [field: SerializeField] public bool IsSelected { get; protected set; }
    [field: SerializeField] public Command[] AvailableCommands { get; private set; } //裝載各種指令
    [field: SerializeField] public int CurrentHealth { get; protected set; }
    [field: SerializeField] public int MaxHealth { get; protected set; }
    [field: SerializeField] public UnitSO unitSO { get; private set; } // 這個單位的數據
    [field: SerializeField] public Owner Owner { get; set; }
    [SerializeField] protected DecalProjector onSelectDecal; // 被選中時的標籤貼紙
    public Transform Transform => transform;


    //定義血量更新事件
    public delegate void HealthUpdatedEvent(CommandableUnit unit, int lastHealth, int newHealth);
    public event HealthUpdatedEvent OnHealthUpdated;
    private Command[] initialAvailableCommands; //表示第一層(頁)的指令列表

    protected virtual void Awake()
    {
        onSelectDecal.gameObject.SetActive(false); // 初始化時隱藏標籤貼紙
    }
    protected virtual void Start() //提醒子類呼叫這個方法將會造成覆寫
    {
        // 初始化當前血量
        CurrentHealth = unitSO.Health;
        MaxHealth = unitSO.Health;

        initialAvailableCommands = AvailableCommands;
    }
    public virtual void OnSelect()
    {
        onSelectDecal.gameObject.SetActive(true); // Enable the decal projector
        IsSelected = true;
        //發送被選到的事件,
        // ps.監聽事件者要負責將該單位添加到選取列表中
        Bus<SelectedEvent>.Publish(new SelectedEvent(this));
    }

    public virtual void OnDeselect()
    {
        onSelectDecal.gameObject.SetActive(false); // Disable the decal projector when deselected
        IsSelected = false;
        //發送取消選取的事件
        // ps.監聽事件者要負責將該單位從選取列表中移除
        OverridesAvailableCommands(null); //傳入null會恢復到該單位的初始化指令列表
        Bus<UnselectedEvent>.Publish(new UnselectedEvent(this));
    }

    public void OverridesAvailableCommands(Command[] commands) // 覆蓋可用的指令列表，即更新可用指令
    {
        if (commands == null || commands.Length == 0)
        {
            AvailableCommands = initialAvailableCommands;
        }
        else
        {
            AvailableCommands = commands;
        }

        //通知UI更新
        //因為單位被選中時會刷新UI
        //之後可以考慮新增一個更明確的事件，目前先偷懶
        if (IsSelected) Bus<SelectedEvent>.Publish(new SelectedEvent(this));
    }
    public void TakeDamage(int damage)
    {
        int lastHealth = (int)CurrentHealth;
        //Clamp，第一個參數是計算式；第二個參數是最小值限制；第三個參數是最大限制
        //使用Clamp確保計算結果CurrentHealth介於0~CurrentHealth之間
        CurrentHealth = (int)Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);
        //更新血量
        OnHealthUpdated?.Invoke(this, lastHealth, (int)CurrentHealth);
        if (CurrentHealth == 0) Die();
    }
    public void Die()
    {
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        int lastHealth = (int)CurrentHealth;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);

        //更新血量
        OnHealthUpdated?.Invoke(this, lastHealth, (int)CurrentHealth);
    }
}