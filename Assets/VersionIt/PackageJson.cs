using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // TODO: figure out how to move uses of this namespace to a file inside of the Editor folder
#endif

namespace Feddas
{
    [System.Serializable]
    public struct SemanticVersion
    {
        [Tooltip("Value as stored in *.json file")]
        public string AsString;

        [Tooltip("Values up to 255 for each octet")]
        public byte[] AsBytes;

        [Tooltip("Large values, such as 2019.1.4294967295")]
        public uint[] AsUint;
    }

    /// <summary>
    /// Create one instance of this ScriptableObject per package.json to be managed.
    /// </summary>
    [CreateAssetMenu(fileName = "VersionNameOfProject", menuName = "Feddas/Version It")]
    public class PackageJson : ScriptableObject
    {
        [ReadOnly]
        [Tooltip("Read only value, Modify PackageInfo instead.")]
        public string Name;

        [ReadOnly]
        [Tooltip("Version info as major.minor.patch")]
        public SemanticVersion Version;

        [Tooltip("the package.json to managed by this ScriptableObject")]
        public TextAsset PackageJsonFile;

        [Tooltip("When false only updates package.json. Otherwise build setting values are also updated.")]
        public bool IsSavingToBuildSettings = true;

        [Tooltip("When true StreamingAssets/VersionData.json is synced to this version")]
        public bool IsSavingToStreamingAsset = false;

        [Tooltip("Data stored in this ScriptableObject, may not yet be synced with package.json")]
        public PackageInfoSerializable PackageInfo;

        /// <summary> data parsed from package.json </summary>
        private PackageParser parser
        {
            get
            {
#if UNITY_EDITOR
                if (PackageJsonFile == null)
                {
                    _parser = null;
                }
                else if (_parser == null)
                {
                    _parser = new PackageParser(AssetDatabase.GetAssetPath(PackageJsonFile));
                }
#endif
                return _parser;
            }
        }
        private PackageParser _parser;

#if UNITY_EDITOR
        private string lastPath;

        private bool ranAwake = false;

        /// <summary> This is invoked when an instance of this ScriptableObject is first created
        /// https://forum.unity.com/threads/scriptableobject-behaviour-discussion-how-scriptable-objects-work.541212/ </summary>
        private void Awake()
        {
            ranAwake = true;

            // Don't overwrite a link to an existing package.json file
            if (PackageJsonFile != null)
            {
                LoadFromJsonFile(); // ensure package.json is the source of truth
                return;
            }

            // check to see if package.json already exists
            var preferredPath = this.preferredPath();

            // var absolutePath = Application.dataPath.Replace("Assets", preferredPath);
            var HasJson = System.IO.File.Exists(preferredPath);

            // Creates package.json if it does not exist in the folder this asset was created
            if (HasJson) // if package.json already exists in this folder, use it
            {
                lastPath = preferredPath;

                // Wait for the editor to finish compiling the assetdatabase before assigning package.json
                EditorApplication.delayCall += bindExistingJson;
            }
            else // else create a new package.json to use
            {
                // handle 'this' not yet being initialized
                PackageInfo = new PackageInfoSerializable();

                newPackageJson(preferredPath);
            }
        }

        void OnValidate()
        {
            if (false == ranAwake) // AssetDatabase hasn't loaded yet, can't load package.json yet
                return;

            LoadPackageJsonFile();
        }

        /// <summary> Applies values from the Unity inspector to package.json & build settings </summary>
        [InspectorButton]
        private void SavePackageInfo()
        {
            SavePackageInfoToJson();
            syncPackageInfo();
        }

        /// <summary> Requires this ScriptableObject to already exists. Therefore, this function can't be called on Awake() </summary>
        [InspectorButton]
        private void NewPackageJson()
        {
            newPackageJson(preferredPath());
        }

        /// <summary> Preferred location of package.json, utilizes folder of this ScriptableObject </summary>
        private string preferredPath()
        {
            // get folder of this ScriptableObject differently depending on if this ScriptableObject previously existed
            var relativeFolder = AssetDatabase.GetAssetPath(this); // note: this may not work with scopedRegistries
            if (string.IsNullOrEmpty(relativeFolder)) // handle this object being newly created, causing 'this' to have no good values
            {
                relativeFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
            }
            else  // handle UnityEditor loading
            {
                relativeFolder = System.IO.Path.GetDirectoryName(relativeFolder);
            }

            return System.IO.Path.Combine(relativeFolder, "package.json");
        }

        private void newPackageJson(string atPath)
        {
            // create new package.json file based off Application.values
            if (string.IsNullOrEmpty(PackageInfo.name))
                PackageInfo.name = Application.identifier;
            if (string.IsNullOrEmpty(PackageInfo.version))
                PackageInfo.version = Application.version;
            if (string.IsNullOrEmpty(PackageInfo.unity))
            {
                // remove patch version, '.0f2' from 2018.2.0f2, as it will cause UPM issues in Unity 2019.1+.
                // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/PackageManagerUI/Editor/Services/Common/ApplicationUtil.cs#L97
                var unityVersionParts = Application.unityVersion.Split('.');
                PackageInfo.unity = unityVersionParts[0] + "." + unityVersionParts[1];
            }

            // create / overwrite package.json file
            System.IO.File.WriteAllText(atPath, PackageParser.Serialize(PackageInfo));
            AssetDatabase.Refresh();

            // Link this object to the newly created asset
            var newJsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(atPath);
            PackageJsonFile = newJsonFile;
            loadInfo(PackageInfo);
        }

        [InspectorButton]
        private void LoadFromJsonFile()
        {
            lastPath = Time.time.ToString(); // mark PackageJsonFile as dirty
            LoadPackageJsonFile();
        }

        [InspectorButton]
        private void LoadBuildSettings()
        {
            PackageInfo.name = Application.identifier;
            PackageInfo.version = Application.version;
            PackageInfo.unity = Application.unityVersion;
            loadInfo(PackageInfo);
        }

        [InspectorButton]
        private void IncrementPatchVersion()
        {
            requiresPackageJsonFile(before: "version can be incremented");

            // update package.json file path incase package.json has moved since it was first set
            parser.PackageFilePath = AssetDatabase.GetAssetPath(PackageJsonFile);

            // increment version, save file, and reload content
            parser.IncrementPatchVersion();
            syncPackageInfo();
            loadInfo(parser.PackageInfo);

            // Make sure the inspector sees the updated package.json text
            AssetDatabase.ImportAsset(parser.PackageFilePath); // this does the same as AssetDatabase.Refresh(); but to a specific file
        }

        private void bindExistingJson()
        {
            EditorApplication.delayCall -= bindExistingJson;

            PackageJsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(lastPath);
            LoadFromJsonFile();
        }

        private void syncPackageInfo()
        {
            SavePackageInfoToBuildSettings();
            SavePackageInfoToStreamingAssets();
        }

        /// <summary> Applies values from the Unity inspector to package.json file
        /// also referenced by [ContextMenuItem("Save to package.json", "SavePackageInfoToJson")] </summary>
        private void SavePackageInfoToJson()
        {
            requiresPackageJsonFile(before: "a save can be completed");

            // Make it clear these values are not being saved by overwriting them
            this.Name = this.PackageInfo.name;
            this.Version.AsString = this.PackageInfo.version;
            parser.PackageInfo = this.PackageInfo;

            // Perform the save
            parser.SavePackageInfo();

            // Make sure the inspector sees the updated package.json text
            AssetDatabase.Refresh();
        }

        /// <summary> Applies values from the Unity inspector to build settings
        /// also referenced by [ContextMenuItem("Save to BuildSettings", "SavePackageInfoToBuildSettings")] </summary>
        private void SavePackageInfoToBuildSettings()
        {
            if (IsSavingToBuildSettings == false)
                return;

            // Make it clear these values are not being saved by overwriting them
            this.Name = this.PackageInfo.name;
            this.Version.AsString = this.PackageInfo.version;

            // Perform the save
            var buildTarget = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            PlayerSettings.SetApplicationIdentifier(buildTarget, PackageInfo.name); // Application.identifier
            PlayerSettings.bundleVersion = PackageInfo.version; // Application.version
        }

        /// <summary> Applies values from the Unity inspector to StreamingAssets/VersionData.txt </summary>
        private void SavePackageInfoToStreamingAssets()
        {
            if (IsSavingToStreamingAsset == false)
                return;

            StreamingAsset streamingAsset = new StreamingAsset("VersionData.json");
            streamingAsset.Save(PackageInfo);
        }
#endif

        private void requiresPackageJsonFile(string before)
        {
            if (PackageJsonFile == null)
                throw new System.Exception("A valid package.json must be set in " + this.name + ".asset's PackageJsonFile field before " + before);
        }

        /// <summary> Initializes PackageInfo </summary>
        public void LoadPackageJsonFile()
        {
            // handle no file
            if (PackageJsonFile == null)
            {
                PackageInfo = null;
#if UNITY_EDITOR
                lastPath = "";
                requiresPackageJsonFile(before: "editing fields. Click the \"New Package Json\" button to generate a package.json file.");
#endif
                return;
            }

#if UNITY_EDITOR
            // continue only if file has changed
            string newPath = AssetDatabase.GetAssetPath(PackageJsonFile);
            if (newPath == lastPath)
                return;

            // use new file
            lastPath = newPath;
            tryParseJson(newPath);
            loadInfo(parser.PackageInfo);
#endif
        }

        private byte[] getBytes(uint[] parserVersion)
        {
            return new byte[]
            {
                (byte)parserVersion[0]    // major
                , (byte)parserVersion[1]  // minor
                , (byte)parserVersion[2]  // patch
            };
        }

        private void tryParseJson(string atFilePath)
        {
            try
            {
                _parser = new PackageParser(atFilePath);
            }
            catch (System.Exception)
            {
                // reset values as if package.json set to "None"
                PackageJsonFile = null;
                LoadPackageJsonFile();

                throw; // re-throw the exception
            }
        }

        private void loadInfo(PackageInfoSerializable packageInfo)
        {
            PackageInfo = packageInfo;
            if (packageInfo == null)
            {
                requiresPackageJsonFile(before: " values can be loaded.");
                return;
            }

            // load values
            Name = packageInfo.name;

            // setting AsUint first ensures uint and string are set to the same value
            Version.AsUint = PackageParser.ParseVersionIntArray(ref packageInfo.version); // note: don't use parser.Version here as values may be from LoadBuildSettings(), not package.json
            Version.AsString = packageInfo.version;
            Version.AsBytes = getBytes(Version.AsUint);
        }
    }
}
