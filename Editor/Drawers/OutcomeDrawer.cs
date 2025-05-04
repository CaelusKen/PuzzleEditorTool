using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Outcome))]
public class OutcomePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var outcomeID = property.FindPropertyRelative("outcomeID");
        var outcomeName = property.FindPropertyRelative("outcomeName");
        var outcomeDescription = property.FindPropertyRelative("outcomeDescription");
        var unlocksPath = property.FindPropertyRelative("unlocksPath");
        var pathIDToUnlock = property.FindPropertyRelative("pathIDToUnlock");
        var givesReward = property.FindPropertyRelative("givesReward");
        var rewardItemID = property.FindPropertyRelative("rewardItemID");
        var endsPuzzle = property.FindPropertyRelative("endsPuzzle");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4;

        position.height = lineHeight;
        EditorGUI.PropertyField(position, outcomeID, new GUIContent("Outcome ID"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, outcomeName, new GUIContent("Outcome Name"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, outcomeDescription, new GUIContent("Outcome Description"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, unlocksPath, new GUIContent("Unlocks Path"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, pathIDToUnlock, new GUIContent("Path ID to Unlock"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, givesReward, new GUIContent("Gives Reward"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, rewardItemID, new GUIContent("Reward Item ID"));
        position.y += lineHeight + spacing;

        EditorGUI.PropertyField(position, endsPuzzle, new GUIContent("Ends Puzzle"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 8 + 4 * 7; // 8 lines + spacing
    }
}