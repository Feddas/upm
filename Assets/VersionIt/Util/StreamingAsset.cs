using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Feddas
{
    /// <summary>
    /// Enables the Unity Inspector to reference a StreamingAsset file.
    /// Alternative is to use a custom editor: https://forum.unity.com/threads/drag-and-drop-streaming-asset-to-inspector-to-get-file-path.499055/
    /// This class relies on System.IO. An alternative is at https://forum.unity.com/threads/cant-use-json-file-from-streamingassets-on-android-and-ios.472164/
    /// </summary>
    [System.Serializable]
    public class StreamingAsset
    {
        /// <summary> This path is used by UnityEditor.AssetDatabase to locate files </summary>
        public const string AssetDatabasePrefix = "Assets/StreamingAssets";

        /// <summary> This path is used by everything but UnityEditor.AssetDatabase </summary>
        public static readonly string AbsolutePrefix = Application.streamingAssetsPath;

        /// <summary> Absolute path of the StreamingObject. This also includes the file name. Used by System.IO to read/write to the file </summary>
        public string AbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(_absolutePath)
                    || _absolutePath.Contains(FilePath) == false) // handle FilePath being modified at runtime
                {
                    _absolutePath = Path.Combine(AbsolutePrefix, FilePath.TrimStart('/')); // '/' roots the path during a combine, causing the first argument to be tossed
                }
                return _absolutePath;
            }
        }
        private string _absolutePath;

        /// <summary> true when file at FilePath exists on the file system </summary>
        public bool Exists
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) == false
                    && File.Exists(AbsolutePath);
            }
        }

        /// <summary> Path of a file inside of the streaming assets folder, relative to streamingAssetsPath. This also includes the file name and extension. </summary>
        public string FilePath;

        /// <summary> Filters files that show up in the dropdown from the StreamingAssets folder. This defines the System.IO.Directory.GetFiles searchPattern </summary>
        public string FileFilter = "*.json";

        /// <summary> Initialize serialization by creating file that has FilePath set to fileName </summary>
        public StreamingAsset(string fileName) : this(fileName, null) // initialize serialization with just FilePath
        {
        }

        /// <summary> Initialize using a json serialization of initialData </summary>
        public StreamingAsset(string fileName, object initialData)
        {
            // create initial file
            FilePath = fileName;
            Save(initialData ?? this);
        }

        /// <summary> Serialize an object into StreamingAssets/FileName </summary>
        public void Save<T>(T objectToSerialize)
        {
            createStreamingAssetsFolder();
            string fileContents = JsonUtility.ToJson(objectToSerialize, true)
                + "\n"; // ovrsource likes an empty LF newline (unix style) at the end of every file;

#if UNITY_EDITOR
            // Create the asset using the AssetDatabase to avoid AssetDatabase.Refresh() for file references.
            TextAsset emptyFile = new TextAsset();
            UnityEditor.AssetDatabase.CreateAsset(emptyFile, Path.Combine(AssetDatabasePrefix, FilePath));
            UnityEditor.AssetDatabase.SaveAssets(); // emptyFile is the last asset that will be added.

            // Write file contents using System.IO
            try
            {
                File.WriteAllText(AbsolutePath, fileContents);
            }
            catch (System.Exception ex)
            {
                string msg = " threw:\n" + ex.ToString();
                Debug.LogError(msg);
                UnityEditor.EditorUtility.DisplayDialog("Error when trying to regenerate file", FilePath + msg, "OK");
            }
#else
            File.WriteAllText(AbsolutePath, fileContents);
#endif
        }

        /// <summary> Deserializes the contents of the json file into a scriptableobject </summary>
        public void Load(object scriptableObject)
        {
            var fileContents = File.ReadAllText(AbsolutePath);

            // deserialize into scriptableObject https://stackoverflow.com/a/42905749
            JsonUtility.FromJsonOverwrite(fileContents, scriptableObject);
        }

        private void createStreamingAssetsFolder()
        {
            if (Directory.Exists(AbsolutePrefix + "/") == false)
            {
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateFolder("Assets", "StreamingAssets");
#else
                Directory.CreateDirectory(AbsolutePrefix + "/");
#endif
            }
        }
    }
}
