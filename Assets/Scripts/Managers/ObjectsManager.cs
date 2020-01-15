using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : Singleton<ObjectsManager>
{
    protected void Start()
    {
        this["Tribe"] = GameObject.Find("Tribe");
    }
}
