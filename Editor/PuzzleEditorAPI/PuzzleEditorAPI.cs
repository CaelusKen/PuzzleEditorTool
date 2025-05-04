using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class PuzzleEditorAPI
{
    private PuzzleData puzzleData;
    private SerializedObject serializedPuzzleData;

    public void LoadPuzzle(PuzzleData puzzle)
    {
        if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));

        puzzleData = puzzle;
        serializedPuzzleData = new SerializedObject(puzzleData);
        serializedPuzzleData.Update();
    }

    public PuzzleData GetPuzzleData()
    {
        return puzzleData;
    }

    public void SavePuzzle()
    {
        if (puzzleData == null) throw new InvalidOperationException("No puzzle loaded to save.");

        serializedPuzzleData.Update();
        serializedPuzzleData.ApplyModifiedProperties();
        EditorUtility.SetDirty(puzzleData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void AddStep()
    {
        EnsurePuzzleLoaded();

        var stepsProp = serializedPuzzleData.FindProperty("steps");
        Undo.RecordObject(puzzleData, "Add Puzzle Step");

        stepsProp.arraySize++;
        serializedPuzzleData.ApplyModifiedProperties();

        var newStep = stepsProp.GetArrayElementAtIndex(stepsProp.arraySize - 1);
        newStep.FindPropertyRelative("stepID").stringValue = Guid.NewGuid().ToString();

        serializedPuzzleData.ApplyModifiedProperties();
        MarkPuzzleDirty();
    }

    public void RemoveStep(int index)
    {
        EnsurePuzzleLoaded();

        var stepsProp = serializedPuzzleData.FindProperty("steps");
        if (index < 0 || index >= stepsProp.arraySize)
            throw new ArgumentOutOfRangeException(nameof(index));

        Undo.RecordObject(puzzleData, "Remove Puzzle Step");
        stepsProp.DeleteArrayElementAtIndex(index);
        serializedPuzzleData.ApplyModifiedProperties();
        MarkPuzzleDirty();
    }

    public void ReorderStep(int oldIndex, int newIndex)
    {
        EnsurePuzzleLoaded();

        var stepsProp = serializedPuzzleData.FindProperty("steps");
        if (oldIndex < 0 || oldIndex >= stepsProp.arraySize)
            throw new ArgumentOutOfRangeException(nameof(oldIndex));
        if (newIndex < 0 || newIndex >= stepsProp.arraySize)
            throw new ArgumentOutOfRangeException(nameof(newIndex));

        Undo.RecordObject(puzzleData, "Reorder Puzzle Steps");
        stepsProp.MoveArrayElement(oldIndex, newIndex);
        serializedPuzzleData.ApplyModifiedProperties();
        MarkPuzzleDirty();
    }

    public string[] ValidatePuzzle()
    {
        EnsurePuzzleLoaded();

        var errors = new List<string>();

        if (string.IsNullOrEmpty(puzzleData.puzzleID))
            errors.Add("Puzzle ID is empty.");

        if (string.IsNullOrEmpty(puzzleData.puzzleName))
            errors.Add("Puzzle Name is empty.");

        if (puzzleData.logicType == PuzzleLogicType.StepBased && puzzleData.steps != null)
        {
            var ids = new HashSet<string>();
            for (int i = 0; i < puzzleData.steps.Count; i++)
            {
                var step = puzzleData.steps[i];
                if (string.IsNullOrEmpty(step.stepID))
                    errors.Add($"Step {i + 1} has an empty Step ID.");
                else if (!ids.Add(step.stepID))
                    errors.Add($"Step {i + 1} has a duplicate Step ID.");
            }
        }

        return errors.ToArray();
    }

    public void ExportToJson(string filePath)
    {
        EnsurePuzzleLoaded();

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("Invalid file path for export.", nameof(filePath));

        string json = JsonUtility.ToJson(puzzleData, prettyPrint: true);
        File.WriteAllText(filePath, json);
    }

    public void ImportFromJson(string filePath)
    {
        EnsurePuzzleLoaded();

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new ArgumentException("Invalid or nonexistent file path for import.", nameof(filePath));

        string json = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, puzzleData);

        EditorUtility.SetDirty(puzzleData);
        serializedPuzzleData.Update();
    }

    public void MarkPuzzleDirty()
    {
        EditorUtility.SetDirty(puzzleData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void EnsurePuzzleLoaded()
    {
        if (puzzleData == null || serializedPuzzleData == null)
            throw new InvalidOperationException("No puzzle loaded.");
    }
}