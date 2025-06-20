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
            Debug.Log(building.Progress.Progress);
            if (building.Progress.State != BuildingProgress.BuildingState.Building)
            {
                yield return null;
                continue;
            }

            float startTime = building.Progress.StartTime;
            float endTime = startTime + building.unitSO.BuildTime;

            progressBar.SetProgress(Mathf.Clamp01((Time.time - startTime) / (endTime - startTime)));
            Debug.Log($"設置建築物建造中的progressBar{Mathf.Clamp01((Time.time - startTime) / (endTime - startTime))}");
            yield return null;
        }
    }
}