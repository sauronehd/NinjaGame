using UnityEngine;

public class UpTiltState : PlayerBaseState
{
private float enterTime;

    public UpTiltState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[UpTiltState] Entering UpTiltState State at {enterTime:F2}s");
        stateMachine.upTiltObject.SetActive(true);
        stateMachine.jabObject.SetActive(false);
    }

    public override void Tick(float deltaTime)
    {
        if((Time.time-enterTime)>0.3)
        {
            stateMachine.SwitchState(stateMachine.IdleState);
            stateMachine.upTiltObject.SetActive(false);
        }
        else if(Time.time-enterTime>0.4)
        {

        }


    }

    public override void Exit()
    {
        Debug.Log($"[UpTiltState] Exiting UpTiltState after {Time.time - enterTime:F2}s");
    }
}
