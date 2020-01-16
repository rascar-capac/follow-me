using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : Singleton<ObjectsManager>
{
    protected override void Start()
    {
        base.Start();
        this["Tribe"] = GameObject.Find("Tribe");
        this["Player"] = GameObject.Find("Player");
        this["Optimum"] = GameObject.Find("Optimum");
        this["Fog"] = GameObject.Find("Fog");
    }
}
