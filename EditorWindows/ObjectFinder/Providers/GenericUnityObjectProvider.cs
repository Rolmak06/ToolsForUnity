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

    /// <summary>
    /// Creates the Search Tree by finding desired elements and creating GUI content with its data 
    /// </summary>

    public virtual List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Objects"), 0);
        searchList.Add(group);


        foreach(T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        { 
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(obj.ToString()))
            {
                level = 1,
                userData = obj
            };

            searchList.Add(entry);
        }

        return searchList;
    }

    /// <summary>
    /// This callback is called when selecting and entry while the search window is open. This return the user data type linked to the entry when
    /// </summary>

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        objectCallback?.Invoke((T)SearchTreeEntry.userData);
        return true;
    }

}
