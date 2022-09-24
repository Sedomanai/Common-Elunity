#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Elang
{
    [CustomEditor(typeof(AtlasAsset))]
    public class AtlasAssetEditor : Editor
    {
        private AtlasAsset atlas { get { return target as AtlasAsset; } }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUI.EndDisabledGroup(); // why?..
            EditorGUI.BeginChangeCheck();

            atlas.makeClipAnimations = EditorGUILayout.Toggle("Make Clips", atlas.makeClipAnimations);
            atlas.makeController = EditorGUILayout.Toggle("Make Controller", atlas.makeController);
            InspectTextures();

            if (GUILayout.Button("GENERATE!")) {
                atlas.Generate();
                EditorUtility.SetDirty(atlas);
                serializedObject.ApplyModifiedProperties();
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(atlas);
                serializedObject.ApplyModifiedProperties();
            }
        }

        void InspectTextures() {
            var prev = atlas.textures.Count;
            var curr = EditorGUILayout.DelayedIntField("User Count", prev);
            if (curr != prev) {
                if (curr < prev) {
                    bool trim = EditorUtility.DisplayDialog("Atlas Textures Trim Warning",
                        "WARNING: Target texture count is lower than current texture count. " +
                        "Proceeding will drop some trailing textures. Continue?", "Yes", "No");
                    if (trim) {
                        atlas.textures.RemoveRange(curr, prev - curr);
                    }
                } else {
                    for (int i = 0; i < curr - prev; i++) {
                        atlas.textures.Add(new Texture2D(0, 0));
                    }
                }
            }

            for (int i = 0; i < curr; i++) {
                atlas.textures[i] =
                    (Texture2D)EditorGUILayout.ObjectField("User", atlas.textures[i], typeof(Texture2D), false);
            }
        }
    }
}
#endif