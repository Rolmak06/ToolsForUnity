using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
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