using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    public class DistanceFilter : BaseFilter
    {
        public Transform sourceObject;
        public float minRange = 0f;
        public float maxRange = 10f;
        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(sourceObject == null)
            {
                Debug.LogError("[OBJECT FINDER] Source Object is null");
                return null;
            }

            objects = exclude? objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) < minRange && Vector3.Distance(sourceObject.position, obj.transform.position) > maxRange).ToList():
            objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) > minRange && Vector3.Distance(sourceObject.position, obj.transform.position) < maxRange).ToList();

            objects.OrderBy(obj => Vector3.Distance(sourceObject.position, obj.transform.position));

            return objects;
        }
    }

}