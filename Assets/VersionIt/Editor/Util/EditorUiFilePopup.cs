using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws a UnityEditor dropdown popup that lists files found at the rootPath that match the fileFilter supplied to the constructor
/// </summary>
public class EditorUiFilePopup
{
    /// <summary> Cached style to use to draw the popup button. </summary>
    private static readonly GUIStyle popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
    {
        imagePosition = ImagePosition.ImageOnly
    };

    private string[] popupOptions
    {
        get
        {
            if (_popupOptions == null || _popupOptions.Length == 0)
            {
                _popupOptions = pathFilter(rootPath, fileFilter);
            }
            return _popupOptions;
        }
        set
        {
            _popupOptions = value;
        }
    }
    private string[] _popupOptions;

    private string rootPath;
    private string fileFilter;
    private bool hasNone;

    /// <param name="rootPath"> example "/StreamingAssets/" </param>
    /// <param name="fileFilter"> example "*.json" </param>
    public EditorUiFilePopup(string rootPath, string fileFilter, bool hasNone = false)
    {
        this.rootPath = rootPath;
        this.fileFilter = fileFilter;
        this.hasNone = hasNone;
    }

    public void SetDirty()
    {
        // WISH: do this automatically right when EditorGUI.Popup is clicked
        _popupOptions = pathFilter(rootPath, fileFilter);
    }

    public void ChangeFilter(string newFileFilter)
    {
        this.fileFilter = newFileFilter;
    }

    /// <param name="position"> Space remaining to position the control </param>
    /// <param name="popupOptions"> options displayed in the popup </param>
    /// <returns> Rectangle on the screen leftover for remaining controls. </returns>
    public Rect Draw(Rect position, SerializedProperty filePathSelected)
    {
        // Handle "/"'s in popupOptions
        for (int i = 0; i < popupOptions.Length; i++)
        {
            popupOptions[i] = ConvertSlashToUnicodeSlash(popupOptions[i]);
        }

        // Calculate rect for configuration button
        Rect buttonRect = new Rect(position);
        buttonRect.yMin += popupStyle.margin.top;
        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;

        // Use a seperate indentLevel than whatever it is currently set to
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Draw & set user made selection
        string selectedValue = ConvertSlashToUnicodeSlash(filePathSelected.stringValue);
        int popupSelectedIndexOld = Mathf.Max(0, System.Array.IndexOf(popupOptions, selectedValue));
        int popupSelectedIndexNew = EditorGUI.Popup(buttonRect, popupSelectedIndexOld, popupOptions, popupStyle);
        filePathSelected.stringValue = RevertSlashToUnicodeSlash(popupOptions[popupSelectedIndexNew]);

        // Update position for next UI element
        EditorGUI.indentLevel = indent;
        position.xMin = buttonRect.xMax;
        return position;
    }

    private string[] pathFilter(string rootFolder, string filter)
    {
        string rootPath = Application.dataPath + rootFolder;

        // Find files matching filter
        IEnumerable<string> result = Enumerable.Empty<string>();
        if (Directory.Exists(rootPath))
        {
            var filteredFiles = Directory.GetFiles(rootPath, filter, SearchOption.AllDirectories);
            result = filteredFiles
                .Select(r => r.Substring(rootPath.Length).Replace('\\', '/')); // match slash type used by AssetDatabase
        }
        else
        {
            Debug.LogWarning("EditorUiFilePopup.cs expected the folder " + rootPath + " to exist to find files of type " + filter);
        }

        // Add optional "None"
        if (hasNone)
        {
            result = (new[] { "None" }).Concat(result);
        }

        return result.ToArray();
    }

    /// <summary>
    /// A normal '/' is parsed by the inspector to create a sub-menu. Use this function to avoid that parsing.
    /// </summary>
    public static string ConvertSlashToUnicodeSlash(string textWithSlashes)
    {
        return textWithSlashes.Replace('/', '\u2215');
    }

    /// <summary>
    /// A normal '/' is parsed by the inspector to create a sub-menu. Use this function to avoid that parsing.
    /// </summary>
    public static string RevertSlashToUnicodeSlash(string textWithSlashes)
    {
        return textWithSlashes.Replace('\u2215', '/');
    }
}
