using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class BaseState : IState
{
    protected static Dictionary<Type, IState> states = new();

    public object Owner { get; set; }

    public static T GetOrCreate<T>(object owner) where T : BaseState, new()
    {
        states.TryGetValue(typeof(T), out var state);

        if (state is not T instance)
        {
            instance = new T
            {
                Owner = owner
            };

            states[typeof(T)] = instance;
            Debug.Log($"State {typeof(T).Name} created.");
        }

        return instance;
    }

    public abstract IEnumerator Enter();
    public abstract IEnumerator Exit();
    public abstract IEnumerator Update();

    public static IState[] GetAllStates => states.Values.ToArray();

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void SetupPlayModeStateChangeHandler()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            states.Clear();
        }
    }
#endif
}