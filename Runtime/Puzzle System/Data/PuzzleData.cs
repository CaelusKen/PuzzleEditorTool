using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Puzzle System/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    public string puzzleID;
    public string puzzleName;
    public PuzzleLogicType logicType;

    public List<PuzzleStep> steps = new(); // Used if logicType == StepBased
    public List<string> checklistItems = new(); // Used if logicType == Checklist

    public Outcome outcome;
}
