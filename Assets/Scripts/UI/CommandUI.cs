using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandUI : MonoBehaviour
{
    [SerializeField] private CommandButton[] commandButtons;
    private HashSet<CommandableUnit> selectUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnselectedEvent>.OnEvent += HandleUnitUnselected;

        //初始化按鈕
        foreach(CommandButton button in commandButtons){
            button.SetIcon(null);
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
                commandButtons[i].SetIcon(commandForSlot.Icon);
            }
            else
            {
                commandButtons[i].SetIcon(null);
            }
        }
    }
}
