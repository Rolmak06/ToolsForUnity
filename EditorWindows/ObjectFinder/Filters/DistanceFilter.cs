using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    /// <summary>
    /// Filters a GameObjects List and keep only those who fits in the specified distance of the specified Transform
    /// </summary>
    public class DistanceFilter : BaseFilter
    {
        public Transform sourceObject;
        public float minRange = 0f;
        public float maxRange = 10f;

        //Constructor
        public DistanceFilter(Transform sourceObject = null, float minRange = 0f, float maxRange = 100f)
        {
            this.sourceObject = sourceObject;
            this.minRange = minRange;
            this.maxRange = maxRange;
        }
        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(sourceObject == null)
            {
                Debug.LogError("[OBJECT FINDER] Source Object is null");
                return objects;
            }

            objects = exclude? objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) < minRange && Vector3.Distance(sourceObject.position, obj.transform.position) > maxRange).ToList():
            objects.Where(obj => Vector3.Distance(sourceObject.position, obj.transform.position) > minRange && Vector3.Distance(sourceObject.position, obj.transform.position) < maxRange).ToList();

            objects.OrderBy(obj => Vector3.Distance(sourceObject.position, obj.transform.position));

            return objects;
        }
    }

}