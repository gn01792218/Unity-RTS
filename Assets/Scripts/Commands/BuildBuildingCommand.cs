using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Build Building Command", menuName = "Commands/Actions/Build Building Command", order = 120)]
public class BuildBuildingCommand : Command
{
    [field: SerializeField] public BuildingUnitSO buildingSo { get; private set; }
    [field: SerializeField] public GameObject GhostPrefab { get; private set; }  //擺放時候的形體
    [field: SerializeField] public BuildingRestrictionSO[] RestrictionsSO { get; private set; } 
    public override bool CanHandle(CommandContext context)
    {
        //點空地的時候
        if (context.Unit is not IBuildingBuilder) return false;

        //如果點擊到已經存在的建築，則恢復建造
        if (context.Hit.collider != null)
        {
            return context.Hit.collider.TryGetComponent(out BuildingUnit buildingUnit)
                   && buildingUnit.unitSO == buildingSo
                    && (buildingUnit.Progress.State == BuildingProgress.BuildingState.Paused
                     || buildingUnit.Progress.State == BuildingProgress.BuildingState.NotStarted);
        }

        return AllRestrictionsPass(context.Hit.point);
    }

    public override void Handle(CommandContext context)
    {
        IBuildingBuilder builder = (IBuildingBuilder)context.Unit;
        if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BuildingUnit buildingUnit))
        {
            //如果點擊到已經存在的建築，則恢復建造
            builder.ResumeBuilding(buildingUnit);
            return;
        }
        else
        {
            //如果點擊到空地，則開始建造
            //檢查是否符合建造條件
            if (!AllRestrictionsPass(context.Hit.point)) return;
            builder.BuildBuilding(buildingSo, context.Hit.point);
        }
    }

    //檢查所有的限制條件是否都通過
    public bool AllRestrictionsPass(Vector3 point) =>
        RestrictionsSO.Length == 0 || RestrictionsSO.All(restriction => restriction.CanPlace(point));
}