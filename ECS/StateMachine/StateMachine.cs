using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedSceneManager.Utility;
using UnityEngine;

public interface IState
{
    protected static Dictionary<Type, IState> states;

    public object Owner { get; set; }

    IEnumerator Enter();
    IEnumerator Update();
    IEnumerator Exit();
}

public abstract class StateMachine<T> where T : class, IState
{
    public enum When { Before, After }
    public enum Stage { Enter, Update, Exit }

    private T _currentState;
    private T _nextState;
    private GlobalCoroutine _stateUpdateCoroutine;

    private readonly Dictionary<(When, Stage), List<Action<T>>> _callbacks = new();

    public void Callback(Action<T> callback, When when, Stage stage)
    {
        var key = (when, stage);
        if (!_callbacks.ContainsKey(key))
        {
            _callbacks[key] = new List<Action<T>>();
        }
        _callbacks[key].Add(callback);
    }

    public void SetNextState(T state)
    {
        _nextState = state;
        Debug.Log($"StateMachine: Transitioning to {state.GetType().Name}");
    }

    public bool IsInState<U>() where U : T
    {
        return _currentState != null && _currentState is U;
    }

    public void Start()
    {
        StateMachineLoop().StartCoroutine();
    }

    public void Stop()
    {
        _stateUpdateCoroutine?.Stop();
        _stateUpdateCoroutine = null;
    }

    private IEnumerator StateMachineLoop()
    {
        while (true)
        {
            if (_nextState != null)
            {
                yield return TransitionState();
            }

            if (_currentState != null && _stateUpdateCoroutine == null)
            {
                _stateUpdateCoroutine = HandleStateUpdate().StartCoroutine();
            }

            yield return null;
        }
    }

    private IEnumerator TransitionState()
    {
        StopCurrentStateUpdate();

        if (_currentState != null)
        {
            yield return HandleStateStage(Stage.Exit, _currentState);
        }

        _currentState = _nextState;
        _nextState = null;

        if (_currentState != null)
        {
            yield return HandleStateStage(Stage.Enter, _currentState);
        }
    }

    private IEnumerator HandleStateUpdate()
    {
        while (_nextState == null)
        {
            if (_currentState != null)
            {
                yield return _currentState.Update();
            }
        }
    }

    private void StopCurrentStateUpdate()
    {
        if (_stateUpdateCoroutine != null)
        {
            _stateUpdateCoroutine.Stop();
            _stateUpdateCoroutine = null;
        }
    }

    private IEnumerator HandleStateStage(Stage stage, T state)
    {
        InvokeCallbacks(When.Before, stage, state);

        IEnumerator routine = stage switch
        {
            Stage.Enter => state.Enter(),
            Stage.Exit => state.Exit(),
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, "Invalid state stage.")
        };

        yield return routine;

        InvokeCallbacks(When.After, stage, state);
    }

    private void InvokeCallbacks(When when, Stage stage, T state)
    {
        var key = (when, stage);
        if (_callbacks.TryGetValue(key, out var callbackList))
        {
            foreach (var callback in callbackList)
            {
                callback.Invoke(state);
            }
        }
    }
}
