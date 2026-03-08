using UnityEngine;
using UnityEngine.UIElements;
using System; // for the static event

[RequireComponent(typeof(UIDocument))]
public class UIRefocus : MonoBehaviour
{
    private UIDocument uIDocument;

    public static event Action OnRefocus;
    public static void Refocus() => OnRefocus?.Invoke();

    private void Awake()
    {
        uIDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        if (root == null) return;

        // Register for automatic refocus on UI changes
        root.RegisterCallback<AttachToPanelEvent>(OnAttached);
        root.RegisterCallback<GeometryChangedEvent>(OnHierarchyChanged);

        // NEW: Listen to the static event (only while this component is enabled)
        OnRefocus += this._Refocus;

        // When the script becomes enabled → auto-focus the first selectable element
        _Refocus();
    }

    private void OnDisable()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        if (root == null) return;

        // Clean up callbacks
        root.UnregisterCallback<AttachToPanelEvent>(OnAttached);
        root.UnregisterCallback<GeometryChangedEvent>(OnHierarchyChanged);

        // NEW: Stop listening to the static event
        OnRefocus -= this._Refocus;

        // Remove ALL focus when disabled (your keyboard/gamepad control takes over)
        Unfocus();
    }

    private void OnAttached(AttachToPanelEvent evt)
    {
        _Refocus();
    }

    private void OnHierarchyChanged(GeometryChangedEvent evt)
    {
        _Refocus();
    }

    private void _Refocus()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        if (root == null) return;

        // Schedule for next frame so everything is fully updated
        root.schedule.Execute(PerformRefocus).StartingIn(0);
    }

    private void PerformRefocus(TimerState _)
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        if (root == null) return;

        var firstFocusable = root.Query<VisualElement>()
            .Where(e =>
                e.focusable &&
                e.enabledInHierarchy &&
                IsEffectivelyVisible(e))
            .First();

        firstFocusable?.Focus();
    }

    /// <summary>
    /// Returns true only if the element AND ALL its ancestors have display != none and visibility == Visible.
    /// This is the reliable way to detect "actually visible on screen" in UI Toolkit.
    /// </summary>
    private bool IsEffectivelyVisible(VisualElement element)
    {
        VisualElement current = element;
        while (current != null)
        {
            var style = current.resolvedStyle;
            if (style.display == DisplayStyle.None)
                return false;

            current = current.parent;
        }
        return true;
    }

    private void Unfocus()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        if (root == null) return;

        // Official way to clear focus in UI Toolkit
        root.focusController?.focusedElement?.Blur();
    }
}