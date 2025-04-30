using UnityEngine;

public class AttackState : PlayerBaseState
{
private float enterTime;

    public AttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private bool resolves = false;

    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[AttackState] Entering Attack State at {enterTime:F2}s");
        if(stateMachine.InputReader.IsUpPressed())
        {
            //print("UpTilt");
            stateMachine.SwitchState(stateMachine.UpTiltState);
            return;
        }
        else
        {
            resolves = true;
        }
    }

    public override void Tick(float deltaTime)
    {
        if((Time.time-enterTime)>0.3)
        {
            stateMachine.SwitchState(stateMachine.IdleState);
            stateMachine.jabObject.SetActive(false);
        }
        else if(resolves)
        {
            stateMachine.jabObject.SetActive(true);
        }
    }

    public override void Exit()
    {
        Debug.Log($"[AttackState] Exiting Attack State after {Time.time - enterTime:F2}s");
        
    }
}
