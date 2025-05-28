public struct GatherResourceEvent : IEvent
{
    public int Amount { get; private set; } //採集的數量
    public GatherableResurceSO ResourceSO { get; private set; } //採集的資源SO

    public GatherResourceEvent(int amount, GatherableResurceSO resources)
    {
        Amount = amount;
        ResourceSO = resources;
    }
}