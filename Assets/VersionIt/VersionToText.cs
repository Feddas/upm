using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: test if this works while in namespace Feddas
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class VersionToText : MonoBehaviour
{
    [Tooltip("searches the text contents for a matching field and replaces it with a version number")]
    public string VersionKey = "{v}";

    [Tooltip("Where the version number is pulled from")]
    public Feddas.PackageJson VersionObject;

    void Start()
    {
        var textObj = this.GetComponent<UnityEngine.UI.Text>();
        var version = VersionObject.Version.AsString;

        if (string.IsNullOrEmpty(VersionKey))
        {
            textObj.text = version;
        }
        else if (textObj.text.Contains(VersionKey))
        {
            textObj.text = textObj.text.Replace(VersionKey, version);
        }
        else
        {
            Debug.LogError(this.name + "'s VersionToText component isn't set up correctly. "
                + "The Text.text field must contain the versionKey value set on VersionToText. "
                + "An example setup is UnityEngine.UI.Text.text='my version is {v}' and VersionKey='{v}'");
        }
    }
}
