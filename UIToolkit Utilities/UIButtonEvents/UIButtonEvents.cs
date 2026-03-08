using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UIToolkitUtilities
{
    [RequireComponent(typeof(UIDocument))]
    public class UIButtonEvent : MonoBehaviour
    {
        private const string IgnoreClassName = "ui-button-event-ignore";

        [SerializeField] private UIDocument document;

        public List<ButtonRef> Buttons = new();

        private readonly Dictionary<Button, Action> _registered = new();

        private void OnEnable()
        {
            document = GetComponent<UIDocument>();
            if (document == null || document.rootVisualElement == null) return;

            var buttons = document.rootVisualElement.Query<Button>().ToList();
            if (buttons.Count == 0) return;

            // Fast lookup map
            var map = new Dictionary<string, UnityEvent>(StringComparer.Ordinal);
            foreach (var entry in Buttons)
            {
                if (entry == null || string.IsNullOrEmpty(entry.buttonName)) continue;
                if (entry.onClick == null) entry.onClick = new UnityEvent();
                map[entry.buttonName] = entry.onClick;
            }

            foreach (var button in buttons)
            {
                if (button == null) continue;
                if (button.ClassListContains(IgnoreClassName)) continue;
                if (string.IsNullOrEmpty(button.name)) continue;

                if (!map.TryGetValue(button.name, out var unityEvent) || unityEvent == null) continue;

                // Prevent double registration
                if (_registered.ContainsKey(button)) continue;

                Action callback = () => unityEvent.Invoke();

                button.clicked += callback;
                _registered[button] = callback;
            }
        }

        private void OnDisable()
        {
            if (document == null || document.rootVisualElement == null)
            {
                _registered.Clear();
                return;
            }

            foreach (var kv in _registered)
            {
                if (kv.Key != null)
                    kv.Key.clicked -= kv.Value;
            }

            _registered.Clear();
        }


        [Serializable]
        public class ButtonRef
        {
            public string buttonName;
            public UnityEvent onClick;

            public ButtonRef(string button)
            {
                buttonName = button;
                onClick = new UnityEvent();
            }
        }
    }
}