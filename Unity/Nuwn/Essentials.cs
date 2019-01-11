using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace Nuwn
{ 
    namespace Essentials
    {
        public class Nuwn_Essentials : MonoBehaviour
        {
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
    }   
}
