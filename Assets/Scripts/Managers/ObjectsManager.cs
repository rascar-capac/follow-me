using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : Singleton<ObjectsManager>
{
    protected override void Start()
    {
        base.Start();
        this["TribeGroundPosition"] = GameObject.Find("TribeGroundPosition");
        this["Player"] = GameObject.Find("Player");
        this["Optimum"] = GameObject.Find("Optimum");
        this["Tribe"] = GameObject.Find("Tribe");
        this["Fog"] = GameObject.Find("Fog");
        this["Terrain"] = GameObject.Find("Terrain");
    }
}
