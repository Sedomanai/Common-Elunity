using System;
using UnityEngine;

namespace Elang
{
    /// <summary>
    /// Extension methods commonly used anywhere. 
    /// </summary>
    public static class CommonExtensionMethods
    {
        public static T AddComponentIfNone<T>(this GameObject obj, Action<T> onAdd) where T : Component {
            T comp = obj.GetComponent<T>();
            if (!comp) { 
                comp = obj.AddComponent<T>();
                onAdd(comp);
            }
            return comp;
        }
        public static T AddComponentIfNone<T>(this GameObject obj) where T : Component {
            T comp = obj.GetComponent<T>();
            if (!comp)
                comp = obj.AddComponent<T>();
            return comp;
        }
    }
}