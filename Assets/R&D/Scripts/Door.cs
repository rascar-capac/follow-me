using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DoorProperties
{
	[Title("Status")][Tooltip("Open the door at Start")][PropertySpace(SpaceBefore = 5)]
	public bool IsOpenOnStart;
	[Tooltip("The player and door can interacts with Y Key")]
	public bool IsInteractable;

	[Title("Opening options")]
	public float OpeningSpeed;
	public float DoorClosedAngle;
	public float DoorOpenAngle;

	[Title("Colliders size")]
	public float XSizeColliders;
	public float YSizeColliders;
	[PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
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
	public List<Vector2Int> OpeningHoursInDayState; // x = Start opening, y = Start closing;
}

public class Door : BaseMonoBehaviour
{
	[ShowInInspector][ReadOnly]
	public bool IsOpen; // Controlled by Door Mode if Mode activated
	[PropertySpace(SpaceBefore = 5, SpaceAfter = 10)]
	public DoorProperties DoorProperties;

	#region Events Door

	[FoldoutGroup("Events Door")]
	public UnityEvent onBeginOpeningDoor = new UnityEvent();
	[FoldoutGroup("Events Door")]
	public UnityEvent onDoorOpen = new UnityEvent();
	[FoldoutGroup("Events Door")]
	public UnityEvent onBeginClosingDoor = new UnityEvent();
	[FoldoutGroup("Events Door")]
	public UnityEvent onDoorClosed = new UnityEvent();

	#endregion

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

		if (DoorProperties.IsOpenOnStart)
			OpenDoor();

		if (DoorProperties.DayStateMode)
		{
			AmbiantManager.I.onDayStateChanged.AddListener(DoorDayStateMode);
			DoorDayStateMode(AmbiantManager.I.CurrentDayState, AmbiantManager.I.NextDayState);
		}

		if (DoorProperties.TimeOfDayMode)
		{
			AmbiantManager.I.onHourChanged.AddListener(DoorTimeOfDayMode);
			DoorTimeOfDayMode(Mathf.RoundToInt(AmbiantManager.I.CurrentTimeOfDay), AmbiantManager.I.CurrentDayState.State);
		}
	}

	public void SetDoorCurrentAngle(float currentAngle)
	{
		LeftDoor.transform.localEulerAngles = new Vector3(LeftDoor.localEulerAngles.x, currentAngle, LeftDoor.localEulerAngles.z);
		RightDoor.transform.localEulerAngles = new Vector3(RightDoor.localEulerAngles.x, -currentAngle, RightDoor.localEulerAngles.z);
	}

	public void InteractWithDoor()
	{
		if (!DoorProperties.IsInteractable)
			return;

		if (!IsOpen)
			OpenDoor();
		else
			CloseDoor();
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

	void DoorTimeOfDayMode(int currentHour, DayState currentDayState)
	{
		if (GamePaused || !DoorProperties.TimeOfDayMode)
			return;

		bool _IsOpeningHour = false;

		for (int i = 0; i < DoorProperties.DoorOpeningHours.Count; i++)
		{
			if (DoorProperties.DoorOpeningHours[i].DayState == currentDayState)
			{
				foreach (Vector2Int _DoorOpeningHour in DoorProperties.DoorOpeningHours[i].OpeningHoursInDayState)
					if (currentHour >= _DoorOpeningHour.x && currentHour < _DoorOpeningHour.y)
						_IsOpeningHour = true;
			}

			// Pourquoi ceçi ne fonctionne pas ?
			//if (DoorProperties.DoorOpeningHours[i].DayState != currentDayState)
			//	break;

			//foreach (Vector2 _DoorOpeningHour in DoorProperties.DoorOpeningHours[i].OpeningHoursInDayState)
			//	if (currentHour >= _DoorOpeningHour.x && currentHour < _DoorOpeningHour.y)
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
		onBeginOpeningDoor.Invoke();

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
		onBeginClosingDoor.Invoke();

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
