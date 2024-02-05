using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Export List", menuName ="Custom Export/Export List")]
public class ExporterListSo : ScriptableObject
{
    public string packageName;
    public List<Object> objects;
}
