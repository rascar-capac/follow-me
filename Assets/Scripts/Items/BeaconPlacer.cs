using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeaconPlacer : Item
{
	[Header("The distance from the player for dropping Beacon")]
	public float _dropDistance = 3.0f;
	[Header("Beacon Prefab to spawn at Ground")]
	public GameObject _beaconPrefab;
	GameObject _beacon;
	[Header("The layers on which the beacon can be placed.")]
	public LayerMask DropableLayers;

	PlayerMovement _player;
	Camera _mainCamera;
	NavMeshAgent _tribeAgent;
	RaycastHit _hitInfo = new RaycastHit();


	protected override void Start()
	{
		base.Start();

		_mainCamera = CameraManager.I._MainCamera;
		_player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		_tribeAgent = ((GameObject)ObjectsManager.I["TribeGroundPosition"]).GetComponent<NavMeshAgent>();

		InputManager.I.onBeaconKeyPressed.AddListener(PlaceBeacon);
	}

	void PlaceBeacon()
	{

		if (!IsEnabled)
			return;

		if (!_player.IsTooFar && AmbiantManager.I.IsDay)
		{
			if (Physics.Raycast(_mainCamera.transform.position + Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized * _dropDistance, Vector3.down, out _hitInfo, 100.0f, DropableLayers))
			{
				if (_beacon == null)
					_beacon = Instantiate(_beaconPrefab);

				_beacon.transform.position = _hitInfo.point;
				//_tribeAgent.destination = _beacon.transform.position;
				_tribeAgent.destination = new Vector3(_beacon.transform.position.x, 0, _beacon.transform.position.z);
			}
		}
		else if (_player.IsTooFar && !AmbiantManager.I.IsDay)
			UIManager.I.AlertMessage("You are too far from tribe and it is night.");
		else if (_player.IsTooFar)
			UIManager.I.AlertMessage("You are too far from tribe.");
		else if (!AmbiantManager.I.IsDay)
			UIManager.I.AlertMessage("Unable to place beacon during night.");
	}
}
