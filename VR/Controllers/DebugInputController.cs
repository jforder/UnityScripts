using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

using System.Collections.Generic;


[System.Serializable]
public class DebugInputData
{
    public KeyCode activeKey;
    public UnityEvent uEvent;
}

public class DebugInputController : MonoBehaviour {

    public List<DebugInputData> inputs = new List<DebugInputData>();

    void Start()
    {
        DebugInputController[] controllersInScene = FindObjectsOfType<DebugInputController>();

        //go through all our inputs and check they are not being used else where - Might a bit overkill!
        for (int j = 0; j < this.inputs.Count; j++)
        {
            for (int i = 0; i < controllersInScene.Length; i++)
            {
                for (int k = 0; k < controllersInScene[i].inputs.Count; k++)
                {
                    if (inputs[j].activeKey == controllersInScene[i].inputs[k].activeKey)
                    {
                        Debug.LogWarning("Same KeyCode Used! - " + inputs[j].activeKey + " On " + controllersInScene[i].gameObject.name, controllersInScene[i].gameObject);
                    }
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {

	    for (int i = 0; i < inputs.Count; i++)
	    {
	        if (Input.GetKeyDown(inputs[i].activeKey))
	        {
	            inputs[i].uEvent.Invoke();
	        }
	    }
	}
}

[CustomEditor(typeof(DebugInputController))]
public class CustomEditorDebugInputContr : Editor
{
    private DebugInputController inputController;

    private void OnEnable()
    {
        inputController = (DebugInputController) target;
    }

    public override void OnInspectorGUI()
    {
        if (inputController != null)
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                DebugInputData data = new DebugInputData();
                
                inputController.inputs.Add(data);
            }

            if (GUILayout.Button("Remove"))
            {
                if(inputController.inputs.Count > 0)
                inputController.inputs.RemoveAt(inputController.inputs.Count - 1);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            for (int i = 0; i < inputController.inputs.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                var sObj = new SerializedObject(inputController);
                
                if (sObj != null)
                {
                    SerializedProperty list = sObj.FindProperty("inputs");
                    SerializedProperty listItem = list.GetArrayElementAtIndex(i);

                    sObj.Update();
                    EditorGUILayout.PropertyField(listItem.FindPropertyRelative("activeKey"), true);
                    EditorGUILayout.PropertyField(listItem.FindPropertyRelative("uEvent"), true);
                    sObj.ApplyModifiedProperties();

                    Repaint();
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}
