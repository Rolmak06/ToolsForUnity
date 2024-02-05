using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagFinder : ObjectFinderCondition
{
    public string targetedTag;
    public override List<GameObject> Process(List<GameObject> objects)
    {
        objects = exclude ? objects.Where(obj => obj.tag != targetedTag).ToList() : objects.Where(obj => obj.tag == targetedTag).ToList();

        return objects;
    }
}