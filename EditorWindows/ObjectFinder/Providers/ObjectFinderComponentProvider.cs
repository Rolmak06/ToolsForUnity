using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObjectFinderComponentProvider : ScriptableObject, ISearchWindowProvider
{
    private Dictionary<string, Type> scriptsList = new Dictionary<string, Type>();
    private Action<Type> onSetIndexCallback;

    public ObjectFinderComponentProvider(Dictionary<string, Type> scriptsList, Action<Type> callback)
    {
        this.scriptsList = scriptsList;
        onSetIndexCallback = callback;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Components"), 0);
        searchList.Add(group);

        Debug.Log("Keys count : " + scriptsList.Keys.Count);

        if(scriptsList.Keys == null) {return searchList;}

        foreach(string key in scriptsList.Keys)
        {
            Type type;
            scriptsList.TryGetValue(key, out type);
            
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(key))
            {
                level = 1,
                userData = type
            };

            searchList.Add(entry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        onSetIndexCallback?.Invoke((Type)SearchTreeEntry.userData);
        return true;
    }


}
