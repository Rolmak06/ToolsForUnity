using System.Collections.Generic;
using UnityEngine;

namespace ObjectFinderTool
{
    public abstract class BaseFilter
    {
        /// <summary>
        /// Will process a gameObject list to apply filters
        /// </summary>
        /// <param name="objects">List of gameObjets to process</param>
        /// <returns>Filtered GameObjects list</returns>
        public abstract List<GameObject> Process(List<GameObject> objects);

        /// <summary>
        /// Does this filter act as exclusion ? This means that the filter will remove matching gameObjects during the process.
        /// </summary>
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

