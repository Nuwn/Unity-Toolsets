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
            /// Chech whether or not a gameobject has a component.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
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
            /// Waits n time before activating the debugger. milliseconds
            /// usage : this.setTimeout((result) => { Debug.Log("debug"); }, 1000 );
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="waitTime"></param>
            /// <param name="Callback"></param>
            public static void SetTimeout(this MonoBehaviour instance, Action Callback, float waitTime) => instance.StartCoroutine(Wait((res) => Callback?.Invoke(), waitTime));
            public static void SetTimeout(this MonoBehaviour instance, Action<object> Callback, float waitTime) => instance.StartCoroutine(Wait((res) => Callback?.Invoke(true), waitTime));
            /// <summary>
            /// Continues interval with callback, use stopinterval to stop it.
            /// Calls method aftergiven time.
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
            static IEnumerator RepeatingWait(Action<bool> Callback, float waitTime)
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
                if(type == null)
                {
                    return true;
                }
                if (type.GetType() == typeof(string))
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
                {
                    return (type == null) ? true : false;
                }
            }
        }
        public static class LayerMaskExtensions
        {
            public static bool HasLayer(this LayerMask layerMask, int layer)
            {
                if (layerMask == (layerMask | (1 << layer)))
                {
                    return true;
                }
                return false;
            }

            public static bool[] HasLayers(this LayerMask layerMask)
            {
                var hasLayers = new bool[32];

                for (int i = 0; i < 32; i++)
                {
                    if (layerMask == (layerMask | (1 << i)))
                    {
                        hasLayers[i] = true;
                    }
                }
                return hasLayers;
            }
        }
        public static class ColliderExtentions
        {
            public static Vector3 RandomPointInBounds(this Collider col)
            {
                return new Vector3(
                    UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x),
                    UnityEngine.Random.Range(col.bounds.min.y, col.bounds.max.y),
                    UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z)
                );
            }
            //public static Vector3 GetPositionInBounds(this Collider col, Transform obj)
            //{
            //    return col.bounds.Contains;
            //}
        }
        public static class AudioExtentions
        {
            public static void FadeIn (this AudioSource a, MonoBehaviour instance, float to, float time, Action callback = null)
            {
                instance.StartCoroutine(Essentials.Nuwn_Essentials.LerpFloat((f) => { a.volume = f; }, 0, to, time, (v) => { callback?.Invoke(); } ));
            }
            public static void FadeOut(this AudioSource a, MonoBehaviour instance, float from, float time, Action callback = null)
            {
                instance.StartCoroutine(Essentials.Nuwn_Essentials.LerpFloat((f) => { a.volume = f; }, from, 0, time, (v) => { callback?.Invoke(); } ));
            }
        }
    }
}

