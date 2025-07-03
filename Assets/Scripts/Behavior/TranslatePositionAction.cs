using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at [Speed] speed", category: "Action/Navigation", id: "7b50c34b580703eb52a82a18e8a715f2")]
public partial class TranslatePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private Animator animator;
    private float endTime;
    private Vector3 direction;
    private Transform selfTransform;
    protected override Status OnStart()
    {
        if (Self.Value == null) return Status.Failure;

        animator = Self.Value.GetComponent<Animator>();

        //1.Calculate how long it takes to reach the target location
        selfTransform = Self.Value.transform;
        endTime = Time.time + CalculateTimeToTarget();
        //2. Calculate the direction to move
        direction = (TargetLocation.Value - selfTransform.position).normalized;
        selfTransform.forward = direction; // Set the forward direction of the transform to face the target location
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Time.time >= endTime) return Status.Success; // If the time has elapsed, return success
        //Move transform in the direction until the time has elapsed
        if (animator != null)animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, Speed.Value); // Set the speed animation parameter
        selfTransform.position += direction * Speed.Value * Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (animator != null) animator.SetFloat(AnimationConstants.MOVE_SPEED_ID, 0f); // Reset speed animation parameter
    }
    private float CalculateTimeToTarget()
    {
        // Calculate the distance to the target location
        float distance = Vector3.Distance(selfTransform.position, TargetLocation.Value);
        // Calculate the time it will take to reach the target location at the specified speed
        return distance / Speed.Value;
    }
}

