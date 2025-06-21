using UnityEngine;

public abstract class Command : ScriptableObject, ICommand
{
    [field: SerializeField] public string CommandName { get; private set; } = "Command";
    [field: SerializeField] public bool IsSingleUnitCommand { get; private set; } = false; //大部分的都可以直接套用到所有Unit上
    [field: SerializeField] public Sprite Icon { get; private set; } //此指令顯示的UI圖示
    [field: Range(0, 8)][field: SerializeField] public int SlotIndex { get; private set; } //該指令位於指令容器中的第幾格Index
    [field: SerializeField] public bool RequiresClickToActive { get; private set; } = true;
    [field: SerializeField] public bool ShowInCommandUI { get; private set; } = true; //是否需要顯示在即時UI上
    public abstract bool CanHandle(CommandContext context);
    public abstract void Handle(CommandContext context);
    public abstract bool IsAvailable(CommandContext? context = null); //該指令可不可使用
}