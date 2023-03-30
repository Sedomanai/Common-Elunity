using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Elang {
    [Serializable]
    public class GaugeBar {

        public enum State {
            NONE,
            EMPTY,
            FULL
        }

        [SerializeField]
        Vector2Int _range = new Vector2Int(0, 100);
        [SerializeField]
        int _value = 100;

        public Vector2Int range { get { return _range; } 
            set {
                _range = value;
                _value = Mathf.Clamp(_value, _range.x, _range.y);
            }
        }
        public int value { get { return _value; } 
            set {
                _value = Mathf.Clamp(value, range.x, range.y);
            }
        }

        public void Damage(int point) {
            value = _value - point;
        }

        public State CheckState() {
            return (_value <= range.x) ? State.EMPTY : (_value >= range.y) ? State.FULL : State.NONE;
        }

        public void Fill() {
            _value = range.y;
        }

        public void Empty() {
            _value = range.x;
        }

        public void ExtendRange(Vector2Int range) {
            _range.x += range.x;
            _range.y += range.y;
            _value = Mathf.Clamp(_value, range.x, range.y);
        }

        public float GetRatio() {
            return (float)value / (float)range.y;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GaugeBar))]
    public class GaugeBarDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            int oldIndentLevel = EditorGUI.indentLevel;
            label = EditorGUI.BeginProperty(position, label, property);
            var drawrect = position;
            drawrect = EditorGUI.PrefixLabel(drawrect, label);
            var range = property.FindPropertyRelative("_range");
            var value = property.FindPropertyRelative("_value");

            Vector2Int v2 = range.vector2IntValue;
            float leftover = drawrect.width;

            float isize = 45f;
            drawrect.width = isize;
            int min = EditorGUI.DelayedIntField(drawrect, v2.x);
            leftover -= (isize + 2) * 2;

            drawrect.x += isize + 2;
            drawrect.width = leftover;
            int prev = value.intValue;
            int val = EditorGUI.IntSlider(drawrect, prev, v2.x, v2.y);

            drawrect.x += leftover;
            drawrect.width = isize;
            int max = EditorGUI.DelayedIntField(drawrect, v2.y);

            if (max != v2.y || min != v2.x) {
                float ratio = (float)(prev - v2.x) / (float)(v2.y - v2.x);
                range.vector2IntValue = new Vector2Int(min, max);
                value.intValue = min + Mathf.FloorToInt(((max - min) * ratio));
            } else value.intValue = val;

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = oldIndentLevel;
        }
    }
#endif
}