using TMPro;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mineralsAmountText;
    [SerializeField] private TextMeshProUGUI gasAmountText;
    [SerializeField] private TextMeshProUGUI populationAmountText;

    [SerializeField] private GatherableResurceSO mineralSO;
    [SerializeField] private GatherableResurceSO gasSO;

    public static int Minerals { get; private set; }
    public static int Gas { get; private set; }
    public static int Population { get; private set; }
    public static int MaxPopulation { get; private set; } = 200;

    private void Awake()
    {
        Bus<GatherResourceEvent>.OnEvent += OnGatherResource;
    }
    private void OnDestroy()
    {
        Bus<GatherResourceEvent>.OnEvent -= OnGatherResource;
    }
    private void OnGatherResource(GatherResourceEvent e)
    {
        if (e.ResourceSO.Equals(mineralSO))
        {
            Minerals += e.Amount;
            mineralsAmountText.SetText(Minerals.ToString());
        }
        else if (e.ResourceSO.Equals(gasSO))
        {
            Gas += e.Amount;
            gasAmountText.SetText(Gas.ToString());
        }
    }

}