using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameFinder : ObjectFinderCondition
{
    public string targetedName;
    public bool exact;
    public bool contains;
    public bool caseSensitive;
    public override List<GameObject> Process(List<GameObject> objects)
    {
        if(exact)
        {
            objects = exclude ? objects.Where(obj => obj.name.ToLower() != targetedName.ToLower()).ToList() : objects.Where(obj => obj.name.ToLower() == targetedName.ToLower()).ToList();
        }

        else if(contains)
        {
            objects = exclude ? objects.Where(obj => !obj.name.ToLower().Contains(targetedName.ToLower())).ToList() : objects.Where(obj => obj.name.ToLower().Contains(targetedName.ToLower())).ToList();
        }

        if(caseSensitive)
        {
            if(contains)
            {
                objects = exclude ? objects.Where(obj => !obj.name.Contains(targetedName)).ToList() :objects.Where(obj => obj.name.Contains(targetedName)).ToList();
            }

            if(exact)
            {
                objects = exclude ? objects.Where(obj => obj.name != targetedName).ToList() : objects.Where(obj => obj.name == targetedName).ToList();
            }
        }

        return objects;
    }
}