using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LibrarySf
{
    /// <summary>
    /// copied from https://youtu.be/VBA1QCoEAX4?t=30m32s / https://bitbucket.org/richardfine/scriptableobjectdemo/src/9a60686609a42fea4d00f5d20ffeb7ae9bc56eb9/Assets/ScriptableObject/Audio/Editor/RangedFloatDrawer.cs
    /// Vector2 based alternative: https://gist.github.com/frarees/9791517
    /// </summary>
    [CustomPropertyDrawer(typeof(MinMaxRange), true)]
    public class DrawerMinMaxRange : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            // load in current values
            SerializedProperty minProp = property.FindPropertyRelative("MinValue");
            SerializedProperty maxProp = property.FindPropertyRelative("MaxValue");
            float minValue = minProp.floatValue;
            float maxValue = maxProp.floatValue;

            // Set bounds of range setable in the inspector
            float rangeMin = 0;
            float rangeMax = 1;
            var ranges = (MinMaxRangeAttribute[])fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
            if (ranges.Length > 0)
            {
                rangeMin = ranges[0].Min;
                rangeMax = ranges[0].Max;
            }

            // display min and max textboxes
            const float rangeBoundsLabelWidth = 30f;
            var rangeBoundsLabel1Rect = new Rect(position);
            rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth; // left align minProp TextField
            validateTextBox(rangeBoundsLabel1Rect, minProp, rangeMin, maxProp.floatValue);
            position.xMin += rangeBoundsLabelWidth;
            var rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth; // right align maxProp TextField
            validateTextBox(rangeBoundsLabel2Rect, maxProp, minProp.floatValue, rangeMax);
            position.xMax -= rangeBoundsLabelWidth;

            // render MinMaxSlider
            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                minProp.floatValue = minValue;
                maxProp.floatValue = maxValue;
            }
            EditorGUI.EndProperty();
        }

        private void validateTextBox(Rect rect, SerializedProperty property, float minValue, float maxValue)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, property, GUIContent.none); // draw the textbox
            if (EditorGUI.EndChangeCheck())
            {
                // make sure value is set to minValue - maxValue range
                property.floatValue = Mathf.Clamp(property.floatValue, minValue, maxValue);
            }
        }
    }
}
