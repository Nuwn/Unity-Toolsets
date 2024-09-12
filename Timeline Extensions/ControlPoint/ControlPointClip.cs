using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Lazy.Timeline
{
    public class ControlPointClip : PlayableAsset, ITimelineClipAsset
    {
        public ControlPointPlayableBehaviour settings;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) =>
            ScriptPlayable<ControlPointPlayableBehaviour>.Create(graph, settings);
    }
}