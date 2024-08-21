using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Debug;


namespace UIToolkitUtilities
{

    public enum JoystickInteraction
    {
        None,        // No interaction or default idle state
        Pressed,     // Joystick is pressed but not moved
        Held,        // Joystick is being held down
        Moving,      // Joystick is being moved
    }

    [UxmlElement]
    public partial class OnScreenJoystick : VisualElement
    {
        [UxmlAttribute]
        public float Radius { get; set; } = 150;

        [UxmlAttribute, Range(0f, 1f)]
        public float DeadZone { get; set; } = 0.2f;

        public static readonly string styleResource = "OnScreenJoystick";
        public static readonly string ussClassName = "joystick";
        public static readonly string handleUssClassName = ussClassName + "_handle";

        private JoystickInteraction currentInteraction = JoystickInteraction.None;
        public JoystickInteraction CurrentInteraction
        {
            get => currentInteraction;
            set
            {
                currentInteraction = value;

                if (value == JoystickInteraction.None)
                    handle.transform.position = Vector3.zero;
            }
        }
        public Vector2 Input { get; set; }

        private readonly VisualElement handle;

        // Tells the editor to assign new user added content to the handle container.
        public override VisualElement contentContainer => handle;

        public OnScreenJoystick()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(styleResource));

            AddToClassList(ussClassName);

            handle = new() { name = "Handle" };
            handle.AddToClassList(handleUssClassName);
            hierarchy.Add(handle);

            generateVisualContent += context =>
            {
                style.height = Radius * 2;
                style.width = Radius * 2;
            };

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            if (evt.originPanel == null ||
                evt.originPanel.contextType != ContextType.Player)
                return;

            UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            UnregisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);

            evt.originPanel.visualTree.UnregisterCallback<PointerUpEvent>(OnRootPointerUp, TrickleDown.TrickleDown);
            evt.originPanel.visualTree.UnregisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
        }

        private void OnAttach(AttachToPanelEvent evt)
        {
            if (evt.destinationPanel == null ||
                evt.destinationPanel.contextType != ContextType.Player)
                return;

            // Local Events
            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            RegisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);

            // UIDoc root events
            evt.destinationPanel.visualTree.RegisterCallback<PointerUpEvent>(OnRootPointerUp, TrickleDown.TrickleDown);
            evt.destinationPanel.visualTree.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
        }

        private void OnRootPointerUp(PointerUpEvent evt) =>
            CurrentInteraction = JoystickInteraction.None;
        private void OnPointerCancel(PointerCancelEvent evt) =>
            CurrentInteraction = JoystickInteraction.None;
        private void OnPointerDown(PointerDownEvent evt) =>
            CurrentInteraction = JoystickInteraction.Pressed;

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (currentInteraction == JoystickInteraction.None) return;

            Vector3 center = this.LocalToWorld(contentRect.center);

            var direction = evt.position - center;

            var distance = direction.magnitude;

            float clampedDistance = Mathf.Min(distance, Radius);

            Vector3 clampedPosition = direction.normalized * clampedDistance;

            handle.transform.position = clampedPosition;


            if (distance < DeadZone * Radius)
            {
                // Input is within the dead zone, treat it as zero
                currentInteraction = JoystickInteraction.Held;
                Input = Vector2.zero;
                return;
            }

            Input = new Vector2(
                Mathf.Clamp(direction.x / Radius, -1f, 1f),
                Mathf.Clamp(direction.y / Radius, -1f, 1f)
            );

            currentInteraction = Input == Vector2.zero ?
                JoystickInteraction.Held :
                JoystickInteraction.Moving;
        }

    }

}