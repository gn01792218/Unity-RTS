using UnityEngine;
[CreateAssetMenu(fileName = "Build Building Command", menuName = "Commands/Actions/Build Building Command", order = 120)]
public class BuildBuildingCommand : Command
{
    [field: SerializeField] public BuildingUnitSO buildingSo { get; private set; }
    [field: SerializeField] public GameObject GhostPrefab { get; private set; }  //擺放時候的形體
    public override bool CanHandle(CommandContext context)
    {
        if (context.Unit is not IBuildingBuilder) return false;
        if (context.Hit.collider != null) //handle 點擊未建造完成的建築時
        {
            return context.Hit.collider.TryGetComponent(out BuildingUnit buildingUnit)
                   && buildingUnit.unitSO == buildingSo
                    && (buildingUnit.Progress.State == BuildingProgress.BuildingState.Paused
                     || buildingUnit.Progress.State == BuildingProgress.BuildingState.NotStarted);
        }
        return true;
    }

    public override void Handle(CommandContext context)
    {
        IBuildingBuilder builder = (IBuildingBuilder)context.Unit;
        Debug.Log($"建造開始 {context.Unit}{buildingSo}");
        if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BuildingUnit buildingUnit))
        {
            //如果點擊到已經存在的建築，則恢復建造
            Debug.Log("點到建築物");
            builder.ResumeBuilding(buildingUnit);
            return;
        }
        else
        {
            //如果點擊到空地，則開始建造
            Debug.Log($"Builder {builder} build {buildingSo.name} at {context.Hit.point}");
            builder.BuildBuilding(buildingSo, context.Hit.point);
        }
    }
}