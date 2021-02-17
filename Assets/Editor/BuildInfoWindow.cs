using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BuildInfoWindow : EditorWindow {
    private bool needToBuild = false;
    // Scroll bars
    private Vector2 depScrollPos;
    private Vector2 includedAssetsScrollPos;
    private Vector2 unusedAssetsScrollPos;

    // Options for presentation
    private bool unusedAssetsVisible = true;
    private bool usedAssetsVisible = true;
    private bool dependenciesVisible = false;
    private bool adaptToSelection = true;
    private UnityEngine.Object[] selectionInProjectView;

    private BuildLogParser buildLogParser = new BuildLogParser();
    private List<AssetData> unusedAssets = new List<AssetData>();

    [MenuItem("Window/Build Info")]
    static void Init() {
        var window = (BuildInfoWindow)EditorWindow.GetWindow(typeof(BuildInfoWindow));
        window.Show();
        // Update selection for filter
        window.OnSelectionChange();
    }

    void OnSelectionChange() {
        selectionInProjectView = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        Repaint();
    }

    void OnGUI() {
        if (needToBuild) {
            GUI.color = Color.red;
            GUILayout.Label("No build info could be found.\nAre you sure you already built the project?", EditorStyles.boldLabel);
        }
        GUI.color = Color.white;

        // Show update button
        if (GUILayout.Button("Update Build Info")) {
            LoadBuildInfo();
        }

        // Show options for presentation
        EditorGUILayout.BeginHorizontal();
        unusedAssetsVisible = GUILayout.Toggle(unusedAssetsVisible, "Unused assets");
        usedAssetsVisible = GUILayout.Toggle(usedAssetsVisible, "Used assets");
        dependenciesVisible = GUILayout.Toggle(dependenciesVisible, "Dependencies");
        adaptToSelection = GUILayout.Toggle(adaptToSelection, "Filter by selection in inspector");
        EditorGUILayout.EndHorizontal();

        if (!needToBuild) {
            EditorGUILayout.BeginHorizontal();
            // Show unused assets
            if (unusedAssetsVisible) {
                ShowAssets(unusedAssets, "UNUSED ASSETS", ref unusedAssetsScrollPos);
            }
            // Show included assets
            if (usedAssetsVisible) {
                ShowAssets(buildLogParser.includedAssets, "INCLUDED ASSETS", ref includedAssetsScrollPos);
            }
            // Show included libraries
            if (dependenciesVisible) {
                ShowDependenciesa();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ShowAssets(List<AssetData> assets, string title, ref Vector2 scrollPos) {
        EditorGUILayout.BeginVertical();
        GUILayout.Label(title.ToUpper() + " (" + assets.Count + ")");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var asset in assets) {
            // Only include assets that are a child (inside some folder) from the selection in the project view
            if (!adaptToSelection || IsChildOfSelection(asset)) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(asset.obj, typeof(UnityEngine.Object), false);
                if(asset.byteSize != null || asset.perCentSize != null) {
                    GUILayout.Label(asset.byteSize+" "+ asset.perCentSize, EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void ShowDependenciesa() {
        EditorGUILayout.BeginVertical();
        depScrollPos = EditorGUILayout.BeginScrollView(depScrollPos);
        GUILayout.Label("INCLUDED DEPENDENCIES" + " (" + buildLogParser.includedDependencies.Count + ")");
        for (int i = 0; i < buildLogParser.includedDependencies.Count; i++) {
            EditorGUILayout.TextField(buildLogParser.includedDependencies[i]);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private bool IsChildOfSelection(AssetData asset) {
        if (selectionInProjectView == null || selectionInProjectView.Length == 0) {
            return true;
        }
        foreach (UnityEngine.Object o in selectionInProjectView) {
            var path = AssetDatabase.GetAssetPath(o);
            if (asset.path.Contains(path)) {
                return true;
            }
        }
        return false;
    }

    private void LoadBuildInfo() {
        buildLogParser.Update();

        if (buildLogParser.includedAssets.Count == 0) {
            needToBuild = true;
        } else {
            needToBuild = false;
            UpdateUnusedAssets();
        }
    }

    private void UpdateUnusedAssets() {
        unusedAssets.Clear();
        var allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in allAssetPaths) {
            // Skip folders
            var isFile = System.IO.File.Exists(path);
            if(isFile) {
                if (!buildLogParser.includedAssets.Exists(a => a.path == path)) {
                    unusedAssets.Add(new AssetData(path));
                }
            }
        }
    }
}