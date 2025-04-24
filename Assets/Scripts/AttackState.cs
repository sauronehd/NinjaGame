using UnityEngine;

public class AttackState : PlayerBaseState
{
private float enterTime;

    public AttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[AttackState] Entering Attack State at {enterTime:F2}s");
    }

    public override void Tick(float deltaTime)
    {
        if((Time.time-enterTime)>0.3)
        {
            playerStateMachine.SwitchState(playerStateMachine.IdleState);
            //play idle animation
            //set knife to inactive
        }
        else
        {
            //rotate the knife to show attack
            //also set knife to active
        }
    }

    public override void Exit()
    {
        Debug.Log($"[AttackState] Exiting Attack State after {Time.time - enterTime:F2}s");
    }
}
