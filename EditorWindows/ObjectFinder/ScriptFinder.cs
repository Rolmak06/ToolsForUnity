using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptFinder : ObjectFinderCondition
{
    public Type targetedType;
    public override List<GameObject> Process(List<GameObject> objects)
    {
        List<GameObject> tempObj = new List<GameObject>(objects);
        objects.Clear();

        if(targetedType == null)
        {
            Debug.Log("Type is null, select a type in the appropriate field on the editor window.");
            return objects;
        }

        foreach(GameObject obj in tempObj)
        {
            foreach(Component comp in obj.GetComponents<Component>())
            {
                if(comp == null){continue;}

                 //ternary operator for inclusion or exclusion of the search method. If exclude is true, collect only objects that doesn't fall in the method's parameters.
                if(exclude ? comp.GetType() != targetedType : comp.GetType() == targetedType)
                {
                        objects.Add(obj);
                        break;
                }
            }
        }

        return objects;
    }
}