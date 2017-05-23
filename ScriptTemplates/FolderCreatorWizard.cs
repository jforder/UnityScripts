using System;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;

public class FolderCreatorWizard : ScriptableWizard
{
    
    public string parentFolderName = "New Folder";

    private static List<string> keys;
    private string path;

    private readonly Dictionary<string, bool> folders = new Dictionary<string, bool>()
    {
        {"Audio", false},
        {"Animation", false},
        {"Materials", false},
        {"Models", false},
        {"Prefabs", false},
        {"Stripts", false},
        {"Scenes", false},
        {"Shaders", false}
    };

    //Property
    private static string SelectedPath
    {
        get
        {
            if (Selection.activeObject != null)
            {
                string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if (AssetDatabase.IsValidFolder(selectedPath))
                {
                    return selectedPath;
                }
                else
                {
                    string fileName = Path.GetFileName(selectedPath);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        return selectedPath.Remove(selectedPath.Length - (fileName.Length + 1));
                    }
                }
            }

            return "";
        }
    }

    [MenuItem("Assets/Create/FolderStructure", false, 21)]
    public static bool CreateFolderStructure()
    {
        if (string.IsNullOrEmpty(SelectedPath))
        {
            Debug.LogWarning("Selected Path Returned Null or Empty - Make sure you have properly Selected a path");
            return false;
        }
        else
        {
            ScriptableWizard.DisplayWizard<FolderCreatorWizard>("Folder Structure Wizard");
        }

        return true;
    }

    private void OnEnable()
    {
        keys = new List<string>(folders.Keys);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Parent Folder Name");

        EditorGUILayout.Space();

        parentFolderName = EditorGUILayout.TextField("Name", parentFolderName);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sub Folders");

        EditorGUILayout.BeginVertical("box");
        foreach (string key in keys)
        {
            folders[key] = EditorGUILayout.Toggle(key, folders[key]);
        }
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();


        if (GUILayout.Button("Create Folders"))
        {

            if (string.IsNullOrEmpty(parentFolderName))
            {
                parentFolderName = "New Folder";
            }

            string parentPath = "";

            //Create Parent Folder
            if (!Directory.Exists(Application.dataPath + "/" + SelectedPath))
            {
                parentPath = AssetDatabase.CreateFolder(SelectedPath, parentFolderName);
                parentPath = AssetDatabase.GUIDToAssetPath(parentPath);
            }

            foreach (KeyValuePair<string, bool> value in folders)
            {
                if (value.Value)
                {
                    if (!Directory.Exists(Application.dataPath + "/" + parentPath))
                    {
                        AssetDatabase.CreateFolder(parentPath, value.Key);
                    }
                }
            }

            this.Close();
        }
    }

}
