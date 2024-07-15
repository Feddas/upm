using UnityEngine;
using System.IO;
using System.Linq;

namespace Feddas
{
    /// <summary> Reads and writes the values stored in package.json </summary>
    public class PackageParser
    {
        /// <summary> path to "Assets/???/package.json". When path is set to null attempts to write to package.json are silently ignored </summary>
        public string PackageFilePath;

        public PackageInfoSerializable PackageInfo;

        /// <summary> Major, Minor, Patch </summary>
        public uint[] Version = new uint[3];

        /// <param name="packageJson">
        /// When packageIsFilePath is true this is treated as a file path.
        /// When false, it's treated as the text contents of a json file
        /// </param>
        /// <param name="packageIsFilePath"> Changes how packageJson is treated </param>
        public PackageParser(string packageJson, bool packageIsFilePath = true)
        {
            if (packageIsFilePath)
            {
                PackageFilePath = packageJson;
                PackageInfo = LoadPackageFilePath(PackageFilePath);
                Version = ParseVersionIntArray(ref PackageInfo.version);
            }
            else
            {
                PackageInfo = LoadPackageJson(packageJson);
                Version = ParseVersionIntArray(ref PackageInfo.version);
            }
        }

        public PackageInfoSerializable LoadPackageFilePath(string packageFilePath)
        {
            if (File.Exists(packageFilePath) == false)
                return null;

            using (StreamReader stream = new StreamReader(packageFilePath))
            {
                return LoadPackageJson(stream.ReadToEnd());
            }
        }

        public PackageInfoSerializable LoadPackageJson(string packageJson)
        {
            try
            {
                return Deserialize(packageJson);
            }
            catch (System.ArgumentException e)
            {
                PackageInfo = null; // make sure old contents of PackageInfo are purged
                throw new System.ArgumentException("Not a valid package.json file", e);
            }
        }

        /// <summary> Applies values make in PackageInfo to package.json file </summary>
        public void SavePackageInfo()
        {
            OverwritePackageJson(PackageInfo);
        }

        /// <summary> Replaces package.json with the information in packageInfo </summary>
        public void OverwritePackageJson(PackageInfoSerializable packageInfo)
        {
            if (string.IsNullOrEmpty(PackageFilePath)) // This case is mostly here to enable Unit Tests
            {
                return;
            }
            else if (File.Exists(PackageFilePath) == false)
            {
                Debug.LogError("Could not find " + PackageFilePath + " file to OverwritePackageJson");
                return;
            }

            using (StreamWriter stream = new StreamWriter(PackageFilePath))
                stream.Write(Serialize(packageInfo));
        }

        public static string Serialize(PackageInfoSerializable packageInfo)
        {
            string jsonText = JsonUtility.ToJson(packageInfo, true); // sample value = @"{""name"":""com.feddas.unitymodules"",""dependencies"":[""com.unity.standardevents\"": \""1.0.13"",""com.shawn.testing\"": \""0.0.1""],""unity"":""2018.2.0f2""}";
            jsonText = convertDependencies(packageInfo, jsonText);
            return jsonText
                + "\n"; // ovrsource likes an empty LF newline (unix style) at the end of every file
        }

        public static PackageInfoSerializable Deserialize(string jsonText)
        {
            string asArray = dependenciesToArray(jsonText);
            return JsonUtility.FromJson<PackageInfoSerializable>(asArray);
        }

        private static string convertDependencies(PackageInfoSerializable packageInfo, string jsonText)
        {
            if (packageInfo.dependencies == null) // no dependencies need to be converted
            {
                return jsonText;
            }

            bool wellFormatted = packageInfo.dependencies.All(text => text.Contains(":"));
            if (wellFormatted) // dependencies are ready to be converted
            {
                return dependenciesToObjects(jsonText);
            }
            else // dependencies must be malformed, remove them
            {
                Debug.LogWarning(packageInfo.name + " has malformed dependencies. All dependencies removed until this is fixed. Each dependency must include both the name and the version in the following format: the.name.thing\": \"0.0.1");
                return dependenciesRemoval(jsonText);
            }
        }

        /// <summary>
        /// Takes json that has the "dependencies" field as string arrays and converts it to npm standard objects
        /// </summary>
        /// <param name="jsonAsArray">json that has "dependencies" value listed as a string array</param>
        private static string dependenciesToObjects(string jsonAsArray)
        {
            // parse out the dependencies section of the json
            string dependenciesPattern = @"""dependencies""+\s*:\s*\[[^\]]*],?";
            var dependenciesSection = System.Text.RegularExpressions.Regex.Match(jsonAsArray, dependenciesPattern);

            if (dependenciesSection.Success)
            {
                // convert brackets to object form instead of array form
                string dependenciesJson = dependenciesSection.Value.Replace("[", "{").Replace("]", "}");

                // convert each string value in the array to object values
                string stringPattern = @"(""[a-zA-Z0-9\.]+)\\""+\s*:\s*\\(""+[a-zA-Z0-9\.]+""+)"; // in "com.unity.standardevents\"": \""1.0.13"" $1 = "com.unity.standardevents $2 = ""1.0.13""
                dependenciesJson = System.Text.RegularExpressions.Regex.Replace(dependenciesJson, stringPattern, "$1\": $2");

                // insert no-longer-an-array json back into dependencies
                jsonAsArray = System.Text.RegularExpressions.Regex.Replace(jsonAsArray, dependenciesPattern, dependenciesJson);
            }

            return jsonAsArray;
        }

        private static string dependenciesRemoval(string jsonText)
        {
            // parse out the dependencies section of the json
            string dependenciesPattern = @"""dependencies""+\s*:\s*\[[^\]]*],?";
            jsonText = System.Text.RegularExpressions.Regex.Replace(jsonText, dependenciesPattern, "");
            return jsonText;
        }

        /// <summary>
        /// Takes json that has the "dependencies" field as npm standard objects and converts it to string arrays
        /// by including both the packageName and version in each string element. Does this by converting ":  to \": \
        /// </summary>
        /// <param name="jsonAsObject">json that has "dependencies" value listed as an object</param>
        private static string dependenciesToArray(string jsonAsObject)
        {
            // parse out the dependencies section of the json
            string dependenciesPattern = @"""dependencies""+\s*:\s*{[^}]*}"; // https://regexr.com/41l1a
            var dependenciesSection = System.Text.RegularExpressions.Regex.Match(jsonAsObject, dependenciesPattern);

            if (dependenciesSection.Success)
            {
                // convert brackets to object form instead of array form
                string dependenciesJson = dependenciesSection.Value.Replace("{", "[").Replace("}", "]");

                // convert each string value in the array to object values
                string stringPattern = @"(""[a-zA-Z0-9\.]+)""\s*:\s*(""+[a-zA-Z0-9\.]+""+)"; // in "com.unity.standardevents": "1.0.13" $1 = "com.unity.standardevents $2 = "1.0.13"
                dependenciesJson = System.Text.RegularExpressions.Regex.Replace(dependenciesJson, stringPattern, "$1\\\": \\$2");

                // insert no-longer-an-array json back into dependencies
                jsonAsObject = System.Text.RegularExpressions.Regex.Replace(jsonAsObject, dependenciesPattern, dependenciesJson);
            }

            return jsonAsObject;
        }

        public static uint[] ParseVersionIntArray(ref string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
                return null;

            byte semanticVersionParts = 3;
            var stringParts = versionString.Split('.');
            var uintParts = new uint[semanticVersionParts];
            bool dirtyString = stringParts.Length > semanticVersionParts; // whether the versionString needs to be updated due to it being unparseable

            for (uint i = 0; i < semanticVersionParts; ++i)
            {
                if (i < stringParts.Length) // get the value from the versionString
                {
                    if (uint.TryParse(stringParts[i], out uintParts[i]))
                        continue; // use the parsed value
                    else
                    {
                        Debug.LogErrorFormat
                        (
                            "Version value unacceptable. The {0} element ({1}) must be parsable as an integer. Modify {2} in package.json."
                            , new string[] { "1st", "2nd", "3rd" }[i] // assumes semanticVersionParts will never be more than 3
                            , stringParts[i]
                            , versionString
                        );
                    }
                }

                // use a fallback value
                dirtyString = true;
                uintParts[i] = 0;
            }

            if (dirtyString) // the conversion wasn't clean, convert uintParts back to overwrite the string value
            {
                versionString = string.Join(".", uintParts.Select(i => i.ToString()).ToArray());
            }

            return uintParts;
        }

        public void IncrementPatchVersion()
        {
            // increment at the patch index, which is 2
            ++Version[2];
            string newVersion = string.Join(".", Version.Select(i => i.ToString()).ToArray());

            // save version info
            saveNewVersion(newVersion);
        }

        private void saveNewVersion(string newVersion)
        {
            string lastVersion = PackageInfo.version;

            // save version info
            PackageInfo.version = newVersion;
            OverwritePackageJson(PackageInfo);

            // notify user
            Debug.Log("App version now " + PackageInfo.version + " replacing previous version "
                + lastVersion + ".\n"
                + "In " + PackageFilePath + ".");
        }
    }
}
