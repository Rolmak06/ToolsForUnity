using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
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

            List<GameObject> renderers = new List<GameObject>();
            List<GameObject> skinnedRenderers = new List<GameObject>();
            List<GameObject> objMaterial = new List<GameObject>();

            renderers = objects.Where(obj => obj.GetComponent<MeshRenderer>()).ToList();
            skinnedRenderers = objects.Where(obj => obj.GetComponent<SkinnedMeshRenderer>()).ToList();


            foreach(GameObject renderer in renderers)
            {
                foreach(Material material in renderer.GetComponent<MeshRenderer>().sharedMaterials)
                {
                    if(material == null){continue;}

                    if(material == targetedMaterial)
                    {
                        objMaterial.Add(renderer);
                    }
                }
            }

            foreach(GameObject skinnedRenderer in skinnedRenderers)
            {
                foreach(Material material in skinnedRenderer.GetComponent<SkinnedMeshRenderer>().sharedMaterials)
                {
                    if(material == null){continue;}
                
                    //ternary operator for inclusion or exclusion of the search method. If exclude is true, collect only objects that doesn't fall in the method's parameters.
                    if(exclude? material != targetedMaterial : material == targetedMaterial)
                    {
                        objMaterial.Add(skinnedRenderer);
                    }
                }   
            }

            objects.Clear();
            objects = objMaterial;

            Debug.Log(objMaterial.Count + " objects in the scene with this material : " + targetedMaterial);

            return objects;
        }
    }

}