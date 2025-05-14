using UnityEngine;

public class SideTiltState : PlayerBaseState
{
private float enterTime;

    public SideTiltState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        enterTime = Time.time;
        Debug.Log($"[SideTiltState] Entering SideTiltState State at {enterTime:F2}s");
        stateMachine.sideTiltObject.SetActive(true);
        stateMachine.sideTiltObject.GetComponent<meleeAttackScript>().consumed = false;
        Physics.SyncTransforms();
    }

    public override void Tick(float deltaTime)
    {
        if((Time.time-enterTime)>6)
        {
            stateMachine.sideTiltObject.SetActive(false);
        }
        
        if(Time.time-enterTime>7)
        {
            stateMachine.SwitchState(stateMachine.IdleState);
            stateMachine.sideTiltObject.SetActive(false);
        }


    }

    public override void Exit()
    {
        Debug.Log($"[SideTiltState] Exiting SideTiltState after {Time.time - enterTime:F2}s");
    }
}
