using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using SO;

[CustomEditor(typeof(LevelSO))]
public class LevelParseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSO level = (LevelSO)target;
        using (new EditorGUI.DisabledScope(level.levelJson == null))
        {
            if (GUILayout.Button("Parse JSON Into Notes"))
                ParseJson(level);
        }
    }

    private static void ParseJson(LevelSO level)
    {
        try
        {
            Note[] parsedNotes = JsonConvert.DeserializeObject<Note[]>(level.levelJson.text);
            if (parsedNotes == null)
            {
                EditorUtility.DisplayDialog("Parse Failed", "The JSON did not contain a note array.", "OK");
                return;
            }

            Undo.RecordObject(level, "Parse Level JSON");
            level.notes = parsedNotes;
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
        }
        catch (JsonException exception)
        {
            Debug.LogException(exception, level);
            EditorUtility.DisplayDialog(
                "Parse Failed",
                $"Could not parse {level.levelJson.name}: {exception.Message}",
                "OK");
        }
    }
}
