using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine 
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState startState)
    {
        this.currentState = startState;
        this.currentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        this.currentState.Exit();
        this.currentState = newState;
        this.currentState.Enter();
    }
}
