using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsProvider : GenericUnityObjectProvider<Material>
{
    public MaterialsProvider(Action<Material> callback) : base(callback)
    {
    }

}
