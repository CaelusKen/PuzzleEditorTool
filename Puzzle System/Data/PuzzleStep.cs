using System;
using UnityEngine;

[Serializable]
public class PuzzleStep
{
    public string stepID = Guid.NewGuid().ToString(); // Unique ID per step
    public string stepDescription; // What this step does or implies

    public string requiredObjectID; // ID of item needed (e.g. key, item name, clue)
    public string interactableID;   // ID of the interactable to act upon

    public InteractableType expectedType; // Trigger, Key, Clue

    public bool isOptional = false; // If false, step must be completed to reach outcome
    public bool requiresSequence = true; // If false, can be done in any order
}
