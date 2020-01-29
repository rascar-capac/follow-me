 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Materials", menuName = "New MaterialSettings", order = 1)]
public class MaterialData : ScriptableObject
{
    [TabGroup("Day")]
    public List<Material> DayMaterials;

    [TabGroup("Night")]
    public List<Material> NightMaterials;
}
