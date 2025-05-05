using UnityEngine;

public abstract class Command : ScriptableObject, ICommand
{
    [field: SerializeField] public Sprite Icon {get; private set;} //此指令顯示的UI圖示
    [field: Range(0,8)][field: SerializeField] public int SlotIndex{get; private set;} //該指令位於指令容器中的第幾格Index
    [field: SerializeField] public bool RequiresClickToActive {get; private set;} = true;
    public abstract bool CanHandle(CommandContext context);
    public abstract void Handle(CommandContext context);
}