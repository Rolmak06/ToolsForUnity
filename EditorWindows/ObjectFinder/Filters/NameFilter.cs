using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    public class NameFilter : BaseFilter
    {
        public string targetedName;
        public bool exact;
        public bool caseSensitive;

        //Constructors
        public NameFilter(string targetedName = null, bool exact = false, bool caseSensitive = false)
        {
            this.targetedName = targetedName;
            this.exact = exact;
            this.caseSensitive = caseSensitive;
        }

        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(targetedName == string.Empty || targetedName == "")
            {
                Debug.LogError("[OBJECT FINDER] Name condition is null");
                return objects;
            }

            if(exact)
            {
                objects = exclude ? objects.Where(obj => obj.name.ToLower() != targetedName.ToLower()).ToList() : objects.Where(obj => obj.name.ToLower() == targetedName.ToLower()).ToList();
            }

            else
            {
                objects = exclude ? objects.Where(obj => !obj.name.ToLower().Contains(targetedName.ToLower())).ToList() : objects.Where(obj => obj.name.ToLower().Contains(targetedName.ToLower())).ToList();
            }

            if(caseSensitive)
            {
                if(exact)
                {
                    objects = exclude ? objects.Where(obj => obj.name != targetedName).ToList() : objects.Where(obj => obj.name == targetedName).ToList();
                }

                else
                {
                    objects = exclude ? objects.Where(obj => !obj.name.Contains(targetedName)).ToList() :objects.Where(obj => obj.name.Contains(targetedName)).ToList();
                }

                
            }

            return objects;
        }

}
}