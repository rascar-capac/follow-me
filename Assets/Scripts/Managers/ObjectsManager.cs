using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : Singleton<ObjectsManager>
{
    protected override void Start()
    {
        Debug.Log("Loading objects");
        base.Start();
        this["TribeGroundPosition"] = GameObject.Find("TribeGroundPosition");
        this["Player"] = GameObject.Find("Player");
        this["Optimum"] = GameObject.Find("Optimum");
        this["Fog"] = GameObject.Find("Fog");

        if (this["TribeGroundPosition"] == null)
        {
            Debug.Log("TribeGroundPosition not found !");
        }
        Debug.Log("Loaded");

    }
}
