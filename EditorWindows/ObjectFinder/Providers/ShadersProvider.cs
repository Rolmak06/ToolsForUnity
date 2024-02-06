using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShadersProvider : GenericUnityObjectProvider<Shader>
{
    public ShadersProvider(Action<Shader> callback) : base(callback)
    {
        
    }

    public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        SearchTreeGroupEntry group = new SearchTreeGroupEntry(new GUIContent("Shaders"), 0);
        searchList.Add(group);

        List<string> groups = new List<string>();

        Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();

        foreach(Shader obj in shaders)
        { 
            //Create indentation levels according to the depth of the shader

            string[] entryTitle = obj.ToString().Split('/');
            string groupName = "";

            for (int i = 0; i < entryTitle.Length-1; i++)
            {
                groupName += entryTitle[i];
                if(!groups.Contains(groupName))
                {
                    searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i+1));
                    groups.Add(groupName);
                }
            }


            // Add the shader to the list at the defined depth with its data

            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
            {
                level = entryTitle.Length,
                userData = obj
            };

            searchList.Add(entry);
        }

        return searchList;
    }
}


