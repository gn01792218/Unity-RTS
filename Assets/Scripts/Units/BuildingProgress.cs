using UnityEngine;
public struct BuildingProgress
{
    public enum BuildingState
    {
        NotStarted,
        Building,
        Paused,
        Completed,
        Destroyed
    }
    [field: SerializeField] public float StartTime { get; private set; }
    [field: SerializeField] public float Progress { get; private set; }
    [field: SerializeField] public BuildingState State { get; private set; }

    public BuildingProgress(float startTime, float progress, BuildingState state)
    {
        StartTime = startTime;
        Progress = progress;
        State = state;
    }
}