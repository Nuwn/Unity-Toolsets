using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nuwn
{
    namespace Extensions
    {
        public static class Extensions
        {
            /// <summary>
            /// How to use: transform.position = transform.SetPosition()
            /// just to show how to build extentions
            /// </summary>
            /// <param name="trans"></param>
            /// <returns></returns>
            public static Vector3 SetPosition(this Transform trans)
            {
                return trans.position = Vector3.zero;
            }
            public static bool HasComponent<T>(this GameObject obj) where T : Component
            {
                return obj.GetComponent<T>() != null;
            } 
        }
        public static class TransformExtensions
        {
            /// <summary>
            /// Checks whether or not the transform is in view of main camera
            /// </summary>
            /// <param name="transform"></param>
            /// <returns></returns>
            public static bool IsInView(this Transform transform) => Essentials.Nuwn_Essentials.IsInView(transform.position, Camera.main);
            /// <summary>
            /// Checks if transform is in view of the targeted camera
            /// </summary>
            /// <param name="transform"></param>
            /// <param name="cam"></param>
            /// <returns></returns>
            public static bool IsInView(this Transform transform, Camera cam) => Essentials.Nuwn_Essentials.IsInView(transform.position, cam);
            /// <summary>
            /// returns which camera is viewing the target, 
            /// asking for a list of all cameras you wish too look up,
            /// so it dont have to search for it and that's slow to do.
            /// </summary>
            /// <param name="transform"></param>
            /// <param name="cameras"></param>
            /// <returns></returns>
            public static List<Camera> IsInView(this Transform transform, List<Camera> cameras)
            {
                List<Camera> camerasViewing = new List<Camera>();

                foreach (var cam in cameras)
                {
                    var res = Essentials.Nuwn_Essentials.IsInView(transform.position, cam);
                    if (res)
                        camerasViewing.Add(cam);
                }
                return camerasViewing;
            }
        }
        public static class MonoBehaviourExtentions
        {
            /// <summary>
            /// Waits n time before activating the debugger.
            /// usage : this.setTimeout(1, (result) => { Debug.Log("debug"); } );
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="waitTime"></param>
            /// <param name="Callback"></param>
            public static void SetTimeout(this MonoBehaviour instance, Action Callback, float waitTime) => instance.StartCoroutine(Wait((res) => Callback?.Invoke(), waitTime));
            public static void SetTimeout(this MonoBehaviour instance, Action<object> Callback, float waitTime) => instance.StartCoroutine(Wait((res) => Callback?.Invoke(true), waitTime));
            /// <summary>
            /// Continues interval with callback, use stopinterval to stop it.
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="Callback"></param>
            /// <param name="intervalTime"></param>
            /// <returns></returns>
            public static Coroutine SetInterval(this MonoBehaviour instance, Action<object> Callback, float intervalTime) => instance.StartCoroutine(RepeatingWait((res) => Callback?.Invoke(true), intervalTime));
            public static Coroutine SetInterval(this MonoBehaviour instance, Action Callback, float intervalTime) => instance.StartCoroutine(RepeatingWait((res) => Callback?.Invoke(), intervalTime));
            /// <summary>
            /// 
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="coroutine">The interval to stop, store it as a var</param>
            public static void StopInterval(this MonoBehaviour instance, Coroutine coroutine) => instance.StopCoroutine(coroutine);




            #region Internal functions
            static IEnumerator Wait(Action<bool> Callback, float duration)
            {
                yield return new WaitForSeconds(duration / 1000);
                Callback.Invoke(true);
            }
            static IEnumerator RepeatingWait( Action<bool> Callback, float waitTime)
            {
                while (true)
                {
                    yield return new WaitForSeconds(waitTime / 1000);
                    Callback.Invoke(true);
                }
            }
            #endregion
        }
        public static class PHP
        {
            public static bool Empty<T>(this T type, out object value)
            {
                value = type;
                return Empty(type);
            }
            public static bool Empty<T>(this T type)
            {

                if (type == null)
                {
                    return true;
                }
                else if (type.GetType() == typeof(string))
                {
                    string data = (string)(object)type;
                    return (data == "" || data == "0") ? true : false;
                }
                else if (type.GetType() == typeof(int))
                {
                    int data = (int)(object)type;
                    return (data == 0) ? true : false;
                }
                else if (type.GetType() == typeof(float))
                {
                    float data = (float)(object)type;
                    return (data == 0.0) ? true : false;
                }
                else if (type.GetType() == typeof(bool))
                {
                    bool data = (bool)(object)type;
                    return !data;
                }
                else if (type.GetType().IsArray || type.GetType().IsGenericType)
                {
                    ICollection data = (object)type as ICollection;
                    return (data.Count == 0) ? true : false;
                }
                else
                    throw new ArgumentOutOfRangeException("This Object is not implemented for this type.");
            }
        }
        public static class JS
        {
          
        }
    }
}

