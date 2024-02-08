using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class can build asset bundles with differents methods 
/// </summary>
public class AssetBundlesBuilder 
{
    /// <summary>
    /// Build Asset Bundles selected by a string array (by names).
    /// </summary>
    /// <param name="assetBundleNames">String array representing bundles names</param>
    /// <param name="outputPath">Folder destination for bundles</param>
    public static void BuildAssetBundlesByName(string[] assetBundleNames, string outputPath, BuildTarget targetPlatform, BuildAssetBundleOptions options) 
   {
       // Argument validation
       if (assetBundleNames == null || assetBundleNames.Length == 0 || assetBundleNames.Contains(""))
       {
            Debug.LogError("Bundles Names contains an error");
            return;
       }

       List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

       foreach (string assetBundle in assetBundleNames)
       {
           var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundle);

            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = assetBundle,
                assetNames = assetPaths
            };

            builds.Add(build);
       }

        CheckPath(outputPath);
        Debug.Log($"Building Assets bundles {assetBundleNames}  with options : {options}  at {outputPath} for {targetPlatform}");
       BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), options, targetPlatform);
   }

    /// <summary>
    /// Build all bundles in the project
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="targetPlatform"></param>
   public static void BuildBundles(string outputPath, BuildTarget targetPlatform, BuildAssetBundleOptions options) 
   {
        CheckPath(outputPath);
        Debug.Log($"Building Assets bundles with options : {options}  at {outputPath} for {targetPlatform}");
        BuildPipeline.BuildAssetBundles(outputPath, options, targetPlatform);
       
    }

    private static void CheckPath(string outputPath)
    {
        if (!System.IO.Directory.Exists(outputPath)) 
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
    }
}
