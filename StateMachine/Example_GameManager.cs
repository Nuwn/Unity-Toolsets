using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Toolset.StateMachine
{
    public class Example_GameManager : Singleton<Example_GameManager>
    {
        GameStateMachine gameStateMachine;

        protected override void Awake()
        {
            base.Awake();
            gameStateMachine = new(this.destroyCancellationToken);
        }

        void Start()
        {
            gameStateMachine.Start();
            gameStateMachine.SetNextState(new PlayState());
        }

        public class GameStateMachine : StateMachine<GameState>
        {
            public GameStateMachine(CancellationToken token) : base(token)
            {
            }
        }

    }

    public abstract class GameState : IState {

        public object Owner { get; set; }

        public virtual Awaitable Enter(){ return null; }

        public virtual Awaitable Update(){ return null; }

        public virtual Awaitable Exit(){ return null; }

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
}