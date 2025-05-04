using System;
using UnityEngine;

[Serializable]
public class Outcome
{
    public string outcomeID = Guid.NewGuid().ToString(); // Unique ID for linking, if needed
    public string outcomeName; // Name of the outcome for display
    public string outcomeDescription;

    public bool unlocksPath = false;
    public string pathIDToUnlock; // ID of door/path that gets enabled

    public bool givesReward = false;
    public string rewardItemID; // Optional item as reward (like a treasure)

    public bool endsPuzzle = true; // Whether solving this outcome finishes the puzzle
}
