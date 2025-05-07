using System.Collections.Generic;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    [SerializeField] private CommandUI commandUI;
    private HashSet<CommandableUnit> selectedUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnselectedEvent>.OnEvent += HandleUnitUnselected;
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
            selectedUnits.Add(unit);
            commandUI.EnableFor(selectedUnits);
        }
    }
    private void HandleUnitUnselected(UnselectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectedUnits.Remove(unit);
            commandUI.Disable();
        }
    }

}