using UnityEngine;
[CreateAssetMenu(fileName = "Build Unit Command", menuName = "Commands/Actions/Build Unit Command", order = 120)]
public class BuildUnitCommand : Command
{
   [field: SerializeField] public UnitSO unitSO {get; private set;} //建築這個類別的相關資料
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is BuildingUnit; //檢查是否為BuildingUnit，因為這個類別包含BuildUnit的方法
    }

    public override void Handle(CommandContext context)
    {
        BuildingUnit buildingUnit = context.Unit as BuildingUnit; //將單位轉換為Unit類別，為了獲取AgentRadius
        buildingUnit.BuildUnit(unitSO);
        
    }
}