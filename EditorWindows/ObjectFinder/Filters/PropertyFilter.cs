using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ObjectFinderTool
{
    public class PropertyFilter : BaseFilter
    {
        public Type componentType;
        public string targetedProperty;
        public object targetedValue; 

        public PropertyFilter(Type componentType = null, string targetedProperty = null, object targetedValue = null, bool exclude = false)
        {
            this.componentType = componentType;
            this.targetedProperty = targetedProperty;
            this.targetedValue = targetedValue;
            this.exclude = exclude;
        }

        public override List<GameObject> Process(List<GameObject> objects)
        {
            List<GameObject> tempObj = new List<GameObject>(objects);
            objects.Clear();

            foreach (GameObject obj in tempObj)
            {
                bool matchFound = false;

                foreach (Component comp in obj.GetComponents<Component>())
                {
                    if (comp == null) continue;

                    PropertyInfo info = comp.GetType().GetProperty(targetedProperty);
                    if (info == null || !info.CanRead) continue;

                    object value = info.GetValue(comp);
                    if (value == null || targetedValue == null) continue;

                    Type valueType = value.GetType();

                    // Handle Flags enum (bitmask)
                    if (valueType.IsEnum && Attribute.IsDefined(valueType, typeof(FlagsAttribute)))
                    {
                        try
                        {
                            long actual = Convert.ToInt64(value);
                            long expected = Convert.ToInt64(targetedValue);
                            if ((actual & expected) == expected)
                            {
                                matchFound = true;
                                break;
                            }
                        }
                        catch { continue; }
                    }
                    // Normal comparison
                    else
                    {
                        if (value.Equals(targetedValue))
                        {
                            matchFound = true;
                            break;
                        }
                    }
                }

                if (exclude ? !matchFound : matchFound)
                {
                    objects.Add(obj);
                }
            }

            return objects;
        }
    }
}
