using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Lazy.Events
{
    [Serializable]
    public class EventManagerSignalEmitter : Marker, INotification, INotificationOptionProvider
    {
        public string _event;
        public bool triggerOnce = false;

        public PropertyName id { get => new(); }

        NotificationFlags INotificationOptionProvider.flags => triggerOnce ? NotificationFlags.TriggerOnce : NotificationFlags.Retroactive;
    }
}