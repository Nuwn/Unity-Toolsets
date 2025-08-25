using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    GameStateMachine gameStateMachine = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameStateMachine.Start();
        gameStateMachine.SetNextState(new PlayState());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class GameStateMachine : StateMachine<GameState> { }

}

public abstract class GameState : IState {

    public object Owner { get; set; }

    public virtual IEnumerator Enter(){ yield break; }

    public virtual IEnumerator Update() { yield break; }

    public virtual IEnumerator Exit() { yield break; }

    protected static Dictionary<Type, GameState> states = new();

    public static T GetOrCreate<T>() where T : GameState, new()
    {
        states.TryGetValue(typeof(T), out var state);

        if (state is not T instance)
        {
            instance = new T();
            RegisterState(instance);
        }

        return instance;
    }

    protected static void RegisterState(GameState state)
    {
        Debug.Log($"Registering {state.GetType().Name} state.");
        states[state.GetType()] = state;
    }
}


public class PlayState : GameState
{
}