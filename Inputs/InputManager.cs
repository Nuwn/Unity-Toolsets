using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Toolsets.Input
{
    #region Active Device

    public enum ActiveDeviceType { None, KeyboardMouse, Gamepad, Touch }

    [Serializable]
    public class ActiveDevice
    {
        public event Action<ActiveDeviceType> OnActiveDeviceChanged;

        private ActiveDeviceType m_value;

        public ActiveDeviceType Value
        {
            get => m_value;
            set
            {
                if (m_value == value) return;
                m_value = value;
                OnActiveDeviceChanged?.Invoke(m_value);
            }
        }

        public ActiveDevice(ActiveDeviceType activeDevice = ActiveDeviceType.KeyboardMouse) =>
            Value = activeDevice;
    }

    #endregion

    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] internal InputActionAsset inputActions;

        [SerializeField] internal List<InputButton> buttons = new();
        [SerializeField] internal List<InputFloat> floatValues = new();
        [SerializeField] internal List<InputVector2> vector2Values = new();

        #region Active Device

        public ActiveDevice activeDevice = new();

        public bool IsKeyboardMouse => activeDevice.Value == ActiveDeviceType.KeyboardMouse;
        public bool IsGamepad => activeDevice.Value == ActiveDeviceType.Gamepad;
        public bool IsTouch => activeDevice.Value == ActiveDeviceType.Touch;

        #endregion

        #region Getters

        public InputButton GetButton(string actionName) => buttons.Find(b =>
            b.reference?.action != null &&
            string.Equals(b.reference.action.name, actionName, StringComparison.OrdinalIgnoreCase));

        public InputFloat GetFloat(string actionName) => floatValues.Find(v =>
            v.reference?.action != null &&
            string.Equals(v.reference.action.name, actionName, StringComparison.OrdinalIgnoreCase));

        public InputVector2 GetVector2(string actionName) => vector2Values.Find(v =>
            v.reference?.action != null &&
            string.Equals(v.reference.action.name, actionName, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region Events

        protected override void Awake()
        {
            base.Awake();

            if (inputActions != null)
                inputActions.Enable();

            foreach (var action in buttons)
                action.Initialize();

            SceneManager.sceneLoaded += (scene, mode) => OnSceneOpened();

            InputSystem.onActionChange += OnActionChange;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.sceneLoaded -= (scene, mode) => OnSceneOpened();

            InputSystem.onActionChange -= OnActionChange;
        }

        private void OnSceneOpened()
        {
            if (inputActions != null)
                inputActions.Enable();
        }

        private void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;
            if (obj is not InputAction action) return;

            InputDevice device = action.activeControl?.device;
            if (device == null) return;

            activeDevice.Value = device switch
            {
                Keyboard or Mouse => ActiveDeviceType.KeyboardMouse,
                Gamepad => ActiveDeviceType.Gamepad,
                Touchscreen => ActiveDeviceType.Touch,
                _ => ActiveDeviceType.None,
            };
        }

        #endregion

        #region Utility

        public void SetPlayerInputsEnabled(bool enabled)
        {
            if (inputActions == null) return;

            var playerMap = inputActions.FindActionMap("Player");

            if (playerMap == null) return;

            if (enabled)
                playerMap.Enable();
            else
                playerMap.Disable();
        }

        #endregion
    }


}
