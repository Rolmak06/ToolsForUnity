using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectFinderEditorWindow : EditorWindow
{
    ObjectFinderConditionsEnum conditionEnum = ObjectFinderConditionsEnum.Name;
    public List<ObjectFinderCondition> conditions = new List<ObjectFinderCondition>();
    private Dictionary<string, Type> scriptsList = new Dictionary<string, Type>();
    string[] scriptListContent = new string[]{"Empty"};

    bool currentScene = true;
    bool onlyActiveGameObjects;
    bool openedScenes;
    bool inProject;

    Vector2 objectScrollView;
    bool needRefresh;

    public List<GameObject> results = new List<GameObject>();
    SerializedProperty resultsProperty;
    SerializedObject so;

    [MenuItem("Tools/Louis/ObjectFinderPlus")]
    static void ShowWindow()
    {
        ObjectFinderEditorWindow window = (ObjectFinderEditorWindow)EditorWindow.GetWindow(typeof(ObjectFinderEditorWindow));
        window.Show();
    }

    void OnEnable()
    {
        ScriptableObject target = this;
        so = new SerializedObject(target);
        resultsProperty = so.FindProperty("results");

        //Reference scripts from project
        ReferenceScripts();
    }

    void OnGUI()
    {   
        EditorGUI.BeginChangeCheck();

        so.Update();

        DrawConditionsManagement();

        EditorGUILayout.Space(10);

        if(conditions.Count > 0)
        DrawConditions();

        EditorGUILayout.Space(10);

        DrawButtons();

        EditorGUILayout.Space(10);

        DrawResultList();

        if(EditorGUI.EndChangeCheck())
        {
        }

    }

    #region Drawing Methods
    private void DrawResultList()
    {
        if (results.Count > 0)
        {
            objectScrollView = EditorGUILayout.BeginScrollView(objectScrollView);
            EditorGUILayout.PropertyField(resultsProperty);
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Search GameObjects"))
        {
            Search();
        }

        if (GUILayout.Button("X - Clear"))
        {
            results.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawConditionsManagement()
    {
        EditorGUILayout.BeginHorizontal();

            conditionEnum = (ObjectFinderConditionsEnum)EditorGUILayout.EnumPopup(conditionEnum);

            if(GUILayout.Button("+ Add Condition"))
            {
                CreateNewCondition();
            }

            EditorGUILayout.LabelField("Conditions : " + conditions.Count.ToString());

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        GUILayout.BeginHorizontal();
            currentScene = EditorGUILayout.Toggle("Current Scene ?", currentScene);
            openedScenes = EditorGUILayout.Toggle("All opened scenes ?", openedScenes);
            onlyActiveGameObjects = EditorGUILayout.Toggle("Only active gameObjects?", onlyActiveGameObjects);
            inProject = EditorGUILayout.Toggle("Project assets ?", inProject);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawConditions()
    {
        try
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

        catch
        {
            //Try catch to avoid error when list is null while cycling through it
        }
        
    }
    
    void DrawNameCondition(NameFinder condition)
    {
        GUILayout.BeginVertical("Name Filter", "window");
        DrawRemoveButton(condition);

        EditorGUILayout.BeginHorizontal();
            bool contains;

            contains = GUILayout.Toggle(!condition.exact, "Contains", "Button");
            condition.exact = GUILayout.Toggle(!contains,"Exact", "Button");
            
            condition.caseSensitive = GUILayout.Toggle( condition.caseSensitive, "Case Sensitive", "Button");
        EditorGUILayout.EndHorizontal();
        
        condition.targetedName = EditorGUILayout.TextField("Name", condition.targetedName);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawTagCondition(TagFinder condition)
    {
        GUILayout.BeginVertical("Tag Filter", "window");
        DrawRemoveButton(condition);

        condition.targetedTag = EditorGUILayout.TextField("Tag", condition.targetedTag);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawLayerCondition(LayerFinder condition)
    {
        GUILayout.BeginVertical("Layer Filter", "window");
        DrawRemoveButton(condition);

        condition.layer = EditorGUILayout.LayerField("Targeted Layer", condition.layer);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawScriptCondition(ScriptFinder condition)
    {
        GUILayout.BeginVertical("Script Filter", "window");

        DrawRemoveButton(condition);

        if(GUILayout.Button(condition.targetedType == null? "Selected Type" : condition.targetedType.Name, EditorStyles.popup, GUILayout.MinWidth(200)))
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectFinderComponentProvider(scriptsList, (x) => condition.targetedType = x));
        }

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawShaderCondition(ShaderFinder condition)
    {
        GUILayout.BeginVertical("Shader Filter", "window");

        DrawRemoveButton(condition);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawMaterialCondition(MaterialFinder condition)
    {
        GUILayout.BeginVertical("Material Filter", "window");

        DrawRemoveButton(condition);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    void DrawDistanceCondition(DistanceFinder condition)
    {

        GUILayout.BeginVertical("Distance Filter", "window");

        DrawRemoveButton(condition);

        EditorGUILayout.MinMaxSlider(ref condition.minRange, ref condition.maxRange, 0f, Mathf.Infinity);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private static void DrawExcludeToggle(ObjectFinderCondition condition)
    {
        
        condition.exclude = EditorGUILayout.Toggle(new GUIContent("Exclude", "Do you want to use this filter as exclusion ?"), condition.exclude);
    }

    void DrawRemoveButton(ObjectFinderCondition condition)
    {
        Rect rect = EditorGUILayout.BeginVertical();

        if (GUI.Button (new Rect (rect.width - 20, rect.position.y - 20, 20, 20), "X"))
        {
            conditions.Remove(condition);
        }

        EditorGUILayout.EndVertical();
    }

#endregion Drawing Methods
    void Search()
    {
        results = Resources.FindObjectsOfTypeAll<GameObject>().ToList();

        for (int i = 0; i < conditions.Count; i++)
        {
            results = conditions[i].Process(results);
        }

        RestrictToActiveObject();
        RestrictToActiveScene();
        RestrictToPrefab();
    }

    private void RestrictToActiveObject()
    {
        List<GameObject> toDelete = new List<GameObject>();
        
        if (onlyActiveGameObjects)
        {
            foreach (GameObject go in results)
            {
                if (!go.activeInHierarchy)
                {
                    toDelete.Add(go);
                }
            }

            foreach(GameObject go in toDelete)
            {
                results.Remove(go);
            }
        }
    }
    private void RestrictToActiveScene()
    {
        if(!openedScenes)
        {
            List<GameObject> toDelete = new List<GameObject>();
            
            foreach (GameObject obj in results)
            {
                if (obj.scene != SceneManager.GetActiveScene())
                {
                    toDelete.Add(obj);
                }
            }
            
            foreach(GameObject obj in toDelete)
            {
                results.Remove(obj);
            }
        }
    }
    private void RestrictToPrefab()
    {
        List<GameObject> toDelete = new List<GameObject>(results);

        if(inProject && !openedScenes && !currentScene)
        {   
            
            foreach (GameObject obj in results)
            {
                if (AssetDatabase.Contains(obj))
                {
                    toDelete.Remove(obj);
                }
            }
        }

        if(!inProject)
        {
            toDelete.Clear();

            foreach (GameObject obj in results)
            {
                if (AssetDatabase.Contains(obj))
                {
                    toDelete.Add(obj);
                }
            }
        }

        foreach(GameObject obj in toDelete)
        {
            results.Remove(obj);
        }
    }

    private void CreateNewCondition()
    {
        switch (conditionEnum)
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

    private void ReferenceScripts()
    {
            scriptsList.Clear();

            Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            for(int i = 0; i < referencedAssemblies.Length; ++i)
            {
                Type[] types = referencedAssemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {   
                    if(types[j].Name.Contains("CameraFilterPack_"))
                    {
                        continue;
                    }


                    if(types[j].IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        scriptsList.TryAdd(types[j].Name, types[j]);
                        Debug.Log($"Type {types[j].Name}, Type Name : {types[j].FullName}, Base Type : {types[j].BaseType}");                
                    }
                }
            }

            scriptListContent = scriptsList.Keys.ToArray();
    }
}

