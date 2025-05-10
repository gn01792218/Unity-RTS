public interface IGatherable
{
    public GatherableResurceSO resourceSO { get; }
    public int Amount { get; } //還剩下多少資源
    public bool IsBusy { get; } //資源是否正在被其他人獲取

    public bool BeinGather(); //開始獲取
    public int EndGather(); //結束獲取,會回傳獲取了多少資源的數量
}