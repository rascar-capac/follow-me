using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraMode
{
    ModePlayer,
    ModeTribe,
    None
}

public class CameraManager : Singleton<CameraManager>
{
    public Camera _MainCamera;
    public float Height => _MainCamera.orthographicSize * 2;
    public float Width => Height * _MainCamera.aspect;
    public Camera PlayerCamera;
    public Camera TribeCamera;
    Transform TribeCameraTarget;
    
    Transform PlayerCameraTarget;
    Player player;
    Tribe tribe;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (_MainCamera == null)
            _MainCamera = Camera.main;
    }

    public void SwitchCamera(CameraMode mode = CameraMode.None, float duration = 0f, Transform target=null)
    {
        if (!tribe)
            tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

        if (mode == CameraMode.None)
        {
            PlayerCamera.gameObject.SetActive(!PlayerCamera.gameObject.activeSelf);
            TribeCamera.gameObject.SetActive(!TribeCamera.gameObject.activeSelf);
        }
        else
        {
            if (mode == CameraMode.ModePlayer)
            {
                PlayerCamera.gameObject.SetActive(true);
                
                TribeCamera.gameObject.SetActive(false);
            }
            else
            {
                PlayerCamera.gameObject.SetActive(false);
                TribeCamera.gameObject.SetActive(true);
            }
        }
        TribeCameraTarget = target;
        if (TribeCameraTarget)
        {
            Renderer r = TribeCameraTarget.GetComponent<Renderer>();
            foreach (Material m in r.materials)
            {
                m.SetColor("_Color", Color.yellow);
            }
            StartCoroutine(GoToTarget());
        }
        if (duration > 0)
       
            StartChrono(duration, () => {
                SwitchCamera(CameraMode.ModePlayer);
                tribe.PauseRandom(false);
                tribe.SetMode(TribeEmotionMode.Normal);
                StartCoroutine(tribe.Live());
            });
    }

    IEnumerator GoToTarget()
    {
        tribe.PauseRandom(true, true);
        //yield return tribe.GoingToPosition(new Vector3(TribeCameraTarget.position.x, TribeCameraTarget.position.y + 300, TribeCameraTarget.position.z), speedMove: 100);
        tribe.SetMode(TribeEmotionMode.Happy);
        yield return StartCoroutine(tribe.RotatingAround(TribeCameraTarget, speedMove: 200).GetEnumerator());

    }

    protected void Update()
    {
        if (!player)
            player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        if (TribeCameraTarget)
            TribeCamera.transform.LookAt(TribeCameraTarget);
    }
}
