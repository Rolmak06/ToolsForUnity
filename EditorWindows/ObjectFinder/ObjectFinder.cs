using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObjectFinder : EditorWindow
{
    ObjectFinderConditionsEnum conditionEnum = ObjectFinderConditionsEnum.Name;
    public List<ObjectFinderCondition> conditions = new List<ObjectFinderCondition>();
    private Dictionary<string, Type> scriptsList = new Dictionary<string, Type>();

    [MenuItem("Tools/Louis/ObjectFinderPlus")]
    static void ShowWindow()
    {
        ObjectFinder window = (ObjectFinder)EditorWindow.GetWindow(typeof(ObjectFinder));
        window.Show();
    }

    void OnGUI()
    {
        ManageConditions();
        DrawConditions();
    }

    private void ManageConditions()
    {
        conditionEnum = (ObjectFinderConditionsEnum)EditorGUILayout.EnumPopup(conditionEnum);

        if(GUILayout.Button("Add Condition"))
        {
            switch(conditionEnum)
            {
                case ObjectFinderConditionsEnum.Name:
                conditions.Add(new NameFinder());
                break;

                case ObjectFinderConditionsEnum.Tag:
                conditions.Add(new TagFinder());
                break;

                case ObjectFinderConditionsEnum.Layer:
                conditions.Add(new LayerFinder());
                break;

                case ObjectFinderConditionsEnum.Shader:
                conditions.Add(new ShaderFinder());
                break;

                case ObjectFinderConditionsEnum.Distance:
                conditions.Add(new DistanceFinder());
                break;

                case ObjectFinderConditionsEnum.Material:
                conditions.Add(new MaterialFinder());
                break;

                case ObjectFinderConditionsEnum.Script:
                conditions.Add(new ScriptFinder());
                break;
            }
        }
    }

    private void DrawConditions()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if(conditions[i] is NameFinder)
            {
                DrawNameCondition(conditions[i] as NameFinder);
            }

            if(conditions[i] is TagFinder)
            {
                DrawTagCondition(conditions[i] as TagFinder);
            }

            if(conditions[i] is LayerFinder)
            {
                DrawLayerCondition(conditions[i] as LayerFinder);
            }

            if(conditions[i] is ScriptFinder)
            {
                DrawScriptCondition(conditions[i] as ScriptFinder);
            }

            if(conditions[i] is ShaderFinder)
            {
                DrawShaderCondition(conditions[i] as ShaderFinder);
            }

            if(conditions[i] is MaterialFinder)
            {
                DrawMaterialCondition(conditions[i] as MaterialFinder);
            }

            if(conditions[i] is DistanceFinder)
            {
                DrawDistanceCondition(conditions[i] as DistanceFinder);
            }
        }
    }
    

    void DrawNameCondition(NameFinder condition)
    {
        condition.targetedName = EditorGUILayout.TextField(condition.targetedName);
    }

    void DrawTagCondition(TagFinder condition)
    {
        condition.targetedTag = EditorGUILayout.TextField(condition.targetedTag);
    }

    void DrawLayerCondition(LayerFinder condition)
    {
        condition.layer = EditorGUILayout.LayerField(condition.layer);
    }

    void DrawScriptCondition(ScriptFinder condition)
    {
        if(GUILayout.Button(condition.targetedType == null? "Selected Type" : condition.targetedType.Name, EditorStyles.popup, GUILayout.MinWidth(200)))
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectFinderComponentProvider(scriptsList, (x) => condition.targetedType = x));
        }
    }

    void DrawShaderCondition(ShaderFinder condition)
    {
        //condition.targetedShader = EditorGUILayout.ObjectField(condition.targetedShader);
    }

    void DrawMaterialCondition(MaterialFinder condition)
    {

    }

    void DrawDistanceCondition(DistanceFinder condition)
    {
        EditorGUILayout.MinMaxSlider(ref condition.minRange, ref condition.maxRange, 0f, Mathf.Infinity);
    }
}
