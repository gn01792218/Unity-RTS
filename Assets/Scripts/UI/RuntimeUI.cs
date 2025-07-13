using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeUI : MonoBehaviour
{
    [SerializeField] private CommandUI commandUI;
    [SerializeField] private BuildingSelectedUI buildingSelectedUI;
    [SerializeField] private UnitIconUI unitIconUI; //單選時的Unit Icon顯示
    [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI; //單選時的UI
    [SerializeField] private UnitTransportUI unitTransportUI; 
    private HashSet<CommandableUnit> selectedUnits = new(12);
    private void Awake()
    {
        Bus<SelectedEvent>.Subscribe(HandleUnitSelected);
        Bus<UnselectedEvent>.Subscribe(HandleUnitUnselected);
        Bus<UnitDeathEvent>.Subscribe(HandleUnitDeath);
        Bus<GatherResourceEvent>.Subscribe(HandleGatherResource);
        Bus<UnitLoadEvent>.Subscribe(HandleUnitLoaded);
        Bus<UnitUnLoadEvent>.Subscribe(HandleUnitUnLoad);
    }

    private void Start()
    {
        commandUI.Disable();
        buildingSelectedUI.Disable();
        unitIconUI.Disable();
        singleUnitSelectedUI.Disable();
        unitTransportUI.Disable();
    }
    private void OnDestroy()
    {
        Bus<SelectedEvent>.Unsubscribe(HandleUnitSelected);
        Bus<UnselectedEvent>.Unsubscribe(HandleUnitUnselected);
        Bus<UnitDeathEvent>.Unsubscribe(HandleUnitDeath);
        Bus<GatherResourceEvent>.Unsubscribe(HandleGatherResource);
        Bus<UnitLoadEvent>.Unsubscribe(HandleUnitLoaded);
        Bus<UnitUnLoadEvent>.Unsubscribe(HandleUnitUnLoad);
    }
    private void HandleUnitLoaded(UnitLoadEvent evt)
    {
        if (selectedUnits.Count == 1 && selectedUnits.First() is ITransporter)
        {
            RefreshUI();
        }
        else if (evt.Unit is CommandableUnit commandable && selectedUnits.Contains(commandable))
        {
            commandable.OnDeselect(); //這裡面會呼叫UnselectedEvent去觸發ResresfUI
        }

    }
    private void HandleUnitUnLoad(UnitUnLoadEvent evt)
    {
        if (selectedUnits.Count == 1 && selectedUnits.First() is ITransporter)
        {
            RefreshUI();
        }
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
        // if (evt.Unit is CommandableUnit unit)
        // {
            selectedUnits.Remove(evt.Unit);
            RefreshUI();
        // }
    }
    private void HandleGatherResource(GatherResourceEvent evt)
    {
        //更新CommandUI
        commandUI.EnableFor(selectedUnits);
    }
    private void RefreshUI()
    {
        //框1個獲選多的個時候
        if (selectedUnits.Count > 0)
        {
            //1.更新指令UI
            commandUI.EnableFor(selectedUnits);

            //2.更新單選的UI指令
            if (selectedUnits.Count == 1)
            {
                ResolvesSingleUnitSelectedUI();
            }
            else //多選時
            {
                unitIconUI.Disable();
                singleUnitSelectedUI.Disable();
                buildingSelectedUI.Disable();
                unitTransportUI.Disable();
            }
        }
        else //沒東西時
        {
            DisableAllContainers();
        }
    }

    private void ResolvesSingleUnitSelectedUI()
    {
        var selectedUnit = selectedUnits.First();
        //1.更新單位圖示UI
        unitIconUI.EnableFor(selectedUnit);

        //2.建築物的UI
        if (selectedUnit is BuildingUnit building)
        {
            singleUnitSelectedUI.Disable();
            unitTransportUI.Disable();
            buildingSelectedUI.EnableFor(building);
        }
        else if (selectedUnit is ITransporter transpoter && transpoter.UsedCapacity > 0)
        {
            unitTransportUI.EnableFor(transpoter);
            singleUnitSelectedUI.Disable();
            buildingSelectedUI.Disable();
        }
        else //選到其他的單位時
        {
            buildingSelectedUI.Disable();
            unitTransportUI.Disable();
            singleUnitSelectedUI.EnableFor(selectedUnit);
        }
    }

    private void DisableAllContainers()
    {
        commandUI.Disable();
        buildingSelectedUI.Disable();
        unitIconUI.Disable();
        singleUnitSelectedUI.Disable();
        unitTransportUI.Disable();
    }
}