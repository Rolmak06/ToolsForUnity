using System.Collections.Generic;
using UnityEngine;

namespace ObjectFinderTool
{
    public abstract class BaseFilter
    {
        public abstract List<GameObject> Process(List<GameObject> objects);

        public bool exclude;
    }


    public enum FilterEnum
    {
        Name,
        Tag,
        Component,
        Shader,
        Layer,
        Material,
        Distance
    }
}

