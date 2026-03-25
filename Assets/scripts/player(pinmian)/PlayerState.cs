using UnityEngine;

public class TopDownPlayerState
{
    protected TopDownPlayer player;
    protected TopDownStateMachine stateMachine;
    protected string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled = false;

    public TopDownPlayerState(TopDownPlayer player, TopDownStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void AnimationFinishedTrigger()
    {
        triggerCalled = true;
    }
}