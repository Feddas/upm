using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Feddas
{
    [CustomPropertyDrawer(typeof(StreamingAsset))]
    public class DrawerStreamingAsset : PropertyDrawer
    {
        /// <summary> Loaded assets that match the FilePath of the StreamingAsset to allow for easy linking to physcial files in the Unity editor </summary>
        private static readonly Dictionary<string, Object> cachedObjects = new Dictionary<string, Object>();

        /// <summary> Gets the path of streamingObject relative to the StreamingAssets folder </summary>
        public static string StreamingPathOf(Object streamingObject)
        {
            if (streamingObject == null)
            {
                return "";
            }

            // Determine path relative to StreamingAssets folder
            string assetPath = AssetDatabase.GetAssetPath(streamingObject);
            if (assetPath.StartsWith(StreamingAsset.AssetDatabasePrefix))
            {
                assetPath = assetPath.Substring(StreamingAsset.AssetDatabasePrefix.Length).TrimStart('/');  // '/' roots the path during a combine, causing the first argument to be tossed
            }
            return assetPath;
        }

        private EditorUiFilePopup popup
        {
            get
            {
                if (_popup == null)
                {
                    _popup = new EditorUiFilePopup("/StreamingAssets/", fileFilter, hasNone: true);
                }
                return _popup;
            }
        }
        private EditorUiFilePopup _popup;

        private string fileFilter; // StreamingAsset.cs's FileFilter defaults to "*.json"

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Undo support
            Undo.RecordObject(property.serializedObject.targetObject, property.propertyPath);
            label = EditorGUI.BeginProperty(position, label, property);

            // Get current property values
            SerializedProperty serialFilePath = property.FindPropertyRelative("FilePath");

            // initialize fileFilter if it hasn't been set yet
            UpdateFileFilter(property);

            // Load the file that matches the FilePath
            string filePathValue = serialFilePath.stringValue = checkObjectFileChanged(serialFilePath.stringValue);

            // Draw Ui
            position = EditorGUI.PrefixLabel(position, label);
            position = ObjectFieldReadOnly(position, filePathValue, label, 18); // TODO: allow drag and drop simliar to //cachedObjects[filePathValue] = EditorGUI.ObjectField(position, cachedObjects[filePathValue], typeof(Object), false);
            popup.Draw(position, serialFilePath);

            // SetDirty if path has changed by the user.
            if (filePathValue != serialFilePath.stringValue)
            {
                UpdateFileFilter(property, force: true); // assume possible changes made to fileFilter
                popup.SetDirty();
            }

            // Undo support
            EditorGUI.EndProperty();
        }

        private void UpdateFileFilter(SerializedProperty property, bool force = false)
        {
            // Only update the filter if it passes this guard
            if (force == false
                && _popup != null // popup has already been initalized
                && string.IsNullOrEmpty(fileFilter) == false)
                return;

            // Do the fileFileFilter update
            fileFilter = property.FindPropertyRelative("FileFilter").stringValue;
            popup.ChangeFilter(fileFilter);
        }

        /// <summary>
        /// Custom ObjectField without picker: https://stackoverflow.com/questions/51397407/is-there-any-way-to-hide-the-object-picker-of-an-editorguilayout-objectfield-i
        /// </summary>
        /// <param name="reservedWidth"> how much width to leave for other remaining UI controls </param>
        /// <returns> space left for remaining UI controls </returns>
        private Rect ObjectFieldReadOnly(Rect position, string filePathValue, GUIContent label, float reservedWidth)
        {
            // get icon for object
            var assetFile = cachedObjects[filePathValue];
            var guiContent = EditorGUIUtility.ObjectContent(assetFile, typeof(Object));

            // Draw property field
            Rect buttonRect = new Rect(position);
            buttonRect.xMax -= reservedWidth;
            var style = new GUIStyle("TextField");
            style.imagePosition = assetFile ? ImagePosition.ImageLeft : ImagePosition.TextOnly;
            if (GUI.Button(buttonRect, guiContent, style) && assetFile)
                EditorGUIUtility.PingObject(assetFile);

            // calculate remaining UI space
            position.xMin = buttonRect.xMax;
            return position;
        }

        private string checkObjectFileChanged(string filePathValue)
        {
            // ensure old filePathValue key exists
            updateCachedObject(filePathValue);

            // Update the file path to match the streamingAsset, incase it has moved
            string newValue = StreamingPathOf(cachedObjects[filePathValue]);
            if (filePathValue != newValue)
            {
                updateCachedObject(newValue);
            }
            return newValue;
        }

        private void updateCachedObject(string filePathValue)
        {
            // new key
            if (cachedObjects.ContainsKey(filePathValue) == false)
            {
                Object streamingAsset = string.IsNullOrEmpty(filePathValue)
                    ? null
                    : AssetDatabase.LoadAssetAtPath<Object>(Path.Combine(StreamingAsset.AssetDatabasePrefix, filePathValue));
                cachedObjects.Add(filePathValue, streamingAsset);
            }
        }
    }
}
