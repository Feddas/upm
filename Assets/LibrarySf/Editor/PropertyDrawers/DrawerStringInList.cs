using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

namespace LibrarySf
{
    [CustomPropertyDrawer(typeof(StringInListAttribute))]
    public class DrawerStringInList : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stringInList = attribute as StringInListAttribute;
            var list = stringInList.List;
            if (property.propertyType == SerializedPropertyType.String)
            {
                int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
                index = EditorGUI.Popup(position, property.displayName, index, list);

                property.stringValue = list[index];
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.Popup(position, property.displayName, property.intValue, list);
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
    }
#endif
}
