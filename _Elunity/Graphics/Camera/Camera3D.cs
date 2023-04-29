using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Elang
{
    public class Camera3D : MonoBehaviour
    {
        public static void ScaleLabel(GUIStyle style, int fontSize = 100) {
#if UNITY_EDITOR
            style.fontSize = Mathf.FloorToInt(fontSize / SceneView.currentDrawingSceneView.size);
#endif
        }
    }
}
