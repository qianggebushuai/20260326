using UnityEngine;

public class TopDownMoveState : TopDownPlayerState
{
    public TopDownMoveState(TopDownPlayer player, TopDownStateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetMoving(true);
    }

    public override void Update()
    {
        base.Update();

        if (!player.HasMoveInput())
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        player.UpdateFacingDirection();
        player.SetAnimatorDirection();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 moveInput = player.GetMoveInputNormalized();
        player.SetVelocity(moveInput.x * player.moveSpeed, moveInput.y * player.moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetMoving(false);
    }
}