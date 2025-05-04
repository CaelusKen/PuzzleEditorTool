# 🧩 Puzzle Editor Tool for Unity

A modular, extensible **Unity Editor extension** for designing logic-driven puzzles using **Step-based** or **Checklist-based** structures. Built for narrative games, adventure puzzles, and logic-based mechanics.

![License](https://img.shields.io/badge/license-MIT-green)
![Unity](https://img.shields.io/badge/unity-6000.0.34f1-blue)
![Version](https://img.shields.io/badge/version-0.0.1-blueviolet)

---

## ✨ Features

- 🎯 **Step-Based Puzzle Logic** – Define sequential puzzle steps with optional and required conditions.
- 📋 **Checklist Puzzle Logic** – Allow designers to set puzzles with unordered tasks.
- 🧠 **Outcome System** – Unlock paths, trigger events, give rewards, or end puzzles.
- ⚙️ **Custom Editor UI** – Built using Unity's `EditorWindow` and `PropertyDrawer` system.
- 🚨 **Validation & Tooltips** – Visual warnings for missing or invalid data.

---

## 📦 Installation

### ✅ Via Unity Package Manager

#### Option 1: Add Git URL

Add this line to your `manifest.json` file in the `Packages` folder of your Unity project:

```json
"com.yourname.puzzleeditor": "https://github.com/yourname/PuzzleEditorTool.git#v0.0.1"
```

#### Option 2: Through Unity Editor

- Open **Unity > Window > Package Manager**
- Click `+` → **Add package from Git URL…**
- Paste:
  ```
  https://github.com/CaelusKen/PuzzleEditorTool.git#v0.0.1
  ```

---

## 🧠 How It Works

The system uses:

- **PuzzleData** (ScriptableObject): stores puzzle logic, type, and steps.

- **PuzzleStep**: defines individual steps or checklist items.

- **Outcome**: handles puzzle result logic like rewards or unlocks.

- **PuzzleEditorWindow**: main interface for creating and editing puzzle flows.

- **Custom PropertyDrawers** for intuitive editing of data.

---

## 🗂 Folder Structure

```
PuzzleEditorTool/
├── package.json
├── README.md
├── LICENSE.md
├── CHANGELOG.md
└── Editor/
    ├── PuzzleEditor/       # Editor window logic and UI
    ├── PuzzleEditorAPI/    # Core logic and ScriptableObjects
    └── Drawers/            # Custom PropertyDrawers and utilities
```

---

## 🛠️ Requirements

- Unity **6000.0.34f1**
- Compatible with **URP**, **HDRP**, and **Built-in** render pipelines.

---

## 🚧 Roadmap

- [ ] Runtime puzzle preview system
- [ ] Built-in hint system support
- [ ] JSON/CSV import/export for localization
- [ ] Puzzle analytics hooks

---

## 🤝 Contributing

We welcome pull requests and community enhancements!  
Please open issues if you find bugs or have suggestions.

---

## 📝 License

Distributed under the **MIT License**.  
See [`LICENSE.md`](./LICENSE.md) for more details.

---

## 👤 Author

**[CaelusKen]** – [@CaelusKen](https://github.com/CaelusKen)  
Crafted with love for puzzle game developers.
