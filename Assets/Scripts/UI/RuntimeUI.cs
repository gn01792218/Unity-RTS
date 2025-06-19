using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    [SerializeField] private CommandUI commandUI;
    [SerializeField] private BuildBuildingUI buildBuildingUI;
    [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI; //單選時的UI
    [SerializeField] private UnitIconUI unitIconUI; //單選時的Unit Icon顯示
    private HashSet<CommandableUnit> selectedUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.Subscribe(HandleUnitSelected);
        Bus<UnselectedEvent>.Subscribe(HandleUnitUnselected);
        Bus<UnitDeathEvent>.Subscribe(HandleUnitDeath);
        Bus<GatherResourceEvent>.Subscribe(HandleGatherResource);
    }

    private void Start()
    {
        commandUI.Disable();
        buildBuildingUI.Disable();
        unitIconUI.Disable();
        singleUnitSelectedUI.Disable();
    }
    private void OnDestroy()
    {
        Bus<SelectedEvent>.Unsubscribe(HandleUnitSelected);
        Bus<UnselectedEvent>.Unsubscribe(HandleUnitUnselected);
        Bus<UnitDeathEvent>.Unsubscribe(HandleUnitDeath);
        Bus<GatherResourceEvent>.Unsubscribe(HandleGatherResource);
    }
    private void HandleUnitSelected(SelectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectedUnits.Add(unit);
            RefreshUI();
        }
    }
    private void HandleUnitUnselected(UnselectedEvent evt)
    {
        if (evt.SelectdObject is CommandableUnit unit)
        {
            selectedUnits.Remove(unit);
            RefreshUI();
        }
    }
    private void HandleUnitDeath(UnitDeathEvent evt)
    {
        if (evt.Unit is CommandableUnit unit)
        {
            selectedUnits.Remove(unit);
            RefreshUI();
        }
    }
    private void HandleGatherResource(GatherResourceEvent evt)
    {
        //更新CommandUI
        commandUI.EnableFor(selectedUnits);
    }
    private void RefreshUI()
    {
        //框選多的個時候
        if (selectedUnits.Count > 0)
        {
            //1.更新指令UI
            commandUI.EnableFor(selectedUnits);

            //2.更新單選的UI指令
            if (selectedUnits.Count == 1)
            {
                var selectedUnit = selectedUnits.First();
                //1.更新單位圖示UI
                unitIconUI.EnableFor(selectedUnit);
                singleUnitSelectedUI.EnableFor(selectedUnit);
                //2.建築物的UI
                if (selectedUnit is BuildingUnit building)
                {
                    buildBuildingUI.EnableFor(building);
                }
                else
                {
                    buildBuildingUI.Disable();
                }
            }
            else //多選時
            {
                unitIconUI.Disable();
                singleUnitSelectedUI.Disable();
                buildBuildingUI.Disable();
            }
        }
        else //沒東西時
        {
            commandUI.Disable();
            buildBuildingUI.Disable();
            unitIconUI.Disable();
            singleUnitSelectedUI.Disable();
        }
    }
}