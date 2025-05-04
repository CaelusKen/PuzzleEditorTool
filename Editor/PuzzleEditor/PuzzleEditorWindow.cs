// PuzzleEditorWindow.cs
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

public class PuzzleEditorWindow : EditorWindow
{
    [MenuItem("Window/Puzzle Editor")]
    public static void ShowWindow()
    {
        GetWindow<PuzzleEditorWindow>("Puzzle Editor");
    }

    private PuzzleEditorAPI puzzleEditorAPI;
    private SerializedObject serializedPuzzleData;

    private VisualElement root;

    // Tab contents
    private VisualElement createPuzzleTab;
    private VisualElement loadPuzzleTab;
    private VisualElement metadataTab;
    private VisualElement stepsTab;
    private VisualElement outcomeTab;
    private VisualElement savePuzzleTab;

    private Label validationLabel;
    private List<int> filteredStepIndices = new List<int>();
    private string stepsSearch = "";
    private TextField stepsSearchField;

    // UI controls
    private ObjectField puzzleDataFieldCreate;
    private Button generatePuzzleButton;
    private Button resetPuzzleButton;

    private ObjectField puzzleDataFieldLoad;

    private Button saveButton;

    private Button btnCreatePuzzle;
    private Button btnLoadPuzzle;
    private Button btnMetadata;
    private Button btnSteps;
    private Button btnOutcome;
    private Button btnSave;

    private ScrollView contentScrollView; // scroll container

    // For the IMGUI reorderable list
    private ReorderableList stepsReorderableList;
    private IMGUIContainer stepsListContainer;

    public void CreateGUI()
    {
        puzzleEditorAPI = new PuzzleEditorAPI();

        root = rootVisualElement;
        root.style.paddingTop = 10;
        root.style.paddingLeft = 15;
        root.style.paddingRight = 15;
        root.style.paddingBottom = 10;
        root.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

        // Tab toolbar
        var tabToolbar = new VisualElement();
        tabToolbar.style.flexDirection = FlexDirection.Row;
        tabToolbar.style.justifyContent = Justify.SpaceAround;
        tabToolbar.style.marginBottom = 12;
        tabToolbar.style.backgroundColor = new Color(0.10f, 0.10f, 0.10f);
        tabToolbar.style.paddingTop = 6;
        tabToolbar.style.paddingBottom = 6;
        tabToolbar.style.borderBottomWidth = 1;
        tabToolbar.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
        root.Add(tabToolbar);

        btnCreatePuzzle = CreateTabButton("Create Puzzle");
        btnLoadPuzzle = CreateTabButton("Load Puzzle");
        btnMetadata = CreateTabButton("Puzzle Metadata");
        btnSteps = CreateTabButton("Puzzle Steps");
        btnOutcome = CreateTabButton("Puzzle Outcome");
        btnSave = CreateTabButton("Save Puzzle");

        tabToolbar.Add(btnCreatePuzzle);
        tabToolbar.Add(btnLoadPuzzle);
        tabToolbar.Add(btnMetadata);
        tabToolbar.Add(btnSteps);
        tabToolbar.Add(btnOutcome);
        tabToolbar.Add(btnSave);

        contentScrollView = new ScrollView();
        contentScrollView.style.flexGrow = 1;
        contentScrollView.style.backgroundColor = new Color(0.12f, 0.12f, 0.12f);
        contentScrollView.style.borderTopWidth = 1;
        contentScrollView.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f);
        root.Add(contentScrollView);

        createPuzzleTab = new VisualElement();
        loadPuzzleTab = new VisualElement();
        metadataTab = new VisualElement();
        stepsTab = new VisualElement();
        outcomeTab = new VisualElement();
        savePuzzleTab = new VisualElement();

        contentScrollView.Add(createPuzzleTab);
        contentScrollView.Add(loadPuzzleTab);
        contentScrollView.Add(metadataTab);
        contentScrollView.Add(stepsTab);
        contentScrollView.Add(outcomeTab);
        contentScrollView.Add(savePuzzleTab);

        InitializeCreatePuzzleTab();
        InitializeLoadPuzzleTab();
        InitializeMetadataTab();
        InitializeStepsTab();
        InitializeOutcomeTab();
        InitializeSavePuzzleTab();

        ShowTab(0);
        UpdateTabsInteractability();

        root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void SetPuzzleData(PuzzleData newPuzzle)
    {
        puzzleEditorAPI.LoadPuzzle(newPuzzle); // Use the API to load puzzle data
        serializedPuzzleData = new SerializedObject(newPuzzle); // Initialize serializedPuzzleData
        RefreshAllPropertyFields();
        UpdateTabsInteractability();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if ((evt.ctrlKey || evt.commandKey) && evt.keyCode == KeyCode.S)
        {
            evt.StopPropagation();
            SavePuzzle();
        }
    }

    private Button CreateTabButton(string text)
    {
        var btn = new Button(() =>
        {
            int index = GetTabIndex(text);
            ShowTab(index);
        })
        {
            text = text,
            tooltip = $"Switch to {text} tab"
        };

        btn.style.marginLeft = 6;
        btn.style.marginRight = 6;
        btn.style.paddingLeft = 10;
        btn.style.paddingRight = 10;
        btn.style.paddingTop = 4;
        btn.style.paddingBottom = 4;
        btn.style.unityFontStyleAndWeight = FontStyle.Bold;
        btn.style.fontSize = 13;
        btn.style.color = new Color(0.8f, 0.8f, 0.8f);
        btn.style.borderBottomWidth = 3;
        btn.style.borderBottomColor = UnityEngine.Color.clear;
        btn.style.unityTextAlign = TextAnchor.MiddleCenter;
        btn.RegisterCallback<MouseEnterEvent>((e) => btn.style.color = new Color(1f, 1f, 1f));
        btn.RegisterCallback<MouseLeaveEvent>((e) => btn.style.color = new Color(0.8f, 0.8f, 0.8f));

        return btn;
    }

    private int GetTabIndex(string tabName)
    {
        switch (tabName)
        {
            case "Create Puzzle": return 0;
            case "Load Puzzle": return 1;
            case "Puzzle Metadata": return 2;
            case "Puzzle Steps": return 3;
            case "Puzzle Outcome": return 4;
            case "Save Puzzle": return 5;
            default: return 0;
        }
    }

    private void HighlightTabButtonByIndex(int index)
    {
        btnCreatePuzzle.style.borderBottomColor = Color.clear;
        btnLoadPuzzle.style.borderBottomColor = Color.clear;
        btnMetadata.style.borderBottomColor = Color.clear;
        btnSteps.style.borderBottomColor = Color.clear;
        btnOutcome.style.borderBottomColor = Color.clear;
        btnSave.style.borderBottomColor = Color.clear;

        Color highlightColor = new Color(0.3f, 0.6f, 1f);

        switch (index)
        {
            case 0: btnCreatePuzzle.style.borderBottomColor = highlightColor; break;
            case 1: btnLoadPuzzle.style.borderBottomColor = highlightColor; break;
            case 2: btnMetadata.style.borderBottomColor = highlightColor; break;
            case 3: btnSteps.style.borderBottomColor = highlightColor; break;
            case 4: btnOutcome.style.borderBottomColor = highlightColor; break;
            case 5: btnSave.style.borderBottomColor = highlightColor; break;
        }
    }

    private void InitializeCreatePuzzleTab()
    {
        createPuzzleTab.Clear();

        puzzleDataFieldCreate = new ObjectField("Puzzle Data")
        {
            objectType = typeof(PuzzleData),
            allowSceneObjects = false,
            tooltip = "Select or create a PuzzleData asset"
        };
        puzzleDataFieldCreate.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == puzzleEditorAPI.GetPuzzleData())
                return;
            puzzleEditorAPI.LoadPuzzle(evt.newValue as PuzzleData);
            RefreshAllPropertyFields();
            UpdateTabsInteractability();
            if (puzzleDataFieldLoad != null)
                puzzleDataFieldLoad.SetValueWithoutNotify(puzzleEditorAPI.GetPuzzleData());
        });

        generatePuzzleButton = new Button(() =>
        {
            if (EditorUtility.DisplayDialog("Generate New Puzzle?", "Create and save a new Puzzle asset?", "Yes", "No"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save New Puzzle", "NewPuzzle.asset", "asset", "Enter file name");
                if (!string.IsNullOrEmpty(path))
                {
                    var newPuzzle = ScriptableObject.CreateInstance<PuzzleData>();
                    newPuzzle.puzzleID = System.Guid.NewGuid().ToString();
                    newPuzzle.puzzleName = "New Puzzle";
                    newPuzzle.logicType = PuzzleLogicType.StepBased;

                    AssetDatabase.CreateAsset(newPuzzle, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    puzzleEditorAPI.LoadPuzzle(newPuzzle);

                    puzzleDataFieldCreate.SetValueWithoutNotify(puzzleEditorAPI.GetPuzzleData());
                    if (puzzleDataFieldLoad != null)
                        puzzleDataFieldLoad.SetValueWithoutNotify(puzzleEditorAPI.GetPuzzleData());

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = newPuzzle;

                    ShowNotification(new GUIContent("New Puzzle created"));
                    RefreshAllPropertyFields();
                    UpdateTabsInteractability();
                }
            }
        })
        { text = "Generate New Puzzle" };
        generatePuzzleButton.tooltip = "Create a new PuzzleData Scriptable Object asset";

        resetPuzzleButton = new Button(() =>
        {
            if (EditorUtility.DisplayDialog("Reset Puzzle?", "Clear current puzzle selection?", "Yes", "No"))
            {
                puzzleEditorAPI.LoadPuzzle(null);
                puzzleDataFieldCreate.SetValueWithoutNotify(null);
                if (puzzleDataFieldLoad != null)
                    puzzleDataFieldLoad.SetValueWithoutNotify(null);
                RefreshAllPropertyFields();
                UpdateTabsInteractability();
            }
        })
        { text = "Reset Puzzle" };
        resetPuzzleButton.tooltip = "Clear current puzzle selection";

        var controls = new VisualElement();
        controls.style.flexDirection = FlexDirection.Row;
        controls.style.justifyContent = Justify.SpaceBetween;
        controls.style.marginTop = 8;
        controls.style.marginBottom = 10;

        controls.Add(generatePuzzleButton);
        controls.Add(resetPuzzleButton);

        createPuzzleTab.Add(puzzleDataFieldCreate);
        createPuzzleTab.Add(controls);
    }

    private void InitializeLoadPuzzleTab()
    {
        loadPuzzleTab.Clear();

        puzzleDataFieldLoad = new ObjectField("Load Puzzle Data")
        {
            objectType = typeof(PuzzleData),
            allowSceneObjects = false,
            tooltip = "Select a PuzzleData asset to load"
        };
        puzzleDataFieldLoad.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == puzzleEditorAPI.GetPuzzleData())
                return;
            puzzleEditorAPI.LoadPuzzle(evt.newValue as PuzzleData);
            RefreshAllPropertyFields();
            UpdateTabsInteractability();
            if (puzzleDataFieldCreate != null)
                puzzleDataFieldCreate.SetValueWithoutNotify(puzzleEditorAPI.GetPuzzleData());
        });

        loadPuzzleTab.Add(puzzleDataFieldLoad);
    }

    private void InitializeMetadataTab()
    {
        metadataTab.Clear();
        metadataTab.style.flexDirection = FlexDirection.Column;

        validationLabel = new Label();
        validationLabel.style.whiteSpace = WhiteSpace.Normal;
        validationLabel.style.color = Color.yellow;
        validationLabel.style.marginBottom = 8;
        metadataTab.Add(validationLabel);

        var puzzleData = puzzleEditorAPI.GetPuzzleData();

        if (puzzleData == null)
        {
            var label = new Label("No Puzzle Data loaded.");
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.color = new Color(0.7f, 0.7f, 0.7f);
            metadataTab.Add(label);
            return;
        }

        serializedPuzzleData = new SerializedObject(puzzleData);
        serializedPuzzleData.Update();

        var errors = puzzleEditorAPI.ValidatePuzzle();
        validationLabel.text = errors.Length > 0 ? "Validation Warnings:\n" + string.Join("\n", errors) : "";

        var header = new Label("Puzzle Metadata")
        {
            style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 16, marginBottom = 8, unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.9f, 0.9f, 0.9f) }
        };
        metadataTab.Add(header);

        var puzzleIDField = new PropertyField(serializedPuzzleData.FindProperty("puzzleID"), "Puzzle ID");
        puzzleIDField.Bind(serializedPuzzleData);
        metadataTab.Add(puzzleIDField);

        var puzzleNameField = new PropertyField(serializedPuzzleData.FindProperty("puzzleName"), "Puzzle Name");
        puzzleNameField.Bind(serializedPuzzleData);
        metadataTab.Add(puzzleNameField);

        var logicTypeField = new PropertyField(serializedPuzzleData.FindProperty("logicType"), "Logic Type");
        logicTypeField.Bind(serializedPuzzleData);
        metadataTab.Add(logicTypeField);
    }

    private void InitializeStepsTab()
    {
        stepsTab.Clear();
        stepsTab.style.flexDirection = FlexDirection.Column;

        var data = puzzleEditorAPI.GetPuzzleData();
        if (data == null || data.logicType != PuzzleLogicType.StepBased)
        {
            var label = new Label("Load a Puzzle Data with StepBased logic type to edit steps.");
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.color = new Color(0.7f, 0.7f, 0.7f);
            stepsTab.Add(label);
            return;
        }

        var header = new Label("Puzzle Steps")
        {
            style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 16, marginBottom = 8, unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.9f, 0.9f, 0.9f) }
        };
        stepsTab.Add(header);

        // Remove existing search field and IMGUIContainer if present
        if (stepsSearchField != null && stepsSearchField.parent == stepsTab)
            stepsTab.Remove(stepsSearchField);
        if (stepsListContainer != null && stepsListContainer.parent == stepsTab)
            stepsTab.Remove(stepsListContainer);

        // Create new search field
        stepsSearchField = new TextField("Search Steps");
        stepsSearchField.style.marginBottom = 8;
        stepsSearchField.SetValueWithoutNotify(stepsSearch);
        stepsSearchField.RegisterValueChangedCallback(evt =>
        {
            stepsSearch = evt.newValue.ToLower();
            UpdateFilteredStepIndices();
            if (stepsReorderableList != null)
                stepsListContainer.MarkDirtyRepaint();
        });
        stepsTab.Add(stepsSearchField);

        UpdateFilteredStepIndices();

        // Create or recreate IMGUIContainer for reorderable list
        stepsListContainer = new IMGUIContainer(() =>
        {
            if (puzzleEditorAPI.GetPuzzleData() == null)
                return;

            if (stepsReorderableList == null)
                SetupReorderableList();

            serializedPuzzleData.Update();
            stepsReorderableList.DoLayoutList();
            serializedPuzzleData.ApplyModifiedProperties();
        });
        stepsListContainer.style.flexGrow = 1;
        stepsListContainer.style.maxHeight = 400;
        stepsListContainer.style.marginTop = 5;
        stepsListContainer.style.marginBottom = 10;

        stepsTab.Add(stepsListContainer);

        // Add description label
        var note = new Label("Drag and drop steps to reorder them.");
        note.style.unityFontStyleAndWeight = FontStyle.Italic;
        note.style.fontSize = 11;
        note.style.color = new Color(0.7f, 0.7f, 0.7f);
        note.style.marginBottom = 8;
        stepsTab.Add(note);
    }

    private void UpdateFilteredStepIndices()
    {
        filteredStepIndices.Clear();
        var data = puzzleEditorAPI.GetPuzzleData();
        if (data == null || data.steps == null) return;

        for (int i = 0; i < data.steps.Count; i++)
        {
            var step = data.steps[i];
            if (string.IsNullOrEmpty(stepsSearch) ||
                (step.stepDescription != null && step.stepDescription.ToLower().Contains(stepsSearch)) ||
                (step.stepID != null && step.stepID.ToLower().Contains(stepsSearch)))
            {
                filteredStepIndices.Add(i);
            }
        }
    }

    private void SetupReorderableList()
    {
        var data = puzzleEditorAPI.GetPuzzleData();
        if (data == null) return;

        serializedPuzzleData = new SerializedObject(data);
        var stepsProp = serializedPuzzleData.FindProperty("steps");

        stepsReorderableList = new ReorderableList(serializedPuzzleData, stepsProp, true, true, true, true);

        stepsReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Puzzle Steps");

        stepsReorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = stepsProp.GetArrayElementAtIndex(index);

            bool matchesFilter = string.IsNullOrEmpty(stepsSearch) ||
                (element.FindPropertyRelative("stepDescription").stringValue?.ToLower().Contains(stepsSearch) ?? false) ||
                (element.FindPropertyRelative("stepID").stringValue?.ToLower().Contains(stepsSearch) ?? false);

            if (matchesFilter)
            {
                EditorGUI.PropertyField(rect, element, new GUIContent($"Step {index + 1}"), true);
            }
            else
            {
                // Draw empty to occupy no space for filtered out items
                EditorGUI.LabelField(rect, "");
            }
        };

        stepsReorderableList.elementHeightCallback = index =>
        {
            var element = stepsProp.GetArrayElementAtIndex(index);

            bool matchesFilter = string.IsNullOrEmpty(stepsSearch) ||
                (element.FindPropertyRelative("stepDescription").stringValue?.ToLower().Contains(stepsSearch) ?? false) ||
                (element.FindPropertyRelative("stepID").stringValue?.ToLower().Contains(stepsSearch) ?? false);

            return matchesFilter ? EditorGUI.GetPropertyHeight(element) + 8 : 0;
        };

        stepsReorderableList.onAddCallback = list =>
        {
            puzzleEditorAPI.AddStep();
            UpdateFilteredStepIndices();
            stepsListContainer.MarkDirtyRepaint();
        };

        stepsReorderableList.onRemoveCallback = list =>
        {
            puzzleEditorAPI.RemoveStep(list.index);
            UpdateFilteredStepIndices();
            stepsListContainer.MarkDirtyRepaint();
        };

        stepsReorderableList.onReorderCallback = list =>
        {
            Undo.RecordObject(data, "Reorder Puzzle Steps");
            serializedPuzzleData.ApplyModifiedProperties();
            UpdateFilteredStepIndices();
            puzzleEditorAPI.MarkPuzzleDirty();
            stepsListContainer.MarkDirtyRepaint();
        };
    }

    private void InitializeOutcomeTab()
    {
        outcomeTab.Clear();
        outcomeTab.style.flexDirection = FlexDirection.Column;

        var data = puzzleEditorAPI.GetPuzzleData();

        if (data == null)
        {
            var label = new Label("No Puzzle Data loaded.");
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.color = new Color(0.7f, 0.7f, 0.7f);
            outcomeTab.Add(label);
            return;
        }

        serializedPuzzleData = new SerializedObject(data);
        serializedPuzzleData.Update();

        var header = new Label("Puzzle Outcome")
        {
            style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 16, marginBottom = 8, unityTextAlign = TextAnchor.MiddleCenter, color = new Color(0.9f, 0.9f, 0.9f) }
        };
        outcomeTab.Add(header);

        var outcomeProp = serializedPuzzleData.FindProperty("outcome");
        if (outcomeProp != null)
        {
            var outcomeField = new PropertyField(outcomeProp, "Outcome");
            outcomeField.Bind(serializedPuzzleData);
            outcomeTab.Add(outcomeField);

            var outcomeContainer = new VisualElement();
            outcomeContainer.style.flexDirection = FlexDirection.Column;
            outcomeContainer.style.marginTop = 8;
            outcomeTab.Add(outcomeContainer);

            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("outcomeID"), "Outcome ID"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("outcomeName"), "Outcome Name"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("outcomeDescription"), "Outcome Description"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("unlocksPath"), "Unlocks Path"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("pathIDToUnlock"), "Path ID to Unlock"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("givesReward"), "Gives Reward"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("rewardItemID"), "Reward Item ID"));
            outcomeContainer.Add(new PropertyField(outcomeProp.FindPropertyRelative("endsPuzzle"), "Ends Puzzle"));
        }
        else
        {
            var label = new Label("Outcome property not found.");
            label.style.color = Color.red;
            outcomeTab.Add(label);
        }
    }

    private void InitializeSavePuzzleTab()
    {
        savePuzzleTab.Clear();

        saveButton = new Button(() => SavePuzzle())
        {
            text = "Save Puzzle"
        };
        saveButton.tooltip = "Save changes to the Puzzle asset";

        savePuzzleTab.Add(saveButton);

        var exportBtn = new Button(() =>
        {
            var data = puzzleEditorAPI.GetPuzzleData();
            if (data == null) return;

            string path = EditorUtility.SaveFilePanel("Export Puzzle Data to JSON", "", data.name + ".json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                puzzleEditorAPI.ExportToJson(path);
                EditorUtility.DisplayDialog("Export Complete", "Puzzle data exported to JSON.", "OK");
            }
        })
        { text = "Export to JSON" };

        var importBtn = new Button(() =>
        {
            var data = puzzleEditorAPI.GetPuzzleData();
            if (data == null) return;

            string path = EditorUtility.OpenFilePanel("Import Puzzle Data from JSON", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                puzzleEditorAPI.ImportFromJson(path);
                RefreshAllPropertyFields();
                EditorUtility.DisplayDialog("Import Complete", "Puzzle data imported from JSON.", "OK");
            }
        })
        { text = "Import from JSON" };

        savePuzzleTab.Add(exportBtn);
        savePuzzleTab.Add(importBtn);
    }

    private void SavePuzzle()
    {
        try
        {
            puzzleEditorAPI.SavePuzzle();
            EditorUtility.DisplayDialog("Puzzle Saved", $"Puzzle '{puzzleEditorAPI.GetPuzzleData().puzzleName}' saved successfully.", "Ok");
            RefreshAllPropertyFields();
        }
        catch (InvalidOperationException)
        {
            EditorUtility.DisplayDialog("No Puzzle Loaded", "Please load or create a puzzle before saving.", "Ok");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("Error Saving Puzzle", ex.Message, "Ok");
        }
    }

    private void ShowTab(int index)
    {
        createPuzzleTab.style.display = index == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        loadPuzzleTab.style.display = index == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        metadataTab.style.display = index == 2 ? DisplayStyle.Flex : DisplayStyle.None;
        stepsTab.style.display = index == 3 ? DisplayStyle.Flex : DisplayStyle.None;
        outcomeTab.style.display = index == 4 ? DisplayStyle.Flex : DisplayStyle.None;
        savePuzzleTab.style.display = index == 5 ? DisplayStyle.Flex : DisplayStyle.None;

        HighlightTabButtonByIndex(index);

        RefreshAllPropertyFields();
    }

    private void RefreshAllPropertyFields()
    {
        var data = puzzleEditorAPI.GetPuzzleData();
        if (data == null) return;
        serializedPuzzleData = new SerializedObject(data);
        serializedPuzzleData.Update();

        InitializeMetadataTab();
        InitializeStepsTab();
        InitializeOutcomeTab();
    }

    private void UpdateTabsInteractability()
    {
        bool puzzleLoaded = puzzleEditorAPI.GetPuzzleData() != null;

        btnMetadata.SetEnabled(puzzleLoaded);
        btnSteps.SetEnabled(puzzleLoaded);
        btnOutcome.SetEnabled(puzzleLoaded);
        btnSave.SetEnabled(puzzleLoaded);
    }
}