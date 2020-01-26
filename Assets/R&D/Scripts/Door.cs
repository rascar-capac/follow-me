using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


[System.Serializable]
public struct DoorProperties
{
	[Title("Door")]
	public float OpeningSpeed;
	public float DoorClosedAngle;
	public float DoorOpenAngle;

	[Title("Colliders size")]
	public float XSizeColliders;
	public float YSizeColliders;
	public float ZSizeColliders;

	[ToggleGroup("DayStateMode")]
	public bool DayStateMode;
	[ToggleGroup("DayStateMode")]
	public List<DayState> OpeningDayStates;
	[ToggleGroup("DayStateMode")]
	public List<DayState> ClosingDayStates;

	[ToggleGroup("TimeOfDayMode")]
	public bool TimeOfDayMode;
	[ToggleGroup("TimeOfDayMode")]
	public List<OpeningHoursProperties> DoorOpeningHours;
}

[System.Serializable]
public struct OpeningHoursProperties
{
	public DayState DayState;
	public List<Vector2> OpeningHoursInDayState; // x = Start opening, y = Start closing;
}


public class Door : BaseMonoBehaviour
{
	[ShowInInspector][ReadOnly][Space(5)][GUIColor(1, 0, 0, 0.6f)]
	public bool IsOpen = false; // Controlled by Door Mode

	[Space(5)]
	public DoorProperties DoorProperties;

	[Space(5)]
	public UnityEvent onDoorOpen = new UnityEvent();
	public UnityEvent onDoorClosed = new UnityEvent();

	Coroutine _InOpeningDoor;
	Coroutine _InClosingDoor;

	#region Debug Mode
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	Transform LeftDoor;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	Transform RightDoor;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	BoxCollider LeftCollider;
	[FoldoutGroup("DebugMode")][ShowInInspector][ReadOnly]
	BoxCollider RightCollider;
	#endregion

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
		SetDoorCurrentAngle(DoorProperties.DoorClosedAngle);
		#endregion

		#region Change colliders size with inspector
		LeftCollider.size = new Vector3(DoorProperties.XSizeColliders, DoorProperties.YSizeColliders, DoorProperties.ZSizeColliders);
		LeftCollider.center = new Vector3(DoorProperties.XSizeColliders / 2, DoorProperties.YSizeColliders / 2, LeftCollider.center.z);
		LeftDoor.localPosition = new Vector3(-DoorProperties.XSizeColliders, LeftDoor.localPosition.y, LeftDoor.localPosition.z);

		RightCollider.size = new Vector3(DoorProperties.XSizeColliders, DoorProperties.YSizeColliders, DoorProperties.ZSizeColliders);
		RightCollider.center = new Vector3(-DoorProperties.XSizeColliders / 2, DoorProperties.YSizeColliders / 2, RightCollider.center.z);
		RightDoor.localPosition = new Vector3(DoorProperties.XSizeColliders, RightDoor.localPosition.y, RightDoor.localPosition.z);
		#endregion
	}

	protected override void Start()
	{
		base.Start();

		AmbiantManager.I.onDayStateChanged.AddListener(DoorDayStateMode);

	}


	private void Update()
	{
		if (GamePaused)
			return;

		// Créer un Event à chaque heure changée ?
		if (DoorProperties.TimeOfDayMode == true)
			DoorTimeOfDayMode();
	}

	public void SetDoorCurrentAngle(float currentAngle)
	{
		LeftDoor.transform.eulerAngles = new Vector3(0, currentAngle, 0);
		RightDoor.transform.eulerAngles = new Vector3(0, -currentAngle, 0);
	}

	#region Door Modes

	void DoorDayStateMode(DayStatesProperties CurrentState, DayStatesProperties NextState)
	{
		if (GamePaused || !DoorProperties.DayStateMode)
			return;

		if (!IsOpen && DoorProperties.OpeningDayStates.Contains(CurrentState.State))
			OpenDoor();

		else if (IsOpen && DoorProperties.ClosingDayStates.Contains(CurrentState.State))
			CloseDoor();
	}

	void DoorTimeOfDayMode()
	{
		bool _IsOpeningHour = false;
		float _CurrentTime = AmbiantManager.I.CurrentTimeOfDay;
		//DayState _CurrentDayState = AmbiantManager.I.CurrentDayState.State; // Pourquoi ceçi ne fonctionne pas ?

		for (int i = 0; i < DoorProperties.DoorOpeningHours.Count; i++)
		{
			if (DoorProperties.DoorOpeningHours[i].DayState == AmbiantManager.I.CurrentDayState.State/*_CurrentDayState*/)
			{
				foreach (Vector2 _DoorOpeningHour in DoorProperties.DoorOpeningHours[i].OpeningHoursInDayState)
					if (_CurrentTime >= _DoorOpeningHour.x && _CurrentTime < _DoorOpeningHour.y)
						_IsOpeningHour = true;
			}

			// Et pourquoi ceçi ne fonctionne pas également ?
			//if (DoorOpeningHours[i].DayState != AmbiantManager.I.CurrentDayState.State/*_CurrentDayState*/)
			//	break;

			//foreach (Vector2 _DoorOpeningHour in DoorOpeningHours[i].OpeningHoursInDayState)
			//	if (_CurrentTime >= _DoorOpeningHour.x && _CurrentTime < _DoorOpeningHour.y)
			//		_IsOpeningHour = true;
		}

		if (!IsOpen && _IsOpeningHour)
			OpenDoor();
		else if (IsOpen && !_IsOpeningHour)
			CloseDoor();
	}

	#endregion


	#region Opening Door

	[FoldoutGroup("DebugMode")][Button("Open Door (Playmode only)")]
	public void OpenDoor()
	{
		if (_InOpeningDoor == null && _InClosingDoor == null)
			_InOpeningDoor = StartCoroutine(OpeningDoor());
	}

	IEnumerator OpeningDoor()
	{
		float _DoorCurrentAngle = DoorProperties.DoorClosedAngle;

		while (_DoorCurrentAngle < DoorProperties.DoorOpenAngle)
		{
			_DoorCurrentAngle += DoorProperties.OpeningSpeed * Time.deltaTime;

			if (_DoorCurrentAngle > DoorProperties.DoorOpenAngle)
				_DoorCurrentAngle = DoorProperties.DoorOpenAngle;

			SetDoorCurrentAngle(_DoorCurrentAngle);

			yield return null;
		}

		onDoorOpen.Invoke();
		IsOpen = true;
		_InOpeningDoor = null;
		yield return null;
	}

	#endregion


	#region Closing Door

	[FoldoutGroup("DebugMode")]	[Button("Close Door (Playmode only)")]
	public void CloseDoor()
	{
		if (_InOpeningDoor == null && _InClosingDoor == null)
			_InClosingDoor = StartCoroutine(ClosingDoor());
	}

	IEnumerator ClosingDoor()
	{
		float _DoorCurrentAngle = DoorProperties.DoorOpenAngle;

		while (_DoorCurrentAngle > DoorProperties.DoorClosedAngle)
		{
			_DoorCurrentAngle -= DoorProperties.OpeningSpeed * Time.deltaTime;

			if (_DoorCurrentAngle < DoorProperties.DoorClosedAngle)
				_DoorCurrentAngle = DoorProperties.DoorClosedAngle;

			SetDoorCurrentAngle(_DoorCurrentAngle);

			yield return null;
		}

		onDoorClosed.Invoke();
		IsOpen = false;
		_InClosingDoor = null;
		yield return null;
	}

	#endregion

}
