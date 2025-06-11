using UnityEngine;
[CreateAssetMenu(fileName = "Build Building Command", menuName = "Commands/Actions/Build Building Command", order = 120)]
public class BuildBuildingCommand : Command
{
    [field: SerializeField] public BuildingUnitSO building { get; private set; }
    [field: SerializeField] public GameObject GhostPrefab { get; private set; }  //擺放時候的形體
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is IBuildingBuilder;
    }

    public override void Handle(CommandContext context)
    {
        IBuildingBuilder builder = (IBuildingBuilder)context.Unit;
        builder.BuildBuilding(building, context.Hit.point);
    }
}