using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Readme))]
[InitializeOnLoad]
public class ReadmeEditor : Editor
{
    static ReadmeEditor()
    {
        EditorApplication.delayCall += SelectReadme;
    }

    static void SelectReadme()
    {
        EditorApplication.delayCall -= SelectReadme;

        var readmeObject = Resources.Load<Readme>("Readme");
        if (readmeObject != null)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = readmeObject;
        }
    }

    protected override void OnHeaderGUI()
    {
        Readme readme = (Readme)target;

        GUILayout.BeginHorizontal();

        var style = GUI.skin.GetStyle("label");
        style.fontSize = 26;
        GUILayout.Label(readme.Title, style);

        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        Readme readme = (Readme)target;

        foreach (string section in readme.Sections)
        {
            var style = GUI.skin.GetStyle("label");
            style.fontSize = 14;
            style.wordWrap = true;
            GUILayout.Label(section, style);

            GUILayout.Space(18);
        }
    }
}