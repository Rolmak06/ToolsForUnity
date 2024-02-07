using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    /// <summary>
    /// Filters a GameObjects List and keep only those who carry the specified material (MeshRenderer and SkinnedMeshRenderer)
    /// </summary>
    public class MaterialFilter : BaseFilter
    {
        public Material targetedMaterial;

        //Constructor
        public MaterialFilter(Material targetedMaterial = null)
        {
            this.targetedMaterial = targetedMaterial;
        }

        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(targetedMaterial == null)
            {
                Debug.LogError("[OBJECT FINDER] Material condition is null");
                return objects;
            }

            // Gets all Renderers of the entry list

            List<GameObject> renderers = new List<GameObject>();
            List<GameObject> objMaterial = new List<GameObject>();

            renderers = objects.Where(obj => obj.GetComponent<Renderer>()).ToList();

            //Search for the targeted material in the list

            foreach(GameObject renderer in renderers)
            {
                foreach(Material material in renderer.GetComponent<Renderer>().sharedMaterials)
                {
                    if(material == null){continue;}

                    if(material == targetedMaterial)
                    {
                        objMaterial.Add(renderer);
                    }
                }
            }


            objects.Clear();
            objects = objMaterial;

            return objects;
        }
    }

}