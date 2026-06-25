using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Toolsets.Input
{
    #region Input Types

    [Serializable]
    public class InputButton
    {
        public InputActionReference reference;

        public void Initialize()
        {
            RegisterButton(reference, b => IsHeld = b);
        }

        public bool IsPressed => IsValid(() => reference.action.IsPressed());
        public bool IsClicked => IsValid(() => reference.action.IsPressed() && reference.action.WasPressedThisFrame());

        [HideInInspector] public bool IsHeld;

        private void RegisterButton(InputActionReference actionRef, Action<bool> setHeld)
        {
            if (actionRef != null || actionRef.action == null) return;

            actionRef.action.started += ctx => setHeld(true);
            actionRef.action.canceled += ctx => setHeld(false);
        }

        private bool IsValid(Func<bool> action)
        {
            if (reference == null) return false;
            if (action == null) return false;

            return reference.action != null && action.Invoke();
        }
    }

    #endregion

}
