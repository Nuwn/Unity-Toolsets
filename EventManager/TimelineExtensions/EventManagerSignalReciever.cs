using UnityEngine;
using UnityEngine.Playables;

namespace Lazy.Events
{
    public class EventManagerSignalReciever : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is not EventManagerSignalEmitter signal)
                return;

            // Handle the custom marker event here.
            EventManager.Invoke(signal._event);
        }
    }
}