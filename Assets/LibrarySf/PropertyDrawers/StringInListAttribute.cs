using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LibrarySf
{
    /// <summary>
    /// Turn a string field into a multi-choice dropdown
    /// copied from https://gist.github.com/ProGM/9cb9ae1f7c8c2a4bd3873e4df14a6687
    /// https://coderwall.com/p/wfy-fa/show-a-popup-field-to-serialize-string-or-integer-values-from-a-list-of-choices-in-unity3d
    /// </summary>
    public class StringInListAttribute : UnityEngine.PropertyAttribute
    {
        public string[] List { get; private set; }

        /// <summary> Build a Unity inspector dropdown list using a string[] </summary>
        public StringInListAttribute(params string[] list)
        {
            List = list;
        }

        /// <summary> Build a Unity inspector dropdown list using a static method </summary>
        /// <param name="type">data type that contains the method</param>
        /// <param name="methodName">This static method must return a string[]. See AllSceneNames for an example.</param>
        public StringInListAttribute(Type type, string methodName)
        {
            var method = type.GetMethod(methodName);
            if (method != null)
            {
                List = method.Invoke(null, null) as string[];
            }
            else
            {
                Debug.LogError("NO SUCH METHOD " + methodName + " FOR " + type);
            }
        }

#if UNITY_EDITOR
        public static string[] AllSceneNames()
        {
            var temp = new List<string>();
            foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
            {
                if (S.enabled)
                {
                    string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                    name = name.Substring(0, name.Length - 6);
                    temp.Add(name);
                }
            }
            return temp.ToArray();
        }
#endif
    }
}
