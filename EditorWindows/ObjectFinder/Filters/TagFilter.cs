using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    /// <summary>
    /// Filters a GameObjects List and keep only those with the specified tag
    /// </summary>
    public class TagFilter : BaseFilter
    {
        public string targetedTag;

        //Constructor
        public TagFilter(string targetedTag = null)
        {
            this.targetedTag = targetedTag;
        }

        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(targetedTag == string.Empty || targetedTag == "")
            {
                Debug.LogError("[OBJECT FINDER] Tag condition is null");
                return objects;
            }

            objects = exclude ? objects.Where(obj => obj.tag != targetedTag).ToList() : objects.Where(obj => obj.tag == targetedTag).ToList();

            return objects;
        }
    }

}