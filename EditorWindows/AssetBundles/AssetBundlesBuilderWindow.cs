using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetBundlesBuilderWindow : EditorWindow
{
    //Store asset bundles names and use of a custom class to display bundles names with a popup and select them with an int 
    private string[] existingAssetBundlesNames;
    private List<AssetBundlesManager.AssetBundleName> assetBundlesNames = new List<AssetBundlesManager.AssetBundleName>(); 
    private List<string> assetBundlesToBuild = new List<string>();

    //Store options into a list to then define the build option by combining them
    private List<BuildAssetBundleOptions> bundleOptionsList = new List<BuildAssetBundleOptions>();
    private BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

    private BuildTarget targetPlatform;
    private string outputPath;

    // Editor variables
    private Vector2 optionsScroll;



    [MenuItem("Asset Bundles/Asset Bundles Builder")]
    static void ShowWindow()
    {
        AssetBundlesBuilderWindow window = GetWindow<AssetBundlesBuilderWindow>();
        window.titleContent = new GUIContent("Asset Bundles Builder");
        window.Show();
    }

    void OnEnable()
    {
        // assign default path and target platform then retreive all existing bundles
        outputPath = Application.streamingAssetsPath + "/AssetBundles/";
        targetPlatform = EditorUserBuildSettings.activeBuildTarget;
        GetBundlesNames();
        bundleOptionsList.Add(BuildAssetBundleOptions.ChunkBasedCompression);
    }

    void OnGUI()
    {
        DrawOutputSelection();

        DrawBuildOptions();

        DrawBundleSelection();

        DrawBuildAllBundles();
    }



    //Get bundles names from bundle manager
    void GetBundlesNames()
    {
        existingAssetBundlesNames = AssetBundlesManager.FindExistingBundles().Keys.ToArray();
    }

    //Draw an output path selection
    private void DrawOutputSelection()
    {
        
        if (GUILayout.Button("Select Output Folder..."))
        {
            outputPath = EditorUtility.OpenFolderPanel("Ouput Location", "", "");
        }
        EditorGUILayout.LabelField("Output Path", outputPath);
    }

    //Draw bundles build options and target
    private void DrawBuildOptions()
    {
        
        GUILayout.BeginVertical("Bundle Options", "window", GUILayout.MaxHeight(300));

            if(GUILayout.Button("+ Add Options"))
            {
                    bundleOptionsList.Add(new BuildAssetBundleOptions());
            }

            optionsScroll = EditorGUILayout.BeginScrollView(optionsScroll);
            
                try // try catch to avoid any error because we're modifying the list we're cycling through
                {   
                    //Allows multiple options 
                    for (int i = 0; i < bundleOptionsList.Count; i++)
                    {
                        GUILayout.BeginVertical("Option " + (i+1), "window");
                            EditorGUILayout.BeginHorizontal();
                                bundleOptionsList[i] = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(bundleOptionsList[i]);
                                if(GUILayout.Button("X", GUILayout.MaxWidth(50)))
                                {
                                    bundleOptionsList.Remove(bundleOptionsList[i]);
                                }
                            EditorGUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                }
                
                catch{}

            EditorGUILayout.EndScrollView();


            targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Build Target Platform", targetPlatform);
        GUILayout.EndVertical();
    }

    //Draw the targeted bundles names list + Button to build selected bundles
    private void DrawBundleSelection()
    {
        EditorGUILayout.Space(10);

        GUILayout.BeginVertical("Selected Bundles", "window", GUILayout.MaxHeight(300));
            EditorGUILayout.LabelField("Selected Bundles :" + assetBundlesNames.Count);

            EditorGUILayout.Space(10);

            if(GUILayout.Button("+ Add Bundle to Selection"))
            {
                assetBundlesNames.Add(new AssetBundlesManager.AssetBundleName());
            }


            try
            {
                for (int i = 0; i < assetBundlesNames.Count; i++)
                {
                    GUILayout.BeginVertical("Bundle " + (i+1), "window");
                        EditorGUILayout.BeginHorizontal();

                            assetBundlesNames[i].NameIndex = EditorGUILayout.Popup(assetBundlesNames[i].NameIndex, existingAssetBundlesNames);
                            if(GUILayout.Button("X", GUILayout.MaxWidth(50)))
                            {
                                assetBundlesNames.Remove(assetBundlesNames[i]);
                            }
                        EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }

            catch{}

            // Disable this option if the bundles name list is not populated 
            GUI.enabled = assetBundlesNames.Count > 0;

            //Draw build bundles buttons : by names / all
            if(GUILayout.Button("Build Selected Asset Bundles"))
            {
                CreateBundleSelectionList();
                CreateBundleOptions();

                if(EditorUtility.DisplayDialog("Build Asset Bundles By Name", $"Are you sure you want to build {assetBundlesToBuild.Count} selected bundles. Target Platform : {targetPlatform}, Build Options : {bundleOptions}", "Yes", "Cancel"))
            {
                AssetBundlesBuilder.BuildAssetBundlesByName(assetBundlesToBuild.ToArray(), outputPath, targetPlatform, bundleOptions);
            }
        }
            
            GUI.enabled = true;

        GUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }

    

    // Draw button to build all bundles
    private void DrawBuildAllBundles()
    {
        if (GUILayout.Button("Build All Asset Bundles"))
        {
            CreateBundleOptions();

            if (EditorUtility.DisplayDialog("Build Asset Bundles", $"Are you sure you want to build all bundles in this project ? Target Platform : {targetPlatform}, Build Options : {bundleOptions}", "Yes", "Cancel"))
            {
                AssetBundlesBuilder.BuildBundles(outputPath, targetPlatform, bundleOptions);
            }
        }
    }

    private void CreateBundleSelectionList()
    {
        assetBundlesToBuild.Clear();

        foreach (AssetBundlesManager.AssetBundleName assetBundleName in assetBundlesNames)
        {
            assetBundleName.Name = existingAssetBundlesNames[assetBundleName.NameIndex];
            assetBundlesToBuild.Add(assetBundleName.Name);
        }
    }
    private void CreateBundleOptions()
    {
        if(bundleOptionsList.Count < 1)
        {
            bundleOptions = BuildAssetBundleOptions.None;
        }

        else
        {
            for (int i = 0; i < bundleOptionsList.Count; i++)
            {
                if(i == 0)
                {
                    bundleOptions = bundleOptionsList[i];
                }

                else
                {
                    bundleOptions |= bundleOptionsList[i];
                }
            }
        }

        
    }
}
