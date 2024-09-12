using UnityEngine.Timeline;

namespace Lazy.Timeline
{
    [TrackColor(0.8f, 0.1f, 0.1f)] // Set the color of the track in the Timeline window
    [TrackClipType(typeof(ControlPointClip))] // Define the type of clips this track can contain
    public class ControlPointTrack : TrackAsset { }
}