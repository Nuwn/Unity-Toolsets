using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Toolsets.Input
{

    [Serializable] public class InputFloat : InputValue<float> { }
    [Serializable] public class InputVector2 : InputValue<Vector2> { }


    [Serializable]
    public abstract class InputValue<T> where T : unmanaged
    {
        public InputActionReference reference;

        public T Value => IsValid(() => reference.action.ReadValue<T>());
        public bool Performed => IsValid(() => reference.action.WasPerformedThisFrame());

        private T IsValid(Func<T> action)
        {
            if (reference?.action == null)
                return default;

            return action.Invoke();
        }

        private bool IsValid(Func<bool> action)
        {
            return reference?.action != null && action.Invoke();
        }
    }

}
