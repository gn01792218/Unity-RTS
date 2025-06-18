using TMPro;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mineralsAmountText;
    [SerializeField] private TextMeshProUGUI gasAmountText;
    [SerializeField] private TextMeshProUGUI populationAmountText;
    [SerializeField] private GatherableResurceSO mineralSO;
    [SerializeField] private GatherableResurceSO gasSO;

    // 靜態引用
    private static GatherableResurceSO _mineralSO;
    private static GatherableResurceSO _gasSO;

    // 公開的靜態屬性
    public static GatherableResurceSO MineralSO => _mineralSO;
    public static GatherableResurceSO GasSO => _gasSO;

    // 現有的靜態資源數量屬性
    public static int Minerals { get; private set; }
    public static int Gas { get; private set; }
    public static int Population { get; private set; }
    public static int MaxPopulation { get; private set; } = 200;

    private void Awake()
    {
        // 初始化靜態引用
        _mineralSO = mineralSO ?? Resources.Load<GatherableResurceSO>("DefaultResources/MineralSO");
        _gasSO = gasSO ?? Resources.Load<GatherableResurceSO>("DefaultResources/GasSO");

        if (_mineralSO == null || _gasSO == null)
        {
            Debug.LogError("Resource SOs not found!", this);
        }

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