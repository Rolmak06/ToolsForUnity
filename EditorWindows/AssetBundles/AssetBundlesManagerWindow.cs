using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundlesManagerWindow : EditorWindow
{  
    SerializedObject so;
    Dictionary<string, List<Object>> bundles = new Dictionary<string, List<Object>>();
    Vector2 bundlesScroll;
    Vector2 newBundleScroll;
    string newBundleName;
    public List<Object> newBundleObjects = new List<Object>();
    SerializedProperty newBundleObjectsProperty;

    [MenuItem("Asset Bundles/Asset Bundles Manager")]
    static void ShowWindow()
    {
        AssetBundlesManagerWindow window = GetWindow<AssetBundlesManagerWindow>();
    }

    void OnEnable()
    {
        ScriptableObject target = this;
        so = new SerializedObject(target);

        RefreshBundles();

        newBundleObjectsProperty = so.FindProperty("newBundleObjects");
    }

    

    void OnGUI()
    {
        so.Update();
        
        DrawBundleCreation();

        EditorGUILayout.Space(10);

        DrawExistingBundles();

        if(GUILayout.Button("Refresh Bundles"))
        {
            bundles = AssetBundlesManager.FindExistingBundles();
        }

        so.ApplyModifiedProperties();
    }

    void DrawBundleCreation()
    {
        GUILayout.BeginVertical("Create Bundle / Add to Bundle", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(300));

            newBundleName = EditorGUILayout.TextField("Bundle Name", newBundleName);

            EditorGUILayout.LabelField("Bundle Objects", EditorStyles.miniBoldLabel);
            newBundleScroll = EditorGUILayout.BeginScrollView(newBundleScroll);
                EditorGUILayout.PropertyField(newBundleObjectsProperty);
            EditorGUILayout.EndScrollView();

            if(GUILayout.Button("Create Bundle / Add to Bundle"))
            {
                AssetBundlesManager.AddObjectsToBundle(newBundleObjects, newBundleName);
            }

        GUILayout.EndVertical();

        
    }
    void DrawExistingBundles()
    {
        GUILayout.BeginVertical("Existing Asset Bundles", "window", GUILayout.MaxHeight(600), GUILayout.MinHeight(300));

            bundlesScroll = EditorGUILayout.BeginScrollView(bundlesScroll);

                foreach(string bundleName in bundles.Keys)
                {
                    EditorGUILayout.LabelField(bundleName, EditorStyles.whiteLargeLabel);

                    List<Object> bundleContent = new List<Object>();

                    bundles.TryGetValue(bundleName, out bundleContent);

                    foreach (Object item in bundleContent)
                    {
                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.ObjectField(item, typeof(Object), true);

                            if(GUILayout.Button("X", GUILayout.MaxWidth(50)))
                            {
                                if(AssetBundlesManager.GetAssetInfo(item).assetBundleName == null)
                                {
                                    Debug.Log("This asset is not marked directly.");
                                    EditorUtility.DisplayDialog("Asset Info", "This asset is not marked directly. We can't remove it from the bundle. Maybe it is contained in a folder marked with the asset bundle name", "Ok");
                                }

                                else
                                {
                                    AssetBundlesManager.RemoveObjectFromBundle(item);
                                    RefreshBundles();
                                }
                            }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space(10);
                
                }

            EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
    }
    
    private void RefreshBundles()
    {
        bundles = AssetBundlesManager.FindExistingBundles();
    }
}
