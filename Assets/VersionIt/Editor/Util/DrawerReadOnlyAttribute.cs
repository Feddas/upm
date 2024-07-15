using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Feddas
{
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class DrawerReadOnlyAttribute : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect rect, UnityEditor.SerializedProperty prop, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(rect, prop, label, true);
            GUI.enabled = wasEnabled;
        }

        /// <summary> includes children in property height </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }
    }
}
