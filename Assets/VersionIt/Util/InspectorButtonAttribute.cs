using UnityEngine;

namespace Feddas // don't cause a performance hit on objects outside of this namespace
{
    /// <summary>
    /// Methods decorated with [InspectorButton] have a button created in the inspector to call them.
    /// Based off of https://github.com/madsbangh/EasyButtons
    /// & https://www.reddit.com/r/Unity3D/comments/5ybdzi/easy_default_inspector_buttons/dfbhfcz
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
    }

    /// TODO: Go back to the PropertyDrawer, drop the attribute and add InspectorButton type that has the button state (visible, disabled, enabled) and an Action delegate pointing to the function it calls
    //    [System.AttributeUsage(System.AttributeTargets.Field)]
    //    public class InspectorButtonAttribute : PropertyAttribute
    //    {
    //        public readonly string MethodName;

    //        public InspectorButtonAttribute(string MethodName)
    //        {
    //            this.MethodName = MethodName;
    //        }
    //    }

    //#if UNITY_EDITOR
    //    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    //    public class InspectorButtonPropertyDrawer : PropertyDrawer
    //    {
    //        private MethodInfo _eventMethodInfo = null;

    //        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    //        {
    //            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
    //            Rect buttonRect = new Rect(position.x, position.y, position.width, position.height);
    //            if (GUI.Button(buttonRect, label.text))
    //            {
    //                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
    //                string eventName = inspectorButtonAttribute.MethodName;

    //                if (_eventMethodInfo == null)
    //                    _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

    //                if (_eventMethodInfo != null)
    //                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
    //                else
    //                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
    //            }
    //        }
    //    }
    //#endif
}
