using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeaconPlacer : Item
{

	[Header("Beacon Prefab to spawn at Ground")]
	public Item _beaconPrefab;
	[Header("The layers on which the beacon can be placed.")]
	public LayerMask DropableLayers;

	PlayerMovement _player;
    Player Player;
	Camera _mainCamera;
	//NavMeshAgent _tribeAgent;
	Tribe _Tribe;
	RaycastHit _HitInfo = new RaycastHit();


	protected override void Start()
	{
		base.Start();

    }

    public override void Init()
    {
        base.Init();

        _mainCamera = CameraManager.I._MainCamera;
        _player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		//_tribeAgent = ((GameObject)ObjectsManager.I["TribeGroundPosition"]).GetComponent<NavMeshAgent>();
		_Tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

		InputManager.I.onBeaconPlaceButtonPressed.AddListener(PlaceBeacon);
        InputManager.I.onBeaconActivateKeyPressed.AddListener(BeaconActivation);

        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        Player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.CompassEnergyLowUnusable) IsEnabled = false; });
        Player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.CompassEnergyLowUnusable) IsEnabled = true; });
    }

    void PlaceBeacon()
	{
		if (!IsEnabled)
			return;

        if (GameManager.I._data.BeaconPlacementCount > 0 && Player.PlacedBeacon.Count >= GameManager.I._data.BeaconPlacementCount)
        {
            //UIManager.I.AlertMessage("The maximum beacon count has been reached.");
            return;
        }

        if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.BeaconPlacerUsable))
        {
            //UIManager.I.AlertMessage("Unable to place beacon now.");
            return;
        }

        if (_player.IsTooFar)
        {
            //UIManager.I.AlertMessage("You are too far from tribe.");
            return;
        }

		if (Physics.Raycast(_mainCamera.transform.position + Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized * GameManager.I._data.BeaconPlacementDistance, Vector3.down, out _HitInfo, 100.0f, DropableLayers))
		{
			Item beacon =	Instantiate(_beaconPrefab);

            beacon.transform.position = _HitInfo.point;
            Player.PlacedBeacon.Add(beacon);
		}
	}
    public void ActivateBeacon(Item beacon)
    {
		//_tribeAgent.destination = new Vector3(beacon.transform.position.x, 0, beacon.transform.position.z);
		_Tribe.ModeGoToBeacon(new Vector3(beacon.transform.position.x, 0, beacon.transform.position.z));
        beacon.GetComponentInChildren<Animator>().SetBool("IsOpened", true);
    }
    public void DeactivateBeacon(Item beacon)
    {
        beacon.GetComponentInChildren<Animator>().SetBool("IsOpened", false);
    }

    public void BeaconActivation()
    {
        if (Player.PlacedBeacon == null || Player.PlacedBeacon.Count <= 0)
            return;

        Player.PlacedBeacon.ForEach(b => DeactivateBeacon(b));
        Player.CurrentBeaconIndex++;
        if (Player.CurrentBeaconIndex == Player.PlacedBeacon.Count)
        {
            Player.CurrentBeaconIndex = -1;
            return;
        }
        ActivateBeacon(Player.PlacedBeacon[(int)Mathf.Repeat(Player.CurrentBeaconIndex, Player.PlacedBeacon.Count)]);
    }
}
