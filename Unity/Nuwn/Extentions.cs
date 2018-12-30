using UnityEngine;

namespace Nuwn
{
    namespace Extentions
    {
        public static class Extentions
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
    }
}

