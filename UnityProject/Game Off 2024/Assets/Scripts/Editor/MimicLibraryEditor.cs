using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;

[CustomEditor(typeof(MimicLibrary))]
public class MimicLibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MimicLibrary mimicLibrary = (MimicLibrary)target;

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Generate Mimics List Enum"))
        {
            GenerateMimicsEnum(mimicLibrary);
        }
        EditorGUILayout.Space(10);

        DrawDefaultInspector();
    }

    private void GenerateMimicsEnum(MimicLibrary library)
    {
        string enumName = "MimicsList";
        string filePath = Path.Combine(Application.dataPath, "Scripts", $"{enumName}.cs");

        // Create enum content
        StringBuilder enumContent = new StringBuilder();
        enumContent.AppendLine("// This file is auto-generated. Do not modify.");
        enumContent.AppendLine();
        enumContent.AppendLine("public enum MimicsList");
        enumContent.AppendLine("{");

        // Add None as first option
        enumContent.AppendLine("    None = 0,");

        // Add all mimic IDs
        var validIds = library.mimics
            .Where(m => !string.IsNullOrEmpty(m.id))
            .Select(m => m.id.Trim())
            .Distinct()
            .OrderBy(id => id);

        foreach (string id in validIds)
        {
            // Convert ID to valid enum name (replace spaces and special characters)
            string enumValue = System.Text.RegularExpressions.Regex.Replace(id, @"[^a-zA-Z0-9]", "_");
            if (char.IsDigit(enumValue[0])) // If starts with number, add prefix
            {
                enumValue = "_" + enumValue;
            }
            enumContent.AppendLine($"    {enumValue},");
        }

        enumContent.AppendLine("}");

        // Write to file
        try
        {
            File.WriteAllText(filePath, enumContent.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"Successfully generated {enumName} enum at {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to generate enum: {e.Message}");
        }
    }
} 