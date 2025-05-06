using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CommandUI : MonoBehaviour
{
    [SerializeField] private CommandButton[] commandButtons;
    private HashSet<CommandableUnit> selectUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnselectedEvent>.OnEvent += HandleUnitUnselected;

    }
    private void Start() //和Componant相關的在Start使用
    {
        //初始化按鈕
        foreach(CommandButton button in commandButtons){
            button.Disable();
        }
    }
    private void ODestroy()
    {
        Bus<SelectedEvent>.Unsubscribe(HandleUnitSelected);
        Bus<UnselectedEvent>.Unsubscribe(HandleUnitUnselected);
    }
    private void HandleUnitSelected(SelectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectUnits.Add(unit);
            RefreshButtons();
        }
    }
    private void HandleUnitUnselected(UnselectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectUnits.Remove(unit);
            RefreshButtons();
        }
    }
    private void RefreshButtons()
    {
        HashSet<Command> availableCommands = new(9); //按鈕介面最多可以放9個按鈕而已
        foreach(CommandableUnit unit in selectUnits){
            availableCommands.UnionWith(unit.AvailableCommands); //使用UnionWith確保不會重複+到一樣的指令
        }
        for(int i =0; i< commandButtons.Length; i++){
            Command commandForSlot = availableCommands.Where(command => command.SlotIndex == i).FirstOrDefault();
            if(commandForSlot != null){
                commandButtons[i].EnableFor(commandForSlot, HandleClick(commandForSlot));
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
