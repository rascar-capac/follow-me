using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera _MainCamera;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (_MainCamera == null)
            _MainCamera = Camera.main;
    }

}
