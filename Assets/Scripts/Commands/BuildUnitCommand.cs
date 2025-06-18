using UnityEngine;
[CreateAssetMenu(fileName = "Build Unit Command", menuName = "Commands/Actions/Build Unit Command", order = 120)]
public class BuildUnitCommand : Command
{
    [field: SerializeField] public UnitSO unitSO { get; private set; } //建築這個類別的相關資料
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is BuildingUnit && HaveEnoughResources();
    }

    public override void Handle(CommandContext context)
    {
        if (!HaveEnoughResources()) return;
        BuildingUnit buildingUnit = context.Unit as BuildingUnit; //將單位轉換為Unit類別，為了獲取AgentRadius
        buildingUnit.BuildUnit(unitSO);
    }
    private bool HaveEnoughResources()
    {
        //檢查是否有足夠的資源來建造單位
        return unitSO.ResourceCostSO.MineralCost <= PlayerResources.Minerals &&
            unitSO.ResourceCostSO.GasCost <= PlayerResources.Gas;
    }
}