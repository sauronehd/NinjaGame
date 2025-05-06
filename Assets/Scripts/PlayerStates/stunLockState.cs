using UnityEngine;

public class stunLockState : PlayerBaseState
{
    public stunLockState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float enterTime;
    public float stunTime;
    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[stunLockState] Entering stunLockState State at {enterTime:F2}s");
        Physics.SyncTransforms();
    }


    public override void Tick(float deltaTime)
    {
        if((Time.time - enterTime) >= stunTime)
        {
            if(stateMachine.wasGroundedLastFrame)
            {
                stateMachine.SwitchState(stateMachine.IdleState);
            }
            else
            {
                stateMachine.SwitchState(stateMachine.FallState);
            }
        }

    }


    public override void Exit()
    {
        Debug.Log($"[stunLockState] Exiting stunLockState after {Time.time - enterTime:F2}s");
    }

}
