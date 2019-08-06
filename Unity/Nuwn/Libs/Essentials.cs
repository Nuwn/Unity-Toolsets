using Nuwn.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Nuwn
{ 
    namespace Essentials
    {
        public class Nuwn_Essentials : MonoBehaviour
        {
            /// <summary>
            /// async loads scene
            /// </summary>
            /// <param name="newScene"></param>
            /// <param name="oldScene"> ignore this to disable unload</param>
            /// <returns></returns>
            public static IEnumerator LoadNewScene(int newScene, int oldScene = -1)
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(newScene);

                while (!async.isDone)
                {
                    yield return null;
                }

                if (oldScene != -1)
                {
                    SceneManager.UnloadSceneAsync(oldScene);
                }
            }



            /// <summary>
            /// Checks whether the target position is in screen view
            /// </summary>
            /// <param name="Position">The target position like target.position</param>
            /// <param name="cam">Camera...</param>
            /// <returns>True is in view</returns>
            public static bool IsInView(Vector3 Position, Camera cam)
            {
                Vector3 newPos = cam.WorldToViewportPoint(Position);
                //Simple check if the target object is out of the screen or inside
                return (newPos.x > 1 || newPos.y > 1 || newPos.x < 0 || newPos.y < 0) ? false : true;
            }
            /// <summary>
            /// Array randomizer, insert ex.
            /// Randomizer(0, array.length)
            /// </summary>
            /// <param name="start"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static int Randomizer(int start, int length) => UnityEngine.Random.Range(0, length - 1);
            public static bool RandomPercentage(float percent)
            {
                System.Random random = new System.Random();
                double rand = random.NextDouble();
                if (rand < (percent / 100))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Array itterator, 
            /// if you want to itterate more then once put it in a loop
            /// for (int i = 0; i "less" n; i++)
            ///     itteration + 1
            /// </summary>
            /// <param name="array"></param>
            /// <param name="current"></param>
            /// <param name="forward">move forwards? or backwards</param>
            /// <returns></returns>
            public static int NextPrev(object[] array, object current, bool forward)
            {
                if (current == null) throw new ArgumentNullException("Value for current not set");

                var length = array.Length;
                var currentIndex = Array.IndexOf(array, current);

                if (currentIndex == -1) throw new ArgumentNullException("Object does not exist in array");
                
                if (forward)
                    return (currentIndex == length - 1) ? 0 : currentIndex + 1;
                else
                    return (currentIndex == 0) ? length - 1 : currentIndex - 1;              
            }
            public static IEnumerator LerpFloat(Action<float> @return, float from, float to, float duration = 2, Action<bool> Callback = null)
            {
                var i = 0f;
                var rate = 1f / duration;

                while (i < 1f)
                {
                    i += Time.deltaTime * rate;
                    @return(Mathf.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, i)));
                    yield return null;
                }
                @return(to);
                Callback?.Invoke(true);
            }
            public static IEnumerator LerpVector3(Action<Vector3> @return, Vector3 from, Vector3 to, float duration = 2, Action<bool> Callback = null)
            {
                var i = 0f;
                var rate = 1f / duration;

                while (i < 1f)
                {
                    i += Time.deltaTime * rate;
                    @return(Vector3.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, i)));
                    yield return null;
                }
                @return(to);
                Callback?.Invoke(true);
            }
            public static IEnumerator LerpQuaternion(Action<Quaternion> @return, Quaternion from, Quaternion to, float duration = 2, Action<bool> Callback = null)
            {
                var i = 0f;
                var rate = 1f / duration;

                while (i < 1f)
                {
                    i += Time.deltaTime * rate;
                    @return(Quaternion.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, i)));
                    yield return null;
                }
                @return(to);
                Callback?.Invoke(true);
            }

            /// <summary>
            /// Checks if the number is between 2 values;
            /// </summary>
            /// <param name="num"></param>
            /// <param name="lower"></param>
            /// <param name="upper"></param>
            /// <param name="inclusive"></param>
            /// <returns></returns>
            public static bool Between(float num, float lower, float upper, bool inclusive = false)
            {
                return inclusive
                    ? lower <= num && num <= upper
                    : lower < num && num < upper;
            }
        }
        public class Nuwn_Instanciating : MonoBehaviour
        {
            /// <summary>
            /// Instanciating a object and returns a targeted component, 
            /// can also search the component in a child
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="prefab"></param>
            /// <param name="startPos">Start Position</param>
            /// <param name="startRot">Start Rotation</param>
            /// <param name="parent">Ignore or set as null to not have a parent</param>
            /// <param name="child">True if you want to search thru childs</param>
            /// <returns></returns>
            public static T Create<T>(GameObject prefab, Vector3 startPos, Quaternion startRot, Transform parent = null, bool child = false)
            {
                GameObject newObject = Instantiate(prefab, startPos, startRot, parent) as GameObject;

                return (child) ? newObject.GetComponentInChildren<T>() : newObject.GetComponent<T>();
            }
        }
        public class Nuwn_Colliders : MonoBehaviour
        {
            /// <summary>
            /// Set and ignore on colliders 1 and 2.
            /// </summary>
            /// <param name="col1">First Collider</param>
            /// <param name="col2">Second Collider</param>
            /// <param name="ignore">Set false to remove the ignore</param>
            public static void IgnoreCollision(Collider coll1, Collider coll2, bool ignore) => Physics.IgnoreCollision(coll1, coll2, ignore);
            public static void MultiIgnoreCollision(Collider coll1, List<Collider> CollList, bool ignore)
            {
                foreach (var col in CollList)
                {
                    IgnoreCollision(coll1, col, ignore);
                }
            }
        }
        public struct RangedFloat
        {
            public float minValue;
            public float maxValue;
        }
        public static class Nuwn_Statics
        {
            public static bool HasMethod(this object objectToCheck, string methodName)
            {
                var type = objectToCheck.GetType();
                return type.GetMethod(methodName) != null;
            }

        }

        [Serializable] public class UnityEventGameObject : UnityEvent<GameObject> { }
        [Serializable] public class UnityEventCollider2D : UnityEvent<Collider2D> { }
        [Serializable] public class UnityEventCollider : UnityEvent<Collider> { }
    }   
}
