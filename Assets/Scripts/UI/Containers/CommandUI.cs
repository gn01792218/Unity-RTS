using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CommandUI : MonoBehaviour, IUIElement<HashSet<CommandableUnit>>
{
    [SerializeField] private CommandButton[] commandButtons;

    public void EnableFor(HashSet<CommandableUnit> selectUnits)
    {
        RefreshButtons(selectUnits);
    }

    public void Disable()
    {
         foreach(CommandButton button in commandButtons){
            button.Disable();
        }
    }
    private void RefreshButtons(HashSet<CommandableUnit> selectunits)
    {
        IEnumerable<Command> availableCommands = selectunits.ElementAt(0).AvailableCommands; //按鈕介面最多可以放9個按鈕而已
        foreach(CommandableUnit unit in selectunits){
            if(unit.AvailableCommands != null) availableCommands = availableCommands.Intersect(unit.AvailableCommands); // 用Intersect找出所有選中單位的可用指令之交集
        }
        for(int i =0; i< commandButtons.Length; i++){
            Command commandForSlot = availableCommands.Where(command => command.SlotIndex == i && command.ShowInCommandUI).FirstOrDefault();
            if(commandForSlot != null){
                commandButtons[i].EnableFor(commandForSlot, selectunits ,HandleClick(commandForSlot));
            }
            else
            {
                commandButtons[i].Disable();
            }
        }
    }
    private UnityAction HandleClick(Command command)
    {
        return ()=> Bus<CommandSelectedEvent>.Publish(new CommandSelectedEvent(command)); //點擊時發送該指令事件
    }
}
