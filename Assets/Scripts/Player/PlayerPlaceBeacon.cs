using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerPlaceBeacon : BaseMonoBehaviour
{
    [Header("The prefabs to instantiate for the Beacon")]
    public GameObject _tribeFollowTargetPrefab;
    [Header("The distance from the player for dropping Beacon")]
    public float DropDistance = 1.0f;
    [Header("The layers on which the beacon can be placed.")]
    public LayerMask DropableLayers;

    PlayerMovement player;
    GameObject _tribeFollowTarget;
    RaycastHit _hitInfo = new RaycastHit();
    NavMeshAgent _tribeAgent;
    Camera _mainCamera;

    protected override void Start()
	{
        base.Start();
        player = GetComponent<PlayerMovement>();
        _tribeAgent = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<NavMeshAgent>();
        _mainCamera = CameraManager.I._MainCamera;
        _tribeFollowTarget = Instantiate(_tribeFollowTargetPrefab);
        _tribeFollowTarget.transform.position = new Vector3(_tribeAgent.transform.position.x, 0, _tribeAgent.transform.position.z);
        InputManager.I.onBeaconKeyPressed.AddListener(PlaceBeacon);
	}

    protected void Update()
    {
        Debug.DrawRay(_mainCamera.transform.position + _mainCamera.transform.forward * DropDistance, Vector3.down * 100.0f, Color.black);
    }

    void PlaceBeacon()
    {
        if (!player.IsTooFar && AmbiantManager.I.IsDay)
        {
            if (Physics.Raycast(_mainCamera.transform.position + Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized * DropDistance, Vector3.down, out _hitInfo, 100.0f, DropableLayers))
            {
                _tribeFollowTarget.transform.position = _hitInfo.point;
                _tribeAgent.destination = _tribeFollowTarget.transform.position;
            }
        }
        else if (player.IsTooFar && !AmbiantManager.I.IsDay)
            UIManager.I.AlertMessage("You are too far from tribe and it is night.");
        else if (player.IsTooFar)
            UIManager.I.AlertMessage("You are too far from tribe.");
        else if (!AmbiantManager.I.IsDay)
            UIManager.I.AlertMessage("Unable to place beacon during night.");

    }
}
