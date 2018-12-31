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

        }
        public static class TransformExtensions
        {
            /// <summary>
            /// Checks whether or not the transform is in view of main camera
            /// </summary>
            /// <param name="transform"></param>
            /// <returns></returns>
            public static bool IsInView(this Transform transform)
            {
                return Essentials.Essentials.IsInView(transform.position, Camera.main);
            }
            /// <summary>
            /// Checks if transform is in view of the targeted camera
            /// </summary>
            /// <param name="transform"></param>
            /// <param name="cam"></param>
            /// <returns></returns>
            public static bool IsInView(this Transform transform, Camera cam)
            {
                return Essentials.Essentials.IsInView(transform.position, cam);
            }
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
                    var res = Essentials.Essentials.IsInView(transform.position, cam);
                    if (res)
                        camerasViewing.Add(cam);
                }
                return camerasViewing;
            }
        }
    }
}

