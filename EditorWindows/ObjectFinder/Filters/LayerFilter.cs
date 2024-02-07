using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{   
    /// <summary>
    /// Filters a GameObjects List and keep only those with specified layer
    /// </summary>
    public class LayerFilter : BaseFilter
    {
        public int layer;
        public override List<GameObject> Process(List<GameObject> objects)
        {
            objects = exclude ? objects.Where(obj => obj.layer != layer).ToList() : objects.Where(obj => obj.layer == layer).ToList();
            return objects;
        }
    }
    
}