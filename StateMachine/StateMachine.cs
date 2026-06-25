using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Toolset.StateMachine
{
    public interface IState
    {
        object Owner { get; set; }

        Awaitable Enter();
        Awaitable Update();
        Awaitable Exit();
    }

    public abstract class StateMachine<T> where T : class, IState
    {
        public enum When { Before, After }
        public enum Stage { Enter, Update, Exit }

        private readonly CancellationToken _token;

        private T _currentState;
        private T _nextState;

        private Awaitable _stateLoop;
        private Awaitable _updateLoop;

        private readonly Dictionary<(When, Stage), List<Action<T>>> _callbacks = new();

        public StateMachine(CancellationToken token)
        {
            _token = token;
        }

        public void Callback(Action<T> callback, When when, Stage stage)
        {
            var key = (when, stage);

            if (!_callbacks.ContainsKey(key))
                _callbacks[key] = new List<Action<T>>();

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
            if (_stateLoop != null)
                return;

            _stateLoop = StateMachineLoop(_token);
        }

        public void Stop()
        {
            _nextState = null;
            _currentState = null;

            _stateLoop = null;
            _updateLoop = null;
        }

        private async Awaitable StateMachineLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_nextState != null)
                {
                    await TransitionState(token);
                }

                if (_currentState != null && _updateLoop == null)
                {
                    _updateLoop = RunStateUpdate(token);
                }

                await Awaitable.NextFrameAsync(token);
            }
        }

        private async Awaitable TransitionState(CancellationToken token)
        {
            StopCurrentUpdateLoop();

            if (_currentState != null)
            {
                await HandleStateStage(Stage.Exit, _currentState, token);
            }

            _currentState = _nextState;
            _nextState = null;

            if (_currentState != null)
            {
                await HandleStateStage(Stage.Enter, _currentState, token);
            }
        }

        private async Awaitable RunStateUpdate(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && _nextState == null)
                {
                    if (_currentState != null)
                    {
                        await _currentState.Update();
                    }

                    await Awaitable.NextFrameAsync(token);
                }
            }
            finally
            {
                _updateLoop = null;
            }
        }

        private void StopCurrentUpdateLoop()
        {
            _updateLoop = null;
        }

        private async Awaitable HandleStateStage(Stage stage, T state, CancellationToken token)
        {
            InvokeCallbacks(When.Before, stage, state);

            switch (stage)
            {
                case Stage.Enter:
                    await state.Enter();
                    break;

                case Stage.Exit:
                    await state.Exit();
                    break;
            }

            InvokeCallbacks(When.After, stage, state);
        }

        private void InvokeCallbacks(When when, Stage stage, T state)
        {
            var key = (when, stage);

            if (_callbacks.TryGetValue(key, out var list))
            {
                foreach (var cb in list)
                    cb.Invoke(state);
            }
        }
    }
}