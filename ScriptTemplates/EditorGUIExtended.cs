using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

//Helper Class for any extra EditorGUI fields
public class EditorGUIExtended
{

    static List<string> layers;
    static string[] layerNames;

    public static LayerMask LayerMaskField(string label, LayerMask selected)
    {

        if (layers == null)
        {
            layers = new List<string>();
            layerNames = new string[4];
        }
        else
        {
            layers.Clear();
        }

        int emptyLayers = 0;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {

                for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                layers.Add(layerName);
            }
            else
            {
                emptyLayers++;
            }
        }

        if (layerNames.Length != layers.Count)
        {
            layerNames = new string[layers.Count];
        }
        for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];

        selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

        return selected;
    }

    public static int EnumPopupValues(string label, Enum selected)
    {
        Type type = selected.GetType();

        if (!type.IsEnum)
        {
            throw new Exception("parameter _enum must be of type System.Enum");
        }

        string[] names = Enum.GetValues(type).Cast<int>().Select(x => x.ToString()).ToArray();
        int[] values = Enum.GetValues(type) as int[];
        int index = Convert.ToInt32(selected);

        return EditorGUILayout.IntPopup("Face Size", index, names, values);
    }
}