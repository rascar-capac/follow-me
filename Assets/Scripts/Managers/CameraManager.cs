using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera _MainCamera;
    public float Height => _MainCamera.orthographicSize * 2;
    public float Width => Height * _MainCamera.aspect;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (_MainCamera == null)
            _MainCamera = Camera.main;
        Debug.Log(Height);
        Debug.Log(Width);
        Debug.Log(_MainCamera.orthographicSize);
    }

}
