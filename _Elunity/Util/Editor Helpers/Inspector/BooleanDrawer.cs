
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Boolean : PropertyAttribute {
    public int size;
    public Boolean(int size) {
        this.size = size;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Boolean))]
public class BooleanDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Boolean boolean = attribute as Boolean;
        EditorGUI.BeginProperty(position, label, property);

        position.width = 100;
        EditorGUI.LabelField(position, label);

        position.x += 90;
        position.width = 30;
        
        CreateToggle(ref position, ref property, "X", 1);
        if (boolean.size < 2)
            return;

        CreateToggle(ref position, ref property, "Y", 2);
        if (boolean.size < 3)
            return;

        CreateToggle(ref position, ref property, "Z", 4);
        if (boolean.size < 4)
            return;

        CreateToggle(ref position, ref property, "W", 8);
    }

    void CreateToggle(ref Rect position, ref SerializedProperty property, string label, int bitOrder) {
        position.x += 30;
        bool check = (bitOrder == (bitOrder & property.intValue));

        EditorGUI.BeginChangeCheck();
        EditorGUI.ToggleLeft(position, label, check);
        if (EditorGUI.EndChangeCheck()) {
            property.intValue = check ? property.intValue - bitOrder : property.intValue + bitOrder;
        }
    }
}
#endif