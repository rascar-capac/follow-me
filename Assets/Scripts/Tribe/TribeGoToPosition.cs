using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TribeGoToPosition : BaseMonoBehaviour
{
	public NavMeshAgent _tribeAgent;
	public Camera _mainCamera;
	public KeyCode _inputAction;
	public GameObject _tribeFollowTarget;

	RaycastHit _hitInfo = new RaycastHit();

	private void Start()
	{
		_tribeAgent.destination = _tribeFollowTarget.transform.position;
	}

	void Update()
	{
		if (Input.GetKeyDown(_inputAction))
		{
			if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out _hitInfo, 100.0f))
			{
				_tribeFollowTarget.transform.position = _hitInfo.point;
				_tribeAgent.destination = _tribeFollowTarget.transform.position;

				Debug.Log(_hitInfo.transform.gameObject.layer);
			}
		}
	}
}
