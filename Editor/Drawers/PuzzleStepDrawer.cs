using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PuzzleStep))]
public class PuzzleStepDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 8 lines with spacing ~ singleLineHeight * 8 + spacer
        return EditorGUIUtility.singleLineHeight * 8 + 7 * 4;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4;
        Rect lineRect = new Rect(position.x, position.y, position.width, lineHeight);

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("stepID"), new GUIContent("Step ID"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("stepDescription"), new GUIContent("Step Description"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("requiredObjectID"), new GUIContent("Required Object ID"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("interactableID"), new GUIContent("Interactable ID"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("expectedType"), new GUIContent("Expected Type"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("isOptional"), new GUIContent("Is Optional"));
        lineRect.y += lineHeight + spacing;

        EditorGUI.PropertyField(lineRect, property.FindPropertyRelative("requiresSequence"), new GUIContent("Requires Sequence"));

        EditorGUI.EndProperty();
    }
}
