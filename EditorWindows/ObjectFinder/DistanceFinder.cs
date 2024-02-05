using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceFinder : ObjectFinderCondition
{
    public Transform sourceObject;
    public float minRange;
    public float maxRange;
    public override List<GameObject> Process(List<GameObject> objects)
    {
        objects = exclude? objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) < minRange && Vector3.Distance(sourceObject.position, obj.transform.position) > maxRange).ToList():
        objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) > minRange && Vector3.Distance(sourceObject.position, obj.transform.position) < maxRange).ToList();

        objects.OrderBy(obj => Vector3.Distance(sourceObject.position, obj.transform.position));

        return objects;
    }
}