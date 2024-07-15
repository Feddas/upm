using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Feddas // don't cause a performance hit on objects outside of this namespace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class EditorInspectorButtonAttribute : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var methods = this.target.GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(o => System.Attribute.IsDefined(o, typeof(InspectorButtonAttribute)));

            foreach (MethodInfo memberInfo in methods)
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(memberInfo.Name)))
                {
                    foreach (var t in this.targets) // supports [CanEditMultipleObjects]
                    {
                        memberInfo.Invoke(t, null);
                    }
                    GUIUtility.ExitGUI(); // fixes https://forum.unity.com/threads/endlayoutgroup-beginlayoutgroup-must-be-called-first.523209/
                }
            }
        }
    }
}
