using UnityEngine;
using System;
#if UNITY_EDITOR

using UnityEditor;

// Reference: @https://catlikecoding.com/unity/tutorials/editor/custom-list/

namespace Elang
{
    public static class EditorList
    {
        [Flags]
        public enum Option
        {
            None = 0,
            ListSize = 1,
            ListLabel = 2,
            ElementLabels = 4,
            Buttons = 8,
            NoElementLabels = ListSize | ListLabel,
            Default = ListLabel | Buttons,
            Legacy = ListSize | ListLabel | ElementLabels,
            All = Legacy | Buttons
        }

        private static GUIContent
            moveButtonContent = new GUIContent("\u21b4", "move down"),
            duplicateButtonContent = new GUIContent("+", "duplicate"),
            deleteButtonContent = new GUIContent("-", "delete"),
            addButtonContent = new GUIContent("+", "add element");

        public static void Show(SerializedProperty list, Option options = Option.Default) {
            // Check if Serialized Object is an array/list
            if (list == null || !list.isArray) {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }

            // Check Option flags
            bool
                showListLabel = (options & Option.ListLabel) != 0,
                showListSize = (options & Option.ListSize) != 0;


            if (showListLabel) {
                //if (showListSize)
                //    EditorGUILayout.BeginHorizontal();
                list.isExpanded = EditorGUILayout.Foldout(list.isExpanded, list.displayName);
                EditorGUI.indentLevel += 1;
            }

            if (!showListLabel || list.isExpanded) {
                SerializedProperty size = list.FindPropertyRelative("Array.size");
                if (size.hasMultipleDifferentValues) {
                    EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
                } else {
                    if (showListSize) {
                        EditorGUILayout.PropertyField(size);
                    }
                    ShowElements(list, options);
                }
            }

            if (showListLabel) {
                EditorGUI.indentLevel -= 1;
            }
        }

        private static void ShowElements(SerializedProperty list, Option options) {
            bool
                showElementLabels = (options & Option.ElementLabels) != 0,
                showButtons = (options & Option.Buttons) != 0;

            for (int i = 0; i < list.arraySize; i++) {
                if (showButtons) {
                    EditorGUILayout.BeginHorizontal();
                }
                if (showElementLabels) {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                } else {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                }
                if (showButtons) {
                    ShowButtons(list, i);
                    EditorGUILayout.EndHorizontal();
                }
            }

            // If buttons are shown but array is empty, add a + button to indicate you need another one.
            if (showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton)) {
                list.arraySize += 1;
            }
        }

        static void ShowButtons(SerializedProperty list, int index) {
            var miniButtonWidth = GUILayout.Width(20f);

            if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth)) {
                list.MoveArrayElement(index, index + 1);
            }
            if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth)) {
                list.InsertArrayElementAtIndex(index);
            }
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth)) {
                int oldSize = list.arraySize;
                list.DeleteArrayElementAtIndex(index);
                if (list.arraySize == oldSize) {
                    list.DeleteArrayElementAtIndex(index);
                }
            }
        }
    }
}
#endif