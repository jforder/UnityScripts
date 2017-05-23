using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEditor.ProjectWindowCallback;

public class ScriptTemplateExt
{
    private static readonly Dictionary<string, string> scriptTemplates = new Dictionary<string, string>()
    {
        {"EditorWindow", "101-C# Script-EditorWindowScript.cs.txt"},
        {"CustomEditor", "102-C# Script-CustomEditorScript.cs.txt"},
        {"EditorWizard", "103-C# Script-EditorWizardScript.cs.txt"}
    };

    [MenuItem("Assets/Create/EditorWindowScript")]
    public static void CreateEditorWindow()
    {
        string templatePath = GetTargetPath(scriptTemplates["EditorWindow"]);
        string rootpath = GetRootPath("NewEditorWindow");

        CreateScript(templatePath, rootpath);
    }

    [MenuItem("Assets/Create/CustomEditorScript")]
    public static void CreateCustomEditor()
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        else
        {
            string filePath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            Type type = AssetDatabase.GetMainAssetTypeAtPath(filePath);

            if (type != typeof(UnityEditor.MonoScript))
            {
                return;
            }

            MonoScript monoScript = (MonoScript)AssetDatabase.LoadMainAssetAtPath(filePath);

            if (!monoScript.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
            {
                return;
            }
            else
            {
                string fileName = Path.GetFileName(filePath).Replace(".cs", "");

                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                string templatePath = GetTargetPath(scriptTemplates["CustomEditor"]);
                string rootpath = GetRootPath("CustomEditor" + fileName);

                CreateScriptAssetFromTemplate(rootpath, fileName, templatePath);
            }
        }
    }

    [MenuItem("Assets/Create/EditorWizardScript")]
    public static void CreateWizard()
    {
        string templatePath = GetTargetPath(scriptTemplates["EditorWizard"]);
        string rootpath = GetRootPath("NewEditorWizard");

        CreateScript(templatePath, rootpath);
    }

    static void CreateScript(string templatePath, string rootpath)
    {
        var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAsset",
            BindingFlags.NonPublic | BindingFlags.Static);

        object[] paramObjects = new[] {templatePath, rootpath};

        method.Invoke(null, paramObjects);
    }


    private static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string editorTargetType,
        string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        string text = File.ReadAllText(resourceFile);
        text = text.Replace("#NOTRIM#", "");
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = text.Replace("#NAME#", fileNameWithoutExtension);
        string text2 = fileNameWithoutExtension.Replace(" ", "");
        text = text.Replace("#SCRIPTNAME#", text2);

        if (text.Contains("#TARGETNAME#"))
        {
            text = text.Replace("#TARGETNAME#", editorTargetType);
        }

        if (char.IsUpper(text2, 0))
        {
            text2 = char.ToLower(text2[0]) + text2.Substring(1);
            text = text.Replace("#SCRIPTNAME_LOWER#", text2);
        }
        else
        {
            text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
            text = text.Replace("#SCRIPTNAME_LOWER#", text2);
        }
        UTF8Encoding encoding = new UTF8Encoding(true);
        File.WriteAllText(fullPath, text, encoding);

        AssetDatabase.ImportAsset(pathName);

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));

        EditorGUIUtility.PingObject(obj);
       
        return obj;
    }

    private static string GetTargetPath(string templateFileName)
    {
        string path = Path.Combine(EditorApplication.applicationContentsPath,
            "Resources/ScriptTemplates/CustomTemplates");
        string result = Path.Combine(path, templateFileName);
        return result;
    }

    private static string GetRootPath(string scriptName)
    {
        if (Selection.activeObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                return selectedPath + "/" + scriptName + ".cs";
            }
            else
            {
                string fileName = Path.GetFileName(selectedPath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    return selectedPath.Remove(selectedPath.Length - (fileName.Length + 1)) + "/" + scriptName + ".cs";
                }
            }
        }

        return "";
    }
}