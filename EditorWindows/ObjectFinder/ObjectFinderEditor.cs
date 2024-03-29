using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ObjectFinderTool
{
    /// <summary>
    /// This class allows you to find a list of gameobjects anywhere in your project using search filters and parameters
    /// </summary>
    public class ObjectFinderEditor
    {

            /// <summary>
            /// Search through all gameobjects in your project with as many filters as you want
            /// </summary>
            /// <param name="filters">Filters applied to the search. Add filters using their constructors to instantiate them</param>
            /// <param name="onlyActive">Select only active gameObjects</param>
            /// <param name="selection">0 = current scene | 1 = all opened scenes | 2 = project | 3 = all</param>
            /// <returns>Results as a list of GameObjects</returns>
            public static List<GameObject> Search(BaseFilter[] filters, bool onlyActive, int selection)
            {
                Debug.Log($"Starts a Search : only Active = {onlyActive}; Selection = {selection}");

                List<GameObject> searchResult = Resources.FindObjectsOfTypeAll<GameObject>().ToList();

                for (int i = 0; i < filters.Length; i++)
                {
                    searchResult = filters[i].Process(searchResult);
                }


                searchResult = ActiveInHierarchyRestriction(searchResult, onlyActive);
                searchResult = FilterByLocation(searchResult, selection);
                

                return searchResult;
            }

            /// <summary>
            /// Select gameobjects depending on their location : current scene, opened scenes or in project
            /// </summary>
            /// <param name="results"> Objects to filter </param>
            /// <param name="selection">0 = current scene | 1 = all opened scenes | 2 = project | 3 = all</param>
            /// <returns>Filtered object list </returns>
            public static List<GameObject> FilterByLocation(List<GameObject> results, int selection)
            {
                switch(selection)
                {
                    case 0: //Current Scene
                        results = results.Where(x => x.scene == SceneManager.GetActiveScene() && !AssetDatabase.Contains(x)).ToList();
                    break;

                    case 1: //All scenes
                        results = results.Where(x => !AssetDatabase.Contains(x)).ToList();
                    break;

                    case 2: //Project
                        results = results.Where(x => AssetDatabase.Contains(x)).ToList();
                    break;

                    case 3: //Everywhere
                        //Do nothing, we keep all !
                    break;
                }

                return results;
            }

            /// <summary>
            /// Keep only active gameobjects. Be aware, project assets do not appear as active.
            /// </summary>
            /// <param name="results"></param>
            /// <param name="onlyActive"></param>
            /// <returns>Filtered GameObjects as a list</returns>
            public static List<GameObject> ActiveInHierarchyRestriction(List<GameObject> results, bool onlyActive)
            {    
                if (onlyActive)
                {
                    results = results.Where(x => x.activeInHierarchy).ToList();
                }

                return results;
            }
    }
}
