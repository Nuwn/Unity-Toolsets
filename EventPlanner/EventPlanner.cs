using System.Collections.Generic;
using UnityEngine;

namespace LazyEvents
{
    /// <summary>
    /// <example>
    ///    This shows how to use it.
    /// <code>
    ///     EventPlanner.Invoke("OnEventHappened", true);
    ///    
    ///     EventPlanner.AddListner("OnEventHappened", OnEventHappened);
    ///     EventPlanner.RemoveListner("OnEventHappened", OnEventHappened);
    ///     
    ///     public void OnEventHappened(object data){
    ///         var res = (bool) data;
    ///     }
    /// </code>
    /// </example>
    /// </summary>
    public static class EventPlanner
    {
        public delegate void EventDelegate(object data);
        private static readonly Dictionary<string, EventDelegate> keyValuePairs = new Dictionary<string, EventDelegate>();

        /// <summary>
        /// <para>EventPlanner.Invoke("OnMouseTrigger", AnyData);</para>
        /// <para>See class for full example</para>
        /// </summary>
        /// <param name="key">string: the identifier</param>
        /// <param name="listner">the data to send, can be any type of object</param>
        public static bool Invoke(string key, object data = null)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key]?.Invoke(data);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <para>EventPlanner.AddListner("OnMouseTrigger", OnMouseTrigger);</para>
        /// <para>See class for full example</para>
        /// </summary>
        /// <param name="key">string: the identifier</param>
        /// <param name="listner">method: the method to call</param>
        public static void AddListner(string key, EventDelegate listner)
        {
            if (!keyValuePairs.ContainsKey(key))
            {
                keyValuePairs.Add(key, listner);
            }
            else
            {
                EventDelegate del = keyValuePairs[key];
                del += listner;
                keyValuePairs[key] = del;
            }
        }
        /// <summary>
        /// <para>EventPlanner.RemoveListner("OnMouseTrigger", OnMouseTrigger);</para>
        /// <para>See class for full example</para>
        /// </summary>
        /// <param name="key">string: the identifier</param>
        /// <param name="listner">method: the method to call</param>
        public static void RemoveListner(string key, EventDelegate listner)
        {
            if (keyValuePairs.TryGetValue(key, out EventDelegate del))
            {
                del -= listner;
                keyValuePairs[key] = del;

                if (del == null || del.GetInvocationList().Length == 0)
                    keyValuePairs.Remove(key);
            }
        }
        /// <summary>
        /// Used if you wish to clear all the events.
        /// </summary>
        public static void Clear()
        {
            keyValuePairs.Clear();
        }

    }
}