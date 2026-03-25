using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownStateMachine
{
    public TopDownPlayerState currentState { get; private set; }

    public void Initialize(TopDownPlayerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(TopDownPlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}