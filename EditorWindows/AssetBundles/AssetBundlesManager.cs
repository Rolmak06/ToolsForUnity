using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundlesManager 
{
    public static void CreateNewBundle()
    {
        
    }

    /// <summary>
    /// Add an objects list to a specified bundle
    /// </summary>
    /// <param name="objects">List of object to include in the bundle</param>
    /// <param name="assetBundleName">Bundle's name</param>
    /// <param name="assetBundleVariant">Bundle's variant</param>
    public static void AddObjectsToBundle(List<UnityEngine.Object> objects, string assetBundleName, string assetBundleVariant = null)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            AssetImporter importerInfo = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objects[i]));
            importerInfo.SetAssetBundleNameAndVariant(assetBundleName, assetBundleVariant);
        }
    }

    public static void RemoveObjectFromBundle(Object obj)
    {
        AssetImporter importerInfo = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj));
        importerInfo.SetAssetBundleNameAndVariant(null, null);
    }

    /// <summary>
    /// Get all bundles and their content as a dictionary
    /// </summary>
    /// <returns>dictionary with bundles names and relative assets</returns>
    public static Dictionary<string, List<Object>> FindExistingBundles()
    {
        Dictionary<string, List<Object>> existingBundles = new Dictionary<string, List<Object>>();

        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();

        foreach(string assetBundleName in assetBundleNames)
        {
            string[] assetsPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            List<Object> itemList = new List<Object>();

            foreach(string assetPath in assetsPaths)
            {
                itemList.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
            }
            
            existingBundles.TryAdd(assetBundleName, itemList);
        }

        return existingBundles;
    }

    public static AssetImporter GetAssetInfo(Object obj)
    {
        AssetImporter importerInfo = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj));
        return importerInfo;
    }

    /// <summary>
    /// Remove unused asset bundles names
    /// </summary>
    public static void CleanUnusedBundleNames()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
    
}
