using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMashine<TState,TTrigger>
{
   
    public TState CurrentState { get; private set; }
    public event Action<StateChangeData<TState,TTrigger>> OnStateChange;
    public Dictionary<TState, List<Transition<TState, TTrigger>>> _transitions = new();

    public StateMashine(TState initialState)
    {
        CurrentState = initialState;
    }
    public void SetTrigger(TTrigger trigger)
    {
        if (!_transitions.ContainsKey(CurrentState))
        {
            Debug.LogAssertion($"There is no exit from state {CurrentState} !!! ");
            return;
        }

        foreach (var transition in _transitions[CurrentState] )
        {
            if (transition.Trigger.Equals(trigger))
            {
                var oldState = CurrentState;
                CurrentState = transition.NextState;
                OnStateChange?.Invoke(new(CurrentState, oldState, trigger));
                return;
            }

        }

        Debug.LogAssertion($"There is no exit from {CurrentState} by triger {trigger}");
    }

    public void AddTransition(TState from, TTrigger byTrigger , TState to)
    {
        if (!_transitions.ContainsKey(from))
            _transitions.Add(from , new());

        foreach (var transition in _transitions[from])
        {
            if (transition.Trigger.Equals(byTrigger))
            {
                Debug.LogAssertion($"The transition from state {from} by trigger {byTrigger} is already exists !!");
                return;
            }
        }

        _transitions[from].Add(new Transition<TState, TTrigger>(to, byTrigger));
    }


}
public class Transition<TState, TTrigger>
{
    public TState NextState { get; }
    public TTrigger Trigger { get; }

    public Transition(TState nextState, TTrigger trigпer)
    {
        NextState = nextState;
        Trigger = trigпer;
    }


}

public class StateChangeData<TState, TTrigger>
{
   public TState NewState { get; }
    public TState OldState { get; }
    public TTrigger Trigger { get; }

    public StateChangeData(TState newState, TState oldState, TTrigger trigger)
    {
        NewState = newState;
        OldState = oldState;
        Trigger = trigger;

    } 
        

}
