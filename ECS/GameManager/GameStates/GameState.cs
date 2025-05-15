using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public interface IGameState : IComponentData, IEnableableComponent { }

public abstract class GameState
{
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

    public static GameState[] GetAllStates => states.Values.ToArray();

    public virtual IEnumerator Enter()
    {
        yield break;
    }
    public virtual IEnumerator Exit()
    {
        yield break;
    }
    public virtual IEnumerator Update()
    {
        yield break;
    }

    public virtual void EnableTag(Entity entity, EntityManager entityManager, bool enable) =>
        throw new NotImplementedException($"{GetType().Name} must implement EnableTag to provide a valid IGameState.");

    protected void ToggleTag<T>(Entity entity, EntityManager entityManager, bool add) where T : unmanaged
    {
        if (add && !entityManager.HasComponent<T>(entity))
        {
            entityManager.AddComponent<T>(entity);
            return;
        }

        if (!add && entityManager.HasComponent<T>(entity))
            entityManager.RemoveComponent<T>(entity);   
    }
}
