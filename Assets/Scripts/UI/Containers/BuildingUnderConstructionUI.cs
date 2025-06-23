using System.Collections;
using TMPro;
using UnityEngine;

public class BuildingUnderConstructionUI : MonoBehaviour, IUIElement<BuildingUnit>
{
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private ProgressBar progressBar;
    public void EnableFor(BuildingUnit building)
    {
        gameObject.SetActive(true);
        unitName.SetText(building.unitSO.UnitName);
        InitProgressBar(building);
        StartCoroutine(AnimateBuildingProgress(building));
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateBuildingProgress(BuildingUnit building)
    {
        while (enabled && building.Progress.Progress < 1)
        {
            if (building.Progress.State != BuildingProgress.BuildingState.Building)
            {
                yield return null;
                continue;
            }

            SetCurrentProgress(building);
            yield return null;
        }
    }

    private void InitProgressBar(BuildingUnit building)
    {
        float startTime = building.Progress.StartTime;
        float endTime = startTime + building.unitSO.BuildTime;

        progressBar.SetProgress(Mathf.Clamp01(startTime / (endTime - startTime)));
    }

    private void SetCurrentProgress(BuildingUnit building)
    {
        float startTime = building.Progress.StartTime;
        float endTime = startTime + building.unitSO.BuildTime;

        progressBar.SetProgress(Mathf.Clamp01((Time.time - startTime) / (endTime - startTime)));
    }
}