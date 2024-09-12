using Lazy.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(ControlPointClip))]
public class ControlPointClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        if (clip.asset is not ControlPointClip _clip) return;

        clip.displayName = $"{_clip.settings.triggerName}; {_clip.settings.wrapMode}" ;
    }
}