using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct OpeningDoorProperties
{
	public DayState DayState;
	public List<Vector2> OpeningHoursInDayState;
}

public class Door : BaseMonoBehaviour
{
	[Title("Door")]
	public float DoorClosedAngle = 0f;
	public float DoorOpenAngle = 90f;
	public float OpeningSpeed = 5f;

	[Title("Colliders size")]
	public float XSizeColliders = 1f;
	public float YSizeColliders = 3f;
	public float ZSizeColliders = 0.3f;

	[ToggleGroup("DayStateMode")]
	public bool DayStateMode = false;
	[ToggleGroup("DayStateMode")]
	public List<DayState> OpeningDayStates;
	[ToggleGroup("DayStateMode")]
	public List<DayState> ClosingDayStates;

	[ToggleGroup("TimeOfDayMode")]
	public bool TimeOfDayMode = false;
	[ToggleGroup("TimeOfDayMode")]
	public List<OpeningDoorProperties> DoorOpeningHours;
	

	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	Transform LeftDoor;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	Transform RightDoor;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	BoxCollider LeftCollider;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	BoxCollider RightCollider;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	public bool isOpen = false;

	Coroutine InOpeningDoor;
	Coroutine InClosingDoor;

	private void OnValidate()
	{
		#region  Checking prefab references for script
		if (LeftDoor == null)
			LeftDoor = transform.GetChild(1);
		if (RightDoor == null)
			RightDoor = transform.GetChild(2);
		if (LeftCollider == null)
			LeftCollider = LeftDoor.GetComponentInChildren<BoxCollider>();
		if (RightCollider == null)
			RightCollider = RightDoor.GetComponentInChildren<BoxCollider>();
		#endregion

		#region Change closed door angle with inspector
		SetDoorCurrentAngle(DoorClosedAngle);
		#endregion

		#region Change colliders size with inspector
		LeftCollider.size = new Vector3(XSizeColliders, YSizeColliders, ZSizeColliders);
		LeftCollider.center = new Vector3(XSizeColliders / 2, YSizeColliders / 2, LeftCollider.center.z);
		LeftDoor.localPosition = new Vector3(-XSizeColliders, LeftDoor.localPosition.y, LeftDoor.localPosition.z);

		RightCollider.size = new Vector3(XSizeColliders, YSizeColliders, ZSizeColliders);
		RightCollider.center = new Vector3(-XSizeColliders / 2, YSizeColliders /2, RightCollider.center.z);
		RightDoor.localPosition = new Vector3(XSizeColliders, RightDoor.localPosition.y, RightDoor.localPosition.z);
		#endregion
	}

	private void Update()
	{
		Debug.Log("--------------");
		if (GamePaused)
			return;

		if (DayStateMode == true)
			DoorDayStateMode();

		if (TimeOfDayMode == true)
			DoorTimeOfDayMode();

		//Debug.Log(Mathf.Round(AmbiantManager.I.CurrentTimeOfDay) + "H");
		Debug.Log("Actual Timer = " + AmbiantManager.I.CurrentTimeOfDay);
		Debug.Log("Actual State = " + AmbiantManager.I.CurrentDayState.State.ToString());
		Debug.Log("--------------");
	}

	public void DoorTimeOfDayMode()
	{
		bool _IsOpeningHour = false;
		//DayState _CurrentDayState = AmbiantManager.I.CurrentDayState.State;
		float _CurrentTime = AmbiantManager.I.CurrentTimeOfDay;

		Debug.Log("Count = " + DoorOpeningHours.Count);

		for (int i = 0; i < DoorOpeningHours.Count; i++)
		{
			Debug.Log("--- " + DoorOpeningHours[i].DayState.ToString());

			//if (DoorOpeningHours[i].DayState != AmbiantManager.I.CurrentDayState.State/*_CurrentDayState*/)
			//	break;

			//foreach (Vector2 _DoorOpeningHour in DoorOpeningHours[i].OpeningHoursInDayState)
			//	if (_CurrentTime >= _DoorOpeningHour.x && _CurrentTime < _DoorOpeningHour.y)
			//		_IsOpeningHour = true;

			if (DoorOpeningHours[i].DayState == AmbiantManager.I.CurrentDayState.State/*_CurrentDayState*/)
			{
				foreach (Vector2 _DoorOpeningHour in DoorOpeningHours[i].OpeningHoursInDayState)
				if (_CurrentTime >= _DoorOpeningHour.x && _CurrentTime < _DoorOpeningHour.y)
					_IsOpeningHour = true;
			}
		}

		if (!isOpen && _IsOpeningHour)
			OpenDoor();
		else if (isOpen && !_IsOpeningHour)
			CloseDoor();
	}

	public void DoorDayStateMode()
	{
		if (!isOpen && OpeningDayStates.Contains(AmbiantManager.I.CurrentDayState.State))
			OpenDoor();

		else if (isOpen && ClosingDayStates.Contains(AmbiantManager.I.CurrentDayState.State))
			CloseDoor();
	}

	public void SetDoorCurrentAngle(float currentAngle)
	{
		LeftDoor.transform.eulerAngles = new Vector3(0, currentAngle, 0);
		RightDoor.transform.eulerAngles = new Vector3(0, -currentAngle, 0);
	}

	[Button("Open Door (Playmode only)")]
	public void OpenDoor()
	{
		if (InOpeningDoor == null && InClosingDoor == null)
			InOpeningDoor = StartCoroutine(OpeningDoor());
		else
			Debug.Log("Animation en cours!");
	}
	IEnumerator OpeningDoor()
	{
		float _DoorCurrentAngle = DoorClosedAngle;

		while (_DoorCurrentAngle < DoorOpenAngle)
		{
			_DoorCurrentAngle += OpeningSpeed * Time.deltaTime;

			if (_DoorCurrentAngle > DoorOpenAngle)
				_DoorCurrentAngle = DoorOpenAngle;

			SetDoorCurrentAngle(_DoorCurrentAngle);

			yield return null;
		}

		Debug.Log("Fin Ouverture");
		isOpen = true;
		InOpeningDoor = null;
		yield return null;
	}

	[Button("Close Door (Playmode only)")]
	public void CloseDoor()
	{
		if (InOpeningDoor == null && InClosingDoor == null)
			InClosingDoor = StartCoroutine(ClosingDoor());
		else
			Debug.Log("Animation en cours!");
	}
	IEnumerator ClosingDoor()
	{
		float _DoorCurrentAngle = DoorOpenAngle;

		while (_DoorCurrentAngle > DoorClosedAngle)
		{
			_DoorCurrentAngle -= OpeningSpeed * Time.deltaTime;

			if (_DoorCurrentAngle < DoorClosedAngle)
				_DoorCurrentAngle = DoorClosedAngle;

			SetDoorCurrentAngle(_DoorCurrentAngle);

			yield return null;
		}

		Debug.Log("Fin Fermeture");
		isOpen = false;
		InClosingDoor = null;
		yield return null;
	}
}
