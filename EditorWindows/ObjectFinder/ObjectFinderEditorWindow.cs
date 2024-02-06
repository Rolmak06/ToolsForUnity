using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.BuiltIn;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectFinderEditorWindow : EditorWindow
{
    ObjectFinderConditionsEnum conditionEnum = ObjectFinderConditionsEnum.Name;
    public List<ObjectFinderCondition> conditions = new List<ObjectFinderCondition>();
    

    bool onlyActiveGameObjects;
    int toolbarInt;
    string[] toolbarOptions = {"Current Scene", "All Opened Scenes", "Project", "All"};

    Vector2 objectScrollView;
    Vector2 filtersScrollView;

    public List<GameObject> results = new List<GameObject>();
    SerializedProperty resultsProperty;
    SerializedObject so;

    private Dictionary<string, Type> scriptsList = new Dictionary<string, Type>();


    [MenuItem("Tools/Object Finder")]
    static void ShowWindow()
    {
        ObjectFinderEditorWindow window = (ObjectFinderEditorWindow)EditorWindow.GetWindow(typeof(ObjectFinderEditorWindow));
        window.titleContent = new GUIContent("Object Finder", "I'll find any gameobject in your project !");
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

        GUILayout.BeginVertical("FILTERS", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(350));
            filtersScrollView = EditorGUILayout.BeginScrollView(filtersScrollView);
                if(conditions.Count > 0)
                DrawFilters();
            EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("SEARCH", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(350));
            EditorGUILayout.Space(10);

            DrawButtons();

            EditorGUILayout.Space(10);

            DrawResultList();
        GUILayout.EndVertical();

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


    /// Buttons GUI
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


    /// Base Search Paramaters GUI 
    private void DrawConditionsManagement()
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarOptions);

        onlyActiveGameObjects = EditorGUILayout.Toggle("Active Objects only ?", onlyActiveGameObjects);
        
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
            conditionEnum = (ObjectFinderConditionsEnum)EditorGUILayout.EnumPopup(conditionEnum);

            if(GUILayout.Button("+ Add Filter"))
            {
                CreateNewCondition();
            }

            EditorGUILayout.LabelField("Filters : " + conditions.Count.ToString());
        EditorGUILayout.EndHorizontal();
    }


    /// User created filters GUI
    private void DrawFilters()
    {
        try
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if(conditions[i] is NameFinder)
                {
                    DrawNameFilter(conditions[i] as NameFinder);
                }

                if(conditions[i] is TagFinder)
                {
                    DrawTagFilter(conditions[i] as TagFinder);
                }

                if(conditions[i] is LayerFinder)
                {
                    DrawLayerFilter(conditions[i] as LayerFinder);
                }

                if(conditions[i] is ComponentFinder)
                {
                    DrawComponentFilter(conditions[i] as ComponentFinder);
                }

                if(conditions[i] is ShaderFinder)
                {
                    DrawShaderFilter(conditions[i] as ShaderFinder);
                }

                if(conditions[i] is MaterialFinder)
                {
                    DrawMaterialFilter(conditions[i] as MaterialFinder);
                }

                if(conditions[i] is DistanceFinder)
                {
                    DrawDistanceFilter(conditions[i] as DistanceFinder);
                }
            }
        }

        catch
        {
            //Try catch to avoid error when list is null while cycling through it
        }
        
    }
    
    #region Filters  Methods
    private void DrawNameFilter(NameFinder condition)
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

    private void DrawTagFilter(TagFinder condition)
    {
        GUILayout.BeginVertical("Tag Filter", "window");
        DrawRemoveButton(condition);

        condition.targetedTag = EditorGUILayout.TextField("Tag", condition.targetedTag);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private void DrawLayerFilter(LayerFinder condition)
    {
        GUILayout.BeginVertical("Layer Filter", "window");
        DrawRemoveButton(condition);

        condition.layer = EditorGUILayout.LayerField("Targeted Layer", condition.layer);

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private void DrawComponentFilter(ComponentFinder condition)
    {
        GUILayout.BeginVertical("Component Filter", "window");

        DrawRemoveButton(condition);

        GUILayout.BeginHorizontal();
            GUILayout.Label("Component :", GUILayout.MaxWidth(100));
            if(GUILayout.Button(condition.targetedType == null? "Selected Component" : condition.targetedType.Name, EditorStyles.popup, GUILayout.MinWidth(200)))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new ObjectFinderComponentProvider(scriptsList, (x) => condition.targetedType = x));
            }
        GUILayout.EndHorizontal();

        DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private void DrawShaderFilter(ShaderFinder condition)
    {
        GUILayout.BeginVertical("Shader Filter", "window");

            DrawRemoveButton(condition);

            condition.targetedShader = (Shader)EditorGUILayout.ObjectField(condition.targetedShader, typeof(Shader), true);

            DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private void DrawMaterialFilter(MaterialFinder condition)
    {
        GUILayout.BeginVertical("Material Filter", "window");

            DrawRemoveButton(condition);

            condition.targetedMaterial = (Material)EditorGUILayout.ObjectField(condition.targetedMaterial, typeof(Material), true);

            DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    private void DrawDistanceFilter(DistanceFinder condition)
    {

        GUILayout.BeginVertical("Distance Filter", "window");

            DrawRemoveButton(condition);

            condition.sourceObject = (Transform)EditorGUILayout.ObjectField(condition.sourceObject, typeof(Transform), true);

            EditorGUILayout.BeginHorizontal();
                condition.minRange = EditorGUILayout.FloatField("Min Distance:", condition.minRange, GUILayout.MaxWidth(200));
                EditorGUILayout.MinMaxSlider(ref condition.minRange, ref condition.maxRange, 0f, 1000f);
                condition.maxRange = EditorGUILayout.FloatField("Max Distance:", condition.maxRange, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();

            DrawExcludeToggle(condition);

        GUILayout.EndVertical();
    }

    #endregion Filters Drawing Methods

    private void DrawExcludeToggle(ObjectFinderCondition condition)
    {
        
        condition.exclude = EditorGUILayout.Toggle(new GUIContent("Exclude", "Do you want to use this filter as exclusion ?"), condition.exclude);
    }

    private void DrawRemoveButton(ObjectFinderCondition condition)
    {
        Rect rect = EditorGUILayout.BeginVertical();

            if (GUI.Button (new Rect (rect.width - 20, rect.position.y - 20, 20, 20), "X"))
            {
                conditions.Remove(condition);
            }

        EditorGUILayout.EndVertical();
    }

#endregion Drawing Methods

#region Tool Methods
    void Search()
    {
        results = Resources.FindObjectsOfTypeAll<GameObject>().ToList();

        for (int i = 0; i < conditions.Count; i++)
        {
            results = conditions[i].Process(results);
        }
        
        FilterByLocation();
        ActiveInHierarchyRestriction();
    }

    /// <summary>
    /// Select gameobjects depending on their location : current scene, opened scenes or in project
    /// </summary>
    private void FilterByLocation()
    {
        switch(toolbarInt)
        {
            case 0: //Current Scene
                results = results.Where(x => x.scene == SceneManager.GetActiveScene() && !AssetDatabase.Contains(x)).ToList();
            break;

            case 1: //All scenes
                results = results.Where(x => !AssetDatabase.Contains(x)).ToList();
            break;

            case 2: //Project
                results = results.Where(x => AssetDatabase.Contains(x)).ToList();
            break;

            case 3: //Everywhere
                //Do nothing, we keep all !
            break;
        }
    }

    /// <summary>
    /// Keep only active gameobjects
    /// </summary>
    private void ActiveInHierarchyRestriction()
    {
        List<GameObject> toDelete = new List<GameObject>();
        
        if (onlyActiveGameObjects)
        {
            results = results.Where(x => x.activeInHierarchy).ToList();
        }
    }

    /// <summary>
    /// Create a new condition according to the enumeration set by the user 
    /// </summary>
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

            case ObjectFinderConditionsEnum.Component:
                conditions.Add(new ComponentFinder());
                break;
        }
    }

    /// <summary>
    /// Populate a dictionnary with every components find in this project 
    /// </summary>
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


                    if(types[j].IsSubclassOf(typeof(Component)))
                    {
                        scriptsList.TryAdd(types[j].Name, types[j]);
                        Debug.Log($"Type {types[j].Name}, Type Name : {types[j].FullName}, Base Type : {types[j].BaseType}");                
                    }
                }
            }
    }

#endregion Tool Methods
}

