using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetBundlesBuilderWindow : EditorWindow
{
    [Serializable]
    public class AssetBundleName
    {
        public string assetBundlesName;
        public int selectedName;
    }

    public string[] assetBundlesNamesToBuild;
    public List<AssetBundleName> assetBundlesNames = new List<AssetBundleName>();
    string outputPath;
    BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
    BuildTarget targetPlatform;


    


    [MenuItem("Asset Bundles/Asset Bundles Builder")]
    static void ShowWindow()
    {
        AssetBundlesBuilderWindow window = GetWindow<AssetBundlesBuilderWindow>();
        window.titleContent = new GUIContent("Asset Bundles Builder");
        window.Show();
    }

    void OnEnable()
    {
        // assign default path and target platform
        outputPath = Application.streamingAssetsPath + "/AssetBundles/";
        targetPlatform = EditorUserBuildSettings.activeBuildTarget;

        // create a scriptable out of this window to cast properties 
      

        //retrieve properties
     
        GetBundlesNames();
    }

    void OnGUI()
    {
        DrawOutputSelection();

        DrawBuildOptions();

        //Draw the targeted bundles names list
        DrawBundleSelection();


        if (GUILayout.Button("Build All Asset Bundles"))
        {
            if (EditorUtility.DisplayDialog("Build Asset Bundles", "Are you sure you want to build all bundles in this project ?", "Yes", "Cancel"))
            {
                AssetBundlesBuilder.BuildBundles(outputPath, targetPlatform);
            }

        }
    }

    //Get names from bundle manager
    void GetBundlesNames()
    {
        assetBundlesNamesToBuild = AssetBundlesManager.FindExistingBundles().Keys.ToArray();
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
        
        bundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Build Bundles Options", bundleOptions);
        targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Build Target Platform", targetPlatform);
    }

    void DrawBundleSelection()
    {
        EditorGUILayout.Space(10);

        GUILayout.BeginVertical("Selected Bundles", "window", GUILayout.MaxHeight(300));
            EditorGUILayout.LabelField("Selected Bundles :" + assetBundlesNames.Count);

            EditorGUILayout.Space(10);

            if(GUILayout.Button("+ Add Bundle to Selection"))
            {
                assetBundlesNames.Add(new AssetBundleName());
            }

            try
            {
                for (int i = 0; i < assetBundlesNames.Count; i++)
                {
                    GUILayout.BeginVertical("Bundle" + i, "window");
                        EditorGUILayout.BeginHorizontal();
                            assetBundlesNames[i].selectedName = EditorGUILayout.Popup(assetBundlesNames[i].selectedName, assetBundlesNamesToBuild);
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
                if(EditorUtility.DisplayDialog("Build Asset Bundles By Name", "Are you sure you want to build selected bundles (by names)", "Yes", "Cancel"))
                {  
                    AssetBundlesBuilder.BuildAssetBundlesByName(assetBundlesNamesToBuild.ToArray(), outputPath, targetPlatform);
                }
            }
            
            GUI.enabled = true;

        GUILayout.EndVertical();
        EditorGUILayout.Space(10);
    }

    
}
