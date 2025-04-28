using UnityEngine;

public class AerialAttackState : PlayerBaseState
{
private float enterTime;

    public AerialAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[AerialAttackState] Entering AerialAttackState State at {enterTime:F2}s");
        stateMachine.aerialAttackObject.SetActive(true);
    }

    public override void Tick(float deltaTime)
    {
        if(stateMachine.wasGroundedLastFrame)
        {
            stateMachine.SwitchState(stateMachine.IdleState);
            //stateMachine.aerialAttackObject.SetActive(false);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[AerialAttackState] Exiting AerialAttackState after {Time.time - enterTime:F2}s");
    }
}
