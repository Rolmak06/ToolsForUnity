using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ObjectFinderCondition
{
    public abstract List<GameObject> Process(List<GameObject> objects);

    public bool exclude;
}


public enum ObjectFinderConditionsEnum
{
    Name,
    Tag,
    Component,
    Shader,
    Layer,
    Material,
    Distance
}
