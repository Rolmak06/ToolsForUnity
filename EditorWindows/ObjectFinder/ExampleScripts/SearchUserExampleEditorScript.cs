using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectFinderTool;
using UnityEditor;
using UnityEngine.AI;

public class SearchUserExampleEditorScript : EditorWindow
{
    [MenuItem("Tools/Object Finder/Example")]
        static void ShowWindow()
        {
            SearchUserExampleEditorScript window = (SearchUserExampleEditorScript)EditorWindow.GetWindow(typeof(SearchUserExampleEditorScript));
            window.titleContent = new GUIContent("Object Finder User", "You can use the Object Finder from any other editor script");
            window.Show();
        }

        void OnGUI()
        {
            if(GUILayout.Button("Disable All Objects with Navmesh Agent named Bob"))
            {
                // Create filters with a targeted component and a targeted name.
                BaseFilter[] searchFilters = {new ComponentFilter(typeof(NavMeshAgent)), new NameFilter("Bob", true, true)};
                
                // Populate the list with the search result
                GameObject[] bobs = ObjectFinderEditor.Search(searchFilters, true, 0).ToArray();

                //Do what you want with it
                foreach (GameObject bob in bobs)
                {
                    bob.SetActive(false);
                }
            }
        }

}
