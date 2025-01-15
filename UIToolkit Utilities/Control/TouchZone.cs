using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Debug;

namespace UIToolkitUtilities
{
    public enum TouchInteraction
    {
        None,
        Pressed,
        Held,
        Moving,
    }

    [UxmlElement]
    public partial class TouchZone : VisualElement
    {
        public event Action<Vector2> OnClick;

        private float tapThreshold = 0.2f;
        private float pointerDownTime = Time.time;

        // used for onClick to tell the user where it was clicked. should it be on release?
        private Vector2 lastPointerPosition = Vector2.zero;

        public event Action<TouchInteraction> OnInteractionChange;

        private TouchInteraction currentInteraction = TouchInteraction.None;
        private TouchInteraction CurrentInteraction
        {
            get => currentInteraction;
            set
            {
                if (currentInteraction == value) return;

                currentInteraction = value;

                if (currentInteraction != TouchInteraction.None)
                {
                    pointerDownTime = Time.time;
                }
                else
                {
                    delta = Vector2.zero;

                    // invoke clicked if pointer released in time
                    if (Time.time - pointerDownTime <= tapThreshold)
                        OnClick?.Invoke(lastPointerPosition);
                }

                OnInteractionChange?.Invoke(value);
            }
        }

        // which button or finger interacted, useful for multi touch
        private int pointerIdInteracted;

        Vector2 delta = Vector2.zero;
        public Vector2 GetDelta
        {
            get
            {
                var temp = delta;
                delta = Vector2.zero;
                return temp;
            }
        }

        public TouchZone()
        {

            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }
        private void OnAttach(AttachToPanelEvent evt)
        {
            if (evt.destinationPanel == null ||
                evt.destinationPanel.contextType != ContextType.Player)
                return;

            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            evt.destinationPanel.visualTree.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            if (evt.originPanel == null ||
                evt.originPanel.contextType != ContextType.Player)
                return;

            UnregisterCallback<PointerDownEvent>(OnPointerDown);
            UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            evt.originPanel.visualTree.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.pointerId != pointerIdInteracted) return;

            lastPointerPosition = ConvertToScreenPosition(evt.localPosition);
            CurrentInteraction = TouchInteraction.None;
        }

        private Vector2 ConvertToScreenPosition(Vector2 position)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float screenX = (position.x / resolvedStyle.width) * screenWidth;
            float screenY = (position.y / resolvedStyle.height) * screenHeight;

            screenY = screenHeight - screenY;

            return new Vector2(screenX, screenY);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            pointerIdInteracted = evt.pointerId;
            CurrentInteraction = TouchInteraction.Pressed;
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (CurrentInteraction == TouchInteraction.None) return;

            if (evt.pointerId != pointerIdInteracted) return;

            delta = evt.deltaPosition;

            // Do we need deadzone?
            CurrentInteraction = (delta.magnitude > 0) ? 
                TouchInteraction.Moving : 
                TouchInteraction.Held;
            
        }



    }

}