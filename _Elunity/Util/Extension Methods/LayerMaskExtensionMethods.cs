using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang
{
    // LayerMask Extension Methods
    public static class LayerMaskExtensionMethods
    {
#if UNITY_EDITOR
        public static void CreateLayer(this UnityEngine.Object obj, string name) {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++) {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name) return;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null) {
                UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }
#endif
        public static void SetLayer(this GameObject obj, string layerName) {
            obj.layer = LayerMask.NameToLayer(layerName);
        }
        public static void Add(this ref LayerMask mask, string s) {
            var layer = LayerMask.NameToLayer(s);
            if (layer != -1)
                mask.value |= 1 << layer;
        }
        public static void Add(this ref LayerMask mask, params string[] s) {
            for (int i = 0; i < s.Length; i++) {
                var layer = LayerMask.NameToLayer(s[i]);
                if (layer != -1)
                    mask.value |= 1 << layer;
            }
        }
        public static void Remove(this ref LayerMask mask, string s) {
            var layer = LayerMask.NameToLayer(s);
            if (layer != -1)
                mask.value &= ~(1 << layer);
        }
        public static void Remove(this ref LayerMask mask, params string[] s) {
            for (int i = 0; i < s.Length; i++) {
                var layer = LayerMask.NameToLayer(s[i]);
                if (layer != -1)
                    mask.value &= ~(1 << layer);
            }
        }
        public static void SetOnly(this ref LayerMask mask, string s) {
            var layer = LayerMask.NameToLayer(s);
            if (layer != -1)
                mask = 1 << layer;
        }
        public static void SetAllExcept(this ref LayerMask mask, string s) {
            var layer = LayerMask.NameToLayer(s);
            if (layer != -1) {
                mask.value = 1 << layer;
                mask.value = ~mask.value;
            }
        }
        public static void SetAllExcept(this ref LayerMask mask, params string[] s) {
            mask.value = 0;
            for (int i = 0; i < s.Length; i++) {
                var layer = LayerMask.NameToLayer(s[i]);
                if (layer != -1)
                    mask.value |= 1 << layer;
            }
            mask.value = ~mask.value;
        }
    }
}