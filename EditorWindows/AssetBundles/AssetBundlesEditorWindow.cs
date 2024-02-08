using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetBundlesEditorWindow : EditorWindow
{
    public List<string> assetBundlesNamesToBuild = new List<string>();
    string outputPath;
    BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
    BuildTarget targetPlatform;
    SerializedObject so;
    SerializedProperty assetBundlesNamesToBuildProperty;


    [MenuItem("Asset Bundles/Asset Bundles Builder")]
    static void ShowWindow()
    {
        AssetBundlesEditorWindow window = GetWindow<AssetBundlesEditorWindow>();
        window.titleContent = new GUIContent("Asset Bundles Builder");
        window.Show();
    }

    void OnEnable()
    {
        // assign default path and target platform
        outputPath = Application.streamingAssetsPath + "/AssetBundles/";
        targetPlatform = EditorUserBuildSettings.activeBuildTarget;

        // create a scriptable out of this window to cast properties 
        ScriptableObject target = this;
        so = new SerializedObject(target);

        //retrieve properties
        assetBundlesNamesToBuildProperty = so.FindProperty("assetBundlesNamesToBuild");
    }

    void OnGUI()
    {
        so.Update();

        //Draw an output path selection
        if(GUILayout.Button("Select Output Folder..."))
        {
           outputPath = EditorUtility.OpenFolderPanel("Ouput Location", "", "");
        }
        EditorGUILayout.LabelField("Output Path", outputPath);

        //Draw bundles build options and target
        bundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Build Bundles Options", bundleOptions);
        targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Build Target Platform", targetPlatform);

        //Draw the targeted bundles names list
        EditorGUILayout.PropertyField(assetBundlesNamesToBuildProperty);


        // Disable this option if the bundles name list is not populated 
        GUI.enabled = assetBundlesNamesToBuild.Count > 0 && !assetBundlesNamesToBuild.Contains("");


        //Draw build bundles buttons : by names / all
        if(GUILayout.Button("Build Selected Asset Bundles"))
        {
            if(EditorUtility.DisplayDialog("Build Asset Bundles By Name", "Are you sure you want to build selected bundles (by names)", "Yes", "Cancel"))
            {  
                AssetBundlesBuilder.BuildAssetBundlesByName(assetBundlesNamesToBuild.ToArray(), outputPath, targetPlatform);
            }
        }
        GUI.enabled = true;

        if(GUILayout.Button("Build All Asset Bundles"))
        {
            if(EditorUtility.DisplayDialog("Build Asset Bundles", "Are you sure you want to build all bundles in this project ?", "Yes", "Cancel"))
            {
                AssetBundlesBuilder.BuildBundles(outputPath, targetPlatform);
            }
            
        }

        so.ApplyModifiedProperties();
    }

    
}
