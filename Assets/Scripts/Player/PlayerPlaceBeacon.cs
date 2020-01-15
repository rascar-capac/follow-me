using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerPlaceBeacon : BaseMonoBehaviour
{
    public GameObject _tribeFollowTargetPrefab;
    public float DropDistance = 1.0f;

    GameObject _tribeFollowTarget;
    RaycastHit _hitInfo = new RaycastHit();
    NavMeshAgent _tribeAgent;
    Camera _mainCamera;

    private void Start()
	{
        _tribeAgent = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<NavMeshAgent>();
        _mainCamera = CameraManager.I._MainCamera;
        _tribeFollowTarget = Instantiate(_tribeFollowTargetPrefab);
        _tribeFollowTarget.transform.position = new Vector3(_tribeAgent.transform.position.x, 0, _tribeAgent.transform.position.z);
        //_tribeAgent.destination = _tribeFollowTarget.transform.position;
        InputManager.I.onGKeyPressed.AddListener(PlaceBeacon);
	}

    protected void Update()
    {
        Debug.DrawRay(_mainCamera.transform.position + _mainCamera.transform.forward * DropDistance, Vector3.down * 100.0f, Color.black);
    }

    void PlaceBeacon()
    {
        if (Physics.Raycast(_mainCamera.transform.position + _mainCamera.transform.forward * DropDistance, Vector3.down, out _hitInfo, 100.0f))
        {
            _tribeFollowTarget.transform.position = _hitInfo.point;
            _tribeAgent.destination = _tribeFollowTarget.transform.position;
        }
    }
}
