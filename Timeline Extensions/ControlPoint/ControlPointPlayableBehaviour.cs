using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Lazy.Timeline
{
    [Serializable]
    public class ControlPointPlayableBehaviour : PlayableBehaviour
    {
        /*
         * If TriggerName is empty, we will use the control track without a trigger
         * but with it's wrap functionality
         */
        public string triggerName = string.Empty;
        public DirectorWrapMode wrapMode = DirectorWrapMode.None;

        private PlayableDirector director;

        private bool initialized = false;
        private double defaultDuration;
        private Playable rootPlayable;
        private double clipStartTime, clipEndTime;

        public override void OnPlayableCreate(Playable playable)
        {
            if (!Application.isPlaying) return; // Only run in Play mode

            if (!InitializeDirector(playable)) return;

            rootPlayable = playable.GetGraph().GetRootPlayable(0);
            InitializeClip();
            RegisterTriggerIfNeeded();

            initialized = true;
        }

        private bool InitializeDirector(Playable playable)
        {
            director = playable.GetGraph().GetResolver() as PlayableDirector;

            if (director == null) return false;

            defaultDuration = director.duration; // Handle potential null case for director

            return true;
        }

        private void InitializeClip()
        {
            var clip = director.GetClip(IsControlPointClip);

            if (clip == null) return;

            clipStartTime = clip.start;
            clipEndTime = clip.end;
        }

        private bool IsControlPointClip(TimelineClip clip) =>
            clip.asset is ControlPointClip controlPointClip &&
            controlPointClip.settings.triggerName == triggerName;

        private void RegisterTriggerIfNeeded()
        {
            if (!string.IsNullOrEmpty(triggerName))
                director.RegisterTrigger(triggerName, OnTriggerCalled);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
                rootPlayable.SetDuration(clipEndTime);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying ||
                !initialized ||
                playable.IsNull() ||
                info.evaluationType == FrameData.EvaluationType.Evaluate) return;


            switch (wrapMode)
            {
                case DirectorWrapMode.Loop:
                    ReturnToClipStart();
                    break;
                case DirectorWrapMode.None:
                    rootPlayable.SetDuration(defaultDuration);
                    break;
                default:
                    break;
            }
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying || director == null || triggerName == null) return;

            director.DeregisterTrigger(triggerName);
        }

        void OnTriggerCalled() => ReturnToClipStart();

        void ReturnToClipStart()
        {
            if (director == null) return;

            director.time = clipStartTime;
            director.Evaluate();
        }
    }
}