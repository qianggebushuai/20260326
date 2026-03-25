using UnityEngine;

public class TopDownIdleState : TopDownPlayerState
{
    public TopDownIdleState(TopDownPlayer player, TopDownStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.StopMovement();

        player.SetMoving(false);

        player.SetAnimatorDirection();
    }

    public override void Update()
    {
        base.Update();

        if (player.HasMoveInput())
        {
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}