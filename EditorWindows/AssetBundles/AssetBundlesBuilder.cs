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
    /// Build a selection of Asset Bundles selected withan array of strings (by names).
    /// </summary>
    /// <param name="assetBundleNames">String array representing bundles names</param>
    /// <param name="outputPath">Folder destination for bundles</param>
    /// <returns> The manifest of the built bundles </returns>
    public static AssetBundleManifest BuildAssetBundlesByName(string[] assetBundleNames, string outputPath, BuildTarget targetPlatform, BuildAssetBundleOptions options) 
   {
       // Argument validation
       if (assetBundleNames == null || assetBundleNames.Length == 0 || assetBundleNames.Contains(""))
       {
            Debug.LogError("Bundles Names contains an error");
            return null;
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
       AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), options, targetPlatform);

       return manifest;
   }

    /// <summary>
    /// Build all Asset Bundles in the project
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="targetPlatform"></param>
    /// <returns> The manifest of the built bundles </returns>
   public static AssetBundleManifest BuildBundles(string outputPath, BuildTarget targetPlatform, BuildAssetBundleOptions options) 
   {
        CheckPath(outputPath);
        Debug.Log($"Building Assets bundles with options : {options}  at {outputPath} for {targetPlatform}");
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, options, targetPlatform);
        return manifest;
       
    }

    private static void CheckPath(string outputPath)
    {
        if (!System.IO.Directory.Exists(outputPath)) 
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
    }
}
