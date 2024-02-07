using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
    /// <summary>
    /// Filters a GameObjects List and keep only those who carry the specified shader (MeshRenderer and SkinnedMeshRenderer)
    /// </summary>
    public class ShaderFilter : BaseFilter
    {
        public Shader targetedShader;

        //Constructor 
        public ShaderFilter(Shader targetedShader = null)
        {
            this.targetedShader = targetedShader;
        }
        public override List<GameObject> Process(List<GameObject> objects)
        {
            if(targetedShader == null)
            {
                Debug.LogError("[OBJECT FINDER] Shader condition is null");
                return objects;
            }

            // Get all Renderers of the entry list

            List<GameObject> renderers = new List<GameObject>();
            List<GameObject> objShader = new List<GameObject>();

            renderers = objects.Where(obj => obj.GetComponent<Renderer>()).ToList();

            //Search for the targeted shader in the list

            foreach(GameObject renderer in renderers)
            {
                foreach(Material material in renderer.GetComponent<MeshRenderer>().sharedMaterials)
                {
                    if(material == null){continue;}
                    if(material.shader == null){continue;}

                    if(material.shader == targetedShader)
                    {
                        objShader.Add(renderer);
                    }
                }
            }


            objects.Clear();
            objects = objShader;

            return objects;
        }
    }

}