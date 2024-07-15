using System.Collections.Generic;

namespace Feddas
{
    /// <summary>
    /// Similiar to UnityEditor.PackageManager.PackageInfo
    /// Yet not the same, as per https://forum.unity.com/threads/serializable-packageinfo.533742/
    /// </summary>
    [System.Serializable]
    public class PackageInfoSerializable
    {
        [System.Serializable]
        public class Sample
        {
            public string displayName;
            public string description;
            public string path;
        }

        public string name;
        public string displayName;
        public string version;

        [UnityEngine.Tooltip("i.e. 2018.2 Note: including patch version, '.0f2', will cause issues with Unity 2019.1+.")]
        public string unity;
        public string category;
        public string description;
        public string author;
        public string homepage;

        [UnityEngine.Tooltip("This should include both the name and version without double quotes on the outside, but double quote delimiter. i.e.\ncom.unity.standardevents\": \"1.0.13\nhas double quotes missing from the outside, it should not be \n\"com.unity.standardevents\": \"1.0.13\"")]
        public string[] dependencies = { }; // initialize dependencies as empty, with count 0 (if it was null, PackagePaser will ignore it causing "[]" vs "{}" errors)
        //public Dictionary<string, string> dependencies; // this type requires correctly handling ISerializationCallbackReceiver in Unity

        //fields not in UnityEditor.PackageManager.PackageInfo but in packages.json
        public string[] keywords;

        [UnityEngine.Tooltip("Optionally import parts of this package https://forum.unity.com/threads/samples-in-packages-manual-setup.623080/")]
        public Sample[] samples;
    }
}
