using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectFinderTool
{
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

            List<GameObject> renderers = new List<GameObject>();
            List<GameObject> skinnedRenderers = new List<GameObject>();
            List<GameObject> objShader = new List<GameObject>();

            renderers = objects.Where(obj => obj.GetComponent<MeshRenderer>()).ToList();
            skinnedRenderers = objects.Where(obj => obj.GetComponent<SkinnedMeshRenderer>()).ToList();


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

            foreach(GameObject skinnedRenderer in skinnedRenderers)
            {
                foreach(Material material in skinnedRenderer.GetComponent<SkinnedMeshRenderer>().sharedMaterials)
                {
                    if(material == null){continue;}
                    if(material.shader == null){continue;}

                    //ternary operator for inclusion or exclusion of the search method. If exclude is true, collect only objects that doesn't fall in the method's parameters.
                    if(exclude? material.shader != targetedShader : material.shader == targetedShader)
                    {
                        objShader.Add(skinnedRenderer);
                    }
                }   
            }

            objects.Clear();
            objects = objShader;

            Debug.Log(objShader.Count + " objects in the scene with this shader : " + targetedShader);

            return objects;
        }
    }

}