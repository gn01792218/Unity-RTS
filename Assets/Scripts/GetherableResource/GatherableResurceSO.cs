using UnityEngine;

[CreateAssetMenu(fileName = "GatherableResurceSO", menuName = "SO/GatherableResurceSO", order = 5)]
public class GatherableResurceSO : ScriptableObject
{
   [field: SerializeField] public int MaxAmount { get; private set; } = 1500;
   [field: SerializeField] public int AmountPerGather { get; private set; } = 8; //每次可以拿多少量
   [field: SerializeField] public float BaseGatherTime { get; private set; } = 1.5f;
}