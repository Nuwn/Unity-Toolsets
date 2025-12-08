using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UIDocument))]
public class UISectionsManager : MonoBehaviour
{
    [Header("Make sure each section has a unique name in UXML")]
    [SerializeField] private UIDocument document;

    [SerializeField] private string rootClassName = "root-element";

    [SerializeField] internal List<Section> Sections = new();

    private readonly Dictionary<string, VisualElement> runtimeLookup = new();

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        StartCoroutine(DelayedRuntimeBind());
    }

    private IEnumerator DelayedRuntimeBind()
    {
        yield return null; // wait one frame for nested UIDocuments

        var root = document.rootVisualElement;
        if (root == null) yield break;

        runtimeLookup.Clear();

        var foundSections = root.Query<VisualElement>(className: rootClassName).ToList();

        foreach (var element in foundSections)
        {
            if (!string.IsNullOrEmpty(element.name))
                runtimeLookup[element.name] = element;
        }

        foreach (var section in Sections)
        {
            if (string.IsNullOrEmpty(section.sectionName)) continue;

            if (!runtimeLookup.TryGetValue(section.sectionName, out var element))
                continue;

            section.sectionElement = element;

            if (section.visible)
                section.Show();
            else
                section.Hide();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        document = GetComponent<UIDocument>();
        if (document == null) return;

        // Delay the scan so nested UIDocuments are present
        EditorApplication.delayCall += DelayedEditorScan;
    }
#endif

#if UNITY_EDITOR
    private void DelayedEditorScan()
    {
        if (this == null) return;
        if (document == null) return;

        var root = document.rootVisualElement;
        if (root == null) return;

        var foundSections = root.Query<VisualElement>(className: rootClassName).ToList();

        // Remove missing
        Sections.RemoveAll(s =>
            s == null ||
            string.IsNullOrEmpty(s.sectionName) ||
            !foundSections.Any(e => e.name == s.sectionName));

        // Add new
        foreach (var element in foundSections)
        {
            if (string.IsNullOrEmpty(element.name)) continue;

            if (!Sections.Any(s => s.sectionName == element.name))
            {
                Sections.Add(new Section
                {
                    sectionName = element.name,
                    visible = true
                });
            }
        }

        EditorUtility.SetDirty(this);
    }
#endif

    public void ShowSection(string name)
    {
        var section = Sections.FirstOrDefault(s => s.sectionName == name);
        section?.Show();
    }

    public void HideSection(string name)
    {
        var section = Sections.FirstOrDefault(s => s.sectionName == name);
        section?.Hide();
    }

    [Serializable]
    public class Section
    {
        public string sectionName;
        public bool visible = true;

        [NonSerialized] public VisualElement sectionElement;

        public void Show()
        {
            visible = true;
            if (sectionElement != null)
                sectionElement.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            visible = false;
            if (sectionElement != null)
                sectionElement.style.display = DisplayStyle.None;
        }
    }
}
