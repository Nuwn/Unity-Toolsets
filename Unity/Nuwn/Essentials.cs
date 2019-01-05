using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace Nuwn
{ 
    namespace Essentials
    {
        public class Essentials : MonoBehaviour
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
            /// <param name="itteration">-1 or 1</param>
            /// <returns></returns>
            public static int NextPrev(object[] array, object current, int itteration)
            {
                if (current == null)
                    throw new ArgumentNullException("Value for current not set");

                if (itteration != 1 && itteration != -1)
                    throw new ArgumentNullException("Wrong Itteration");

                var length = array.Length;
                var currentIndex = Array.IndexOf(array, current);

                if (currentIndex == -1)
                    throw new ArgumentNullException("Object does not exist in array");
                
                int res = 0;

                if (itteration == 1)
                {
                    if (currentIndex == length - 1)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = currentIndex + 1;
                    }
                }
                else if (itteration == -1)
                {
                    if (currentIndex == 0)
                    {
                        res = length - 1;
                    }
                    else
                    {
                        res = currentIndex - 1;
                    }
                }
                return res;
            }      
        }
        public class Instanciating : MonoBehaviour
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

                T yourObject = (child) ? newObject.GetComponentInChildren<T>() : newObject.GetComponent<T>(); ;
                
                return yourObject;
            }
        }
        public class Colliders : MonoBehaviour
        {
            /// <summary>
            /// Set and ignore on colliders 1 and 2.
            /// </summary>
            /// <param name="col1">First Collider</param>
            /// <param name="col2">Second Collider</param>
            /// <param name="ignore">Set false to remove the ignore</param>
            public static void IgnoreCollision(Collider col1, Collider col2, bool ignore)
            {
                Physics.IgnoreCollision(col1, col2, ignore);
            }
        }  
    }
}