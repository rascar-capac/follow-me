using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Demo_TribeFollowLeader : MonoBehaviour
{
	Transform _tribeLeader;
	NavMeshAgent _tribeAgent;
	Vector3 _lastPositionLeader;

	private void Awake()
	{
		_tribeLeader = GameObject.Find("Tribe").GetComponent<Transform>();
		_tribeAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if(_lastPositionLeader != _tribeLeader.transform.position)
		{
			_tribeAgent.destination = _tribeLeader.transform.position;
			_lastPositionLeader = _tribeLeader.transform.position;
		}
	}
}
