using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Manages the game's states and their transitions.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    private GameState _currentState;
    private GameState _nextState;
    private Coroutine _stateUpdateCoroutine;

    private Entity _gameStateEntity;
    private EntityManager _entityManager;

    public Action<bool> OnApplicationPauseEvent;
    public Action<bool> OnApplicationFocusEvent;

    public void ChangeState<T>() where T : GameState, new() =>
        _nextState = GameState.GetOrCreate<T>();

    private void Start()
    {
        InitializeEntity();
        StartCoroutine(StateMachineLoop());
    }


    private void OnApplicationFocus(bool focus) => 
        OnApplicationFocusEvent?.Invoke(focus);

    private void OnApplicationPause(bool pause) => 
        OnApplicationPauseEvent?.Invoke(pause);

    private void InitializeEntity()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _gameStateEntity = _entityManager.CreateEntity();
        _entityManager.SetName(_gameStateEntity, "GameStateEntity");
    }

    // The main state machine loop that handles state transitions and updates.
    private IEnumerator StateMachineLoop()
    {
        while (true)
        {
            if (_nextState != null)
                yield return HandleStateTransition();

            if (_currentState != null && _stateUpdateCoroutine == null)
                _stateUpdateCoroutine = StartCoroutine(HandleStateUpdate());

            yield return null;
        }
    }

    private IEnumerator HandleStateTransition()
    {
        if (_stateUpdateCoroutine != null)
        {
            StopCoroutine(_stateUpdateCoroutine);
            _stateUpdateCoroutine = null;
        }

        if (_currentState != null)
        {
            EnableTag(_currentState, false);
            yield return _currentState.Exit();
        }

        _currentState = _nextState;
        _nextState = null;

        if (_currentState != null)
            yield return _currentState.Enter();

        EnableTag(_currentState, true);
    }

    private IEnumerator HandleStateUpdate()
    {
        while (_nextState == null)
        {
            if (_currentState != null)
                yield return _currentState.Update();
        }
    }

    private void EnableTag(GameState currentState, bool enable) => 
        currentState.EnableTag(_gameStateEntity, _entityManager, enable);

}
