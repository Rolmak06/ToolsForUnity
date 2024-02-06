using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnityObjectProvider : ScriptableObject, ISearchWindowProvider
{
    private Action<UnityEngine.Object> objectCallback;

    public UnityObjectProvider(Action<UnityEngine.Object> callback)
    {
        objectCallback = callback;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Objects"), 0);
        searchList.Add(group);



        foreach(UnityEngine.Object obj in Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
        {
            //Sort by type : meshes, textures, materials, scripts, 
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(obj.name))
            {
                level = 1,
                userData = obj
            };

            searchList.Add(entry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        objectCallback?.Invoke((UnityEngine.Object)SearchTreeEntry.userData);
        return true;
    }

}
