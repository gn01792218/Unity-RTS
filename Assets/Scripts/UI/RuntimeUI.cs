using System.Collections.Generic;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    [SerializeField] private CommandUI commandUI;
    [SerializeField] private BuildBuildingUI buildBuildingUI;
    private HashSet<CommandableUnit> selectedUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnselectedEvent>.OnEvent += HandleUnitUnselected;
    }
    private void Start()
    {
        commandUI.Disable();
        buildBuildingUI.Disable();
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
        if(evt.SelectdObject is BuildingUnit building && selectedUnits.Count == 1 ) //只有單選到building才會顯示唷
        {
            buildBuildingUI.EnableFor(building);
        }
    }
    private void HandleUnitUnselected(UnselectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectedUnits.Remove(unit);
            commandUI.Disable();
            if(unit is BuildingUnit)
            {
                buildBuildingUI.Disable();
            }
        }
    }
}