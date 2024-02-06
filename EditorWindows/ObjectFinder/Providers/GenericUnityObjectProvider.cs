using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class GenericUnityObjectProvider<T> : ScriptableObject, ISearchWindowProvider
{
    private Action<T> objectCallback;

    public GenericUnityObjectProvider(Action<T> callback)
    {
        objectCallback = callback;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Objects"), 0);
        searchList.Add(group);


        foreach(T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            //Sort by type : meshes, textures, materials, scripts, 
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(obj.ToString()))
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
        objectCallback?.Invoke((T)SearchTreeEntry.userData);
        return true;
    }

}
