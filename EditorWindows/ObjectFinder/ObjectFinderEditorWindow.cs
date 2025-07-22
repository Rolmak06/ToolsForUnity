using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ObjectFinderTool
{

    public class ObjectFinderEditorWindow : EditorWindow
    {
        FilterEnum filterEnum = FilterEnum.Name;
        public List<BaseFilter> conditions = new List<BaseFilter>();
        

        bool onlyActiveGameObjects;
        int toolbarInt;
        string[] toolbarOptions = {"Current Scene", "All Opened Scenes", "Project", "Child of selected", "All"};

        Vector2 objectScrollView;
        Vector2 filtersScrollView;

        public List<GameObject> results = new List<GameObject>();
        SerializedProperty resultsProperty;
        SerializedObject so;

        private Dictionary<string, Type> scriptsList = new Dictionary<string, Type>();


        [MenuItem("Tools/Object Finder Window")]
        static void ShowWindow()
        {
            ObjectFinderEditorWindow window = GetWindow<ObjectFinderEditorWindow>();
            window.titleContent = new GUIContent("Object Finder Window", "With this, you'll be able to find any gameobject in your project !");
            window.Show();
        }

        void OnEnable()
        {
            ScriptableObject target = this;
            so = new SerializedObject(target);
            resultsProperty = so.FindProperty("results");

            //Reference scripts from project
            ReferenceComponents();
        }

        void OnGUI()
        {   
            so.Update();

            DrawSearchParameters();

            EditorGUILayout.Space(10);

            GUILayout.BeginVertical("FILTERS", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(300));
                filtersScrollView = EditorGUILayout.BeginScrollView(filtersScrollView);
                    if(conditions.Count > 0)
                    DrawFilters();
                EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            EditorGUILayout.Space(10);

            GUILayout.BeginVertical("SEARCH", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(300));
                EditorGUILayout.Space(10);

                DrawButtons();

                EditorGUILayout.Space(10);

                DrawResultList();
            GUILayout.EndVertical();

            if(GUILayout.Button("Select results"))
            {
                Selection.objects = results.ToArray();
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
                    results = ObjectFinderEditor.Search(conditions.ToArray(), onlyActiveGameObjects, toolbarInt);
                }

                if (GUILayout.Button("X - Clear"))
                {
                    results.Clear();
                }
            EditorGUILayout.EndHorizontal();
        }


        /// Base Search Paramaters GUI 
        private void DrawSearchParameters()
        {
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarOptions);

            onlyActiveGameObjects = EditorGUILayout.Toggle("Active Objects only ?", onlyActiveGameObjects);
            
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
                filterEnum = (FilterEnum)EditorGUILayout.EnumPopup(filterEnum);

                if(GUILayout.Button("+ Add Filter"))
                {
                    CreateNewFilter();
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
                    if(conditions[i] is NameFilter)
                    {
                        DrawNameFilter(conditions[i] as NameFilter);
                    }

                    if(conditions[i] is TagFilter)
                    {
                        DrawTagFilter(conditions[i] as TagFilter);
                    }

                    if(conditions[i] is LayerFilter)
                    {
                        DrawLayerFilter(conditions[i] as LayerFilter);
                    }

                    if(conditions[i] is ComponentFilter)
                    {
                        DrawComponentFilter(conditions[i] as ComponentFilter);
                    }

                    if(conditions[i] is ShaderFilter)
                    {
                        DrawShaderFilter(conditions[i] as ShaderFilter);
                    }

                    if(conditions[i] is MaterialFilter)
                    {
                        DrawMaterialFilter(conditions[i] as MaterialFilter);
                    }

                    if(conditions[i] is DistanceFilter)
                    {
                        DrawDistanceFilter(conditions[i] as DistanceFilter);
                    }

                    if (conditions[i] is PropertyFilter)
                    {
                        DrawPropertyFilter(conditions[i] as PropertyFilter);
                    }
                }
            }

            catch
            {
                //Try catch to avoid error when list is null while cycling through it
            }
            
        }
        
#region Filters  Methods
        private void DrawNameFilter(NameFilter condition)
        {
            GUILayout.BeginVertical("Name Filter", "window");
            DrawRemoveButton(condition);

            EditorGUILayout.BeginHorizontal();
                bool contains;

                contains = GUILayout.Toggle(!condition.exact, new GUIContent("Contains", "Will match every gameObjects with a name that contains the search"), "Button");
                condition.exact = GUILayout.Toggle(!contains, new GUIContent("Exact", "Will match only gameObjects with a name that match exactly the search"), "Button");
                
                condition.caseSensitive = GUILayout.Toggle( condition.caseSensitive, new GUIContent("Case Sensitive", "Does casing is taking into account to find a match ?"), "Button");
            EditorGUILayout.EndHorizontal();
            
            condition.targetedName = EditorGUILayout.TextField("Name", condition.targetedName);

            DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawTagFilter(TagFilter condition)
        {
            GUILayout.BeginVertical("Tag Filter", "window");
            DrawRemoveButton(condition);

            condition.targetedTag = EditorGUILayout.TextField("Tag", condition.targetedTag);

            DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawLayerFilter(LayerFilter condition)
        {
            GUILayout.BeginVertical("Layer Filter", "window");
            DrawRemoveButton(condition);

            condition.layer = EditorGUILayout.LayerField("Targeted Layer", condition.layer);

            DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawComponentFilter(ComponentFilter condition)
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

        private void DrawShaderFilter(ShaderFilter condition)
        {
            GUILayout.BeginVertical("Shader Filter", "window");

                DrawRemoveButton(condition);

                condition.targetedShader = (Shader)EditorGUILayout.ObjectField("Targeted Shader", condition.targetedShader, typeof(Shader), true);

                DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawMaterialFilter(MaterialFilter condition)
        {
            GUILayout.BeginVertical("Material Filter", "window");

                DrawRemoveButton(condition);

                condition.targetedMaterial = (Material)EditorGUILayout.ObjectField("Targeted Material", condition.targetedMaterial, typeof(Material), true);

                DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawDistanceFilter(DistanceFilter condition)
        {

            GUILayout.BeginVertical("Distance Filter", "window");

                DrawRemoveButton(condition);

                condition.sourceObject = (Transform)EditorGUILayout.ObjectField("Source Transform", condition.sourceObject, typeof(Transform), true);

                EditorGUILayout.BeginHorizontal();
                    condition.minRange = EditorGUILayout.FloatField("Min Distance:", condition.minRange, GUILayout.MaxWidth(200));
                    EditorGUILayout.MinMaxSlider(ref condition.minRange, ref condition.maxRange, 0f, 1000f);
                    condition.maxRange = EditorGUILayout.FloatField("Max Distance:", condition.maxRange, GUILayout.MaxWidth(200));
                EditorGUILayout.EndHorizontal();

                DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }

        private void DrawPropertyFilter(PropertyFilter condition)
        {
            GUILayout.BeginVertical("Property Filter", "window");

            DrawRemoveButton(condition);

            // --- Component Type Selection (Using Search Window) ---
            GUILayout.BeginHorizontal();
            GUILayout.Label("Component:", GUILayout.MaxWidth(100));

            string buttonLabel = condition.componentType == null ? "Select Component" : condition.componentType.Name;
            if (GUILayout.Button(buttonLabel, EditorStyles.popup, GUILayout.MinWidth(200)))
            {
                // Open your existing search window
                SearchWindow.Open(
                    new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    new ObjectFinderComponentProvider(scriptsList, (selectedType) => {
                        condition.componentType = selectedType;
                        condition.targetedProperty = null; // Reset property on type change
                        condition.targetedValue = null;
                    })
                );
            }
            GUILayout.EndHorizontal();

            // --- Property and Value Drawing (only if type is selected) ---
            if (condition.componentType != null)
            {
                // 1. Property Selection
                var props = condition.componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> readableProps = new List<PropertyInfo>();
                foreach (var p in props)
                {
                    if (p.CanRead && p.GetIndexParameters().Length == 0)
                        readableProps.Add(p);
                }

                int currentPropIndex = Mathf.Max(0, readableProps.FindIndex(p => p.Name == condition.targetedProperty));
                string[] propNames = readableProps.ConvertAll(p => p.Name).ToArray();

                currentPropIndex = EditorGUILayout.Popup("Property", currentPropIndex, propNames);
                condition.targetedProperty = readableProps[currentPropIndex].Name;

                // 2. Value Input Based on Property Type
                var selectedProp = readableProps[currentPropIndex];
                condition.targetedValue = DrawValueField("Target Value", condition.targetedValue, selectedProp.PropertyType);
            }

            DrawExcludeToggle(condition);

            GUILayout.EndVertical();
        }




        #endregion Filters Drawing Methods

        private void DrawExcludeToggle(BaseFilter condition)
        {
            
            condition.exclude = EditorGUILayout.Toggle(new GUIContent("Exclude", "Do you want to use this filter as exclusion ?"), condition.exclude);
        }

        private void DrawRemoveButton(BaseFilter condition)
        {
            Rect rect = EditorGUILayout.BeginVertical();

                if (GUI.Button (new Rect (rect.width - 20, rect.position.y - 20, 20, 20), "X"))
                {
                    conditions.Remove(condition);
                }

            EditorGUILayout.EndVertical();
        }

#endregion Drawing Methods

#region Editor Window Methods

        /// Create a new condition according to the enumeration set by the user 
        private void CreateNewFilter()
        {
            switch (filterEnum)
            {
                case FilterEnum.Name:
                    conditions.Add(new NameFilter());
                    break;

                case FilterEnum.Tag:
                    conditions.Add(new TagFilter());
                    break;

                case FilterEnum.Layer:
                    conditions.Add(new LayerFilter());
                    break;

                case FilterEnum.Shader:
                    conditions.Add(new ShaderFilter());
                    break;

                case FilterEnum.Distance:
                    conditions.Add(new DistanceFilter());
                    break;

                case FilterEnum.Material:
                    conditions.Add(new MaterialFilter());
                    break;

                case FilterEnum.Component:
                    conditions.Add(new ComponentFilter());
                    break;

                case FilterEnum.Property:
                    conditions.Add(new PropertyFilter());
                    break;
            }
        }


        /// Populate a dictionnary with every components find in this project 
        private void ReferenceComponents()
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

        private List<Type> GetAllComponentTypes()
        {
            var componentTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }

                foreach (var type in types)
                {
                    if (typeof(Component).IsAssignableFrom(type) &&
                        !type.IsAbstract &&
                        type.IsPublic &&
                        type.IsClass)
                    {
                        componentTypes.Add(type);
                    }
                }
            }

            componentTypes.Sort((a, b) => a.Name.CompareTo(b.Name));
            return componentTypes;
        }

        private object DrawValueField(string label, object currentValue, Type type)
        {
            if (type == typeof(int))
            {
                return EditorGUILayout.IntField(label, currentValue is int i ? i : 0);
            }
            else if (type == typeof(float))
            {
                return EditorGUILayout.FloatField(label, currentValue is float f ? f : 0f);
            }
            else if (type == typeof(bool))
            {
                return EditorGUILayout.Toggle(label, currentValue is bool b && b);
            }
            else if (type == typeof(string))
            {
                return EditorGUILayout.TextField(label, currentValue as string ?? "");
            }
            else if (type.IsEnum)
            {
                // Make sure the enum is cast to the correct type
                if (currentValue == null || currentValue.GetType() != type)
                {
                    currentValue = Enum.GetValues(type).GetValue(0);
                }

                return EditorGUILayout.EnumPopup(label, (Enum)Enum.ToObject(type, currentValue));
            }

            else if (type == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(label, currentValue is Vector2 v ? v : Vector2.zero);
            }
            else if (type == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(label, currentValue is Vector3 v ? v : Vector3.zero);
            }
            else if (type == typeof(Color))
            {
                return EditorGUILayout.ColorField(label, currentValue is Color c ? c : Color.white);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return EditorGUILayout.ObjectField(label, currentValue as UnityEngine.Object, type, true);
            }

            EditorGUILayout.LabelField(label, $"Unsupported type: {type.Name}");
            return currentValue;
        }


        #endregion Editor Window Methods

    }
}

