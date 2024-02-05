using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class CustomPackageExporterEditor : EditorWindow
{
    public ExporterListSo exportList;
    string exportListDestinationPath;
    public string packageName = "package";
    string destinationPath; 
    public List<Object> assetsToExport = new List<Object>();
    public List<Object> dependencies = new List<Object>();
    public List<string> exportedPackageAssetList = new List<string>();

    private SerializedObject so;

    SerializedProperty assetsProperty;
    SerializedProperty exportListProperty;
    SerializedProperty dependenciesListProperty;
    Vector2 exportAssetsScrollView;
    Vector2 dependenciesAssetsScrollView;


    [SerializeField, Tooltip("In addition to the assets paths listed, all dependent assets will be included as well.")]
    bool includeDependencies;


    [MenuItem("Assets/Custom Package Exporter")]
    static void ShowWindow()
    {
        CustomPackageExporterEditor window = (CustomPackageExporterEditor)EditorWindow.GetWindow(typeof(CustomPackageExporterEditor));
        window.titleContent = new GUIContent("Custom Package Export");
        window.minSize = new Vector2(200,200);
        window.Show();
    }
    void OnEnable()
    {
        ScriptableObject target = this;
        so = new SerializedObject(target);
        assetsProperty = so.FindProperty("assetsToExport");
        exportListProperty = so.FindProperty("exportList");
        dependenciesListProperty = so.FindProperty("dependencies");
    }

    void OnGUI()
    {
        so.Update();

        EditorGUILayout.Space(5);

        DrawPackageName();

        EditorGUILayout.Space(5);

        DrawExportScriptableObject();

        EditorGUILayout.Space(5);

        DrawAssetList();

        DrawDependenciesList();

        //DrawRecursiveAssetsList();

        EditorGUILayout.Space(5);

        includeDependencies = GUILayout.Toggle(includeDependencies, new GUIContent("Include Dependencies ?", "In addition to the assets paths listed, all dependent assets will be included as well."));

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Export Package"))
        {
            ExportPackage();
        }

        so.ApplyModifiedProperties();
    }

    private void DrawAssetList()
    {
        EditorGUILayout.LabelField("Assets to export", EditorStyles.largeLabel);

        exportAssetsScrollView = EditorGUILayout.BeginScrollView(exportAssetsScrollView);

        EditorGUILayout.PropertyField(assetsProperty);

        EditorGUILayout.EndScrollView();
    }
    private void DrawDependenciesList()
    {
        
        if(includeDependencies)
        {
            EditorGUILayout.LabelField("Dependencies", EditorStyles.largeLabel);

            if(GUILayout.Button("Show dependencies"))
            {
                ShowDependencies();
            }

            dependenciesAssetsScrollView = EditorGUILayout.BeginScrollView(dependenciesAssetsScrollView);

            EditorGUILayout.PropertyField(dependenciesListProperty);

            EditorGUILayout.EndScrollView();
        }
    }
    private void DrawPackageName()
    {
        packageName = EditorGUILayout.TextField("Package Name", packageName);
    }
    private void DrawExportScriptableObject()
    {
        
        EditorGUILayout.PropertyField(exportListProperty);
        EditorGUILayout.HelpBox("Export List is optional. They allow you to quickly get objects to export. As they are scriptable objects, they can be setup in the editor and then, be used here to fasten your workflow.", MessageType.Info);
        
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create List"))
        {
            CreateExporterList();
        }

        if (GUILayout.Button("Import from List"))
        {
            if(exportList == null) 
            {
                EditorGUILayout.HelpBox("No export list selected", MessageType.Warning);
            }
            else
            {
                packageName = exportList.packageName;
                assetsToExport = new List<Object>(exportList.objects);
            }
            
        }

        if (GUILayout.Button("Save to List"))
        {
            if(exportList == null) 
            {
                CreateExporterList();

                if(exportList != null)
                {
                    exportList.packageName = packageName;
                    exportList.objects = new List<Object>(assetsToExport);
                }
                
            }
            else
            {
                exportList.packageName = packageName;
                exportList.objects = new List<Object>(assetsToExport);
            }
        }

        EditorGUILayout.EndHorizontal();
    }
    void SetPackageList()
    {
        exportedPackageAssetList.Clear();

        foreach(Object obj in assetsToExport)
        {
            if(obj == null) {continue;}

            exportedPackageAssetList.Add(AssetDatabase.GetAssetPath(obj));
        }
    }
    void ExportPackage()
    {
        SetPackageList();

        destinationPath = EditorUtility.OpenFolderPanel("Export Destination", "", "");

        ExportPackageOptions options = CreateExportOptions();

        AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), $"{destinationPath}/{packageName}.unitypackage", options);
    }
    

    void ShowDependencies()
    {
        dependencies.Clear();
        dependencies = GetDependenciesAssets();
    }


    private ExportPackageOptions CreateExportOptions()
    {
        ExportPackageOptions options = ExportPackageOptions.Interactive | ExportPackageOptions.Recurse;

        if(includeDependencies)
        {
            options |= ExportPackageOptions.IncludeDependencies;
        }
        
        return options;
    }


    private List<Object> GetDependenciesAssets()
    {
        List<Object> dependencies = new List<Object>();

        foreach(Object obj in assetsToExport)
        {
           string[] dependenciesPath = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(obj));

           foreach(string dependency in dependenciesPath)
           {
                Object dependencyObj = AssetDatabase.LoadAssetAtPath(dependency, typeof(Object));

                //Ignore object if already in the asset list
                if(assetsToExport.Contains(dependencyObj))
                {
                    continue;
                }

                if(dependencies.Contains(dependencyObj))
                {
                    continue;
                }

                dependencies.Add(dependencyObj);
           }
        }

        return dependencies;
    }

    private List<Object> GetRecursiveFolderAssets(List<Object> list)
    {
        List<Object> recurseAssets = new List<Object>();

        foreach(Object obj in list)
        {
            GetObjectsRecursive(recurseAssets, obj);
        }

        return recurseAssets;
    }

    private void GetObjectsRecursive(List<Object> recurseAssets, Object obj)
    {
        if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)))
        {
            Debug.Log("Checking folder : " + AssetDatabase.GetAssetPath(obj));
            List<Object> folderAssets = FindAssets<Object>(AssetDatabase.GetAssetPath(obj));
            Debug.Log($"{folderAssets.Count} asset in the folder {obj.name}");
            recurseAssets.AddRange(folderAssets);

            foreach(Object folderAsset in folderAssets)
            {
                GetObjectsRecursive(recurseAssets, folderAsset);
            }

        }
    }

    public static List<T> FindAssets<T>(params string[] paths) where T : Object
    {
        string[] assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(T), paths);
        List<T> assets = new List<T>();
        foreach (string guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            assets.Add(asset);
        }
        return assets;
    }


    #region Asset Creation Methods
    void CreateExporterList()
    {
        ExporterListSo asset = ScriptableObject.CreateInstance<ExporterListSo>();
        asset.packageName = packageName;

        exportListDestinationPath = EditorUtility.OpenFolderPanel("Export Destination", "Asset", "Asset");
        Debug.Log("New Export List save in :" + exportListDestinationPath);
        exportListDestinationPath = exportListDestinationPath.Substring(exportListDestinationPath.IndexOf("Assets/"));
        Debug.Log("New Export List save in :" + exportListDestinationPath);

        AssetDatabase.CreateAsset(asset, $"{exportListDestinationPath}/{packageName}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
        exportList = asset;
    }
    

    #endregion
}
