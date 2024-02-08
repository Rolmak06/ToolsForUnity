using System.Collections;
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
    public static void BuildAssetBundlesByName(string[] assetBundleNames, string outputPath, BuildTarget targetPlatform) 
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
           Debug.Log("assetBundle to build:" + build.assetBundleName);
       }

        CheckPath(outputPath);
       BuildPipeline.BuildAssetBundles(outputPath, builds.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, targetPlatform);
   }

    /// <summary>
    /// Build all bundles in the project
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="targetPlatform"></param>
   public static void BuildBundles(string outputPath, BuildTarget targetPlatform) 
   {
        CheckPath(outputPath);
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, targetPlatform);
    }

    private static void CheckPath(string outputPath)
    {
        if (!System.IO.Directory.Exists(outputPath)) 
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
    }
}
