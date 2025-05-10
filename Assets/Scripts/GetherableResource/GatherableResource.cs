using UnityEngine;

public class GatherableResource : MonoBehaviour, IGatherable
{
    [field: SerializeField] public GatherableResurceSO resourceSO { get; private set; }

    [field: SerializeField] public int Amount { get; private set; }

    [field: SerializeField] public bool IsBusy { get; private set; }

    private void Start()
    {
        Amount = resourceSO.MaxAmount;
    }

    public bool BeinGather()
    {
        if (IsBusy) return false; //已經有人採時，回傳false不給採

        IsBusy = true; //給採，設置為true，下一個人進來，將進入上面那段
        return true; //給採
    }

    public int EndGather()
    {
        IsBusy = false;
        int amountGathered = Mathf.Min(resourceSO.AmountPerGather, Amount); //取每次可以拿取的量、或當前剩餘量的最小值
        Amount -= amountGathered;

        if (Amount <= 0) Destroy(gameObject);
        return amountGathered;
    }
}