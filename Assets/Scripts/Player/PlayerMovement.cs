using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : BaseMonoBehaviour
{
	[HideInInspector]
    public CharacterController _controller;
    [TabGroup("Properties")][Header("Current Speed in m/s of the player")]
    public float _speed = 12f;
	[TabGroup("Properties")][Header("The gravity force")]
    public float _gravity = -9.81f;
	[TabGroup("Properties")][Header("The maximum height of jump")]
    public float _jumpHeight = 3f;
	[TabGroup("Properties")]
	public float PlayerRunStamina = 100f;
	[TabGroup("Properties")]
	public float TribeDistance = 0.1f;

	[TabGroup("States")]
	public bool InGame = true;
	[TabGroup("States")]
	public bool _isGrounded;
	[TabGroup("States")]
	public bool IsWaiting = true;
	[TabGroup("States")][Header("Tribe is too far")]
	public bool IsTooFar = false;
	[TabGroup("States")][Header("Player is running")]
    public bool IsRunning = false;
	[TabGroup("States")][ShowInInspector][ReadOnly]
	bool IsRunStaminaNull => PlayerRunStamina == 0;
	[TabGroup("States")][ShowInInspector][ReadOnly]
	bool PlayerMayRun = true;
	[TabGroup("States")][ShowInInspector][ReadOnly]
	bool _isMoveAllowed = true;

    [TabGroup("Ground Detection")][Header("The game object bottom of the player to check he is grounded.")]
    public Transform _groundCheck;
	[TabGroup("Ground Detection")]
	public float _groundDistance = 0.0f;
	[TabGroup("Ground Detection")][Header("The Layer of the ground or element on which we can walk.")]
    public LayerMask _groundMask;

	// For Slope Detection RENOCK, remove if useless.
	[TabGroup("Slope Detection")]
	public Vector3 HitAngle;
	[TabGroup("Slope Detection")]
	public Vector3 OffsetStartRaycast;
	[TabGroup("Slope Detection")]
	public float RayLenght;
	RaycastHit HitForward;
	[TabGroup("Slope Detection")][ReadOnly]
	public float HitForwardSlopeAngle;
	RaycastHit HitBackward;
	[TabGroup("Slope Detection")][ReadOnly]
	public float HitBackwardSlopeAngle;
	RaycastHit HitRight;
	[TabGroup("Slope Detection")][ReadOnly]
	public float HitRightSlopeAngle;
	RaycastHit HitLeft;
	[TabGroup("Slope Detection")][ReadOnly]
	public float HitLeftSlopeAngle;

	[TabGroup("Events")]
    public UnityEvent onPlayerRunStaminaNullEnter = new UnityEvent();
	[TabGroup("Events")]
	public UnityEvent onPlayerRunStaminaNullExit = new UnityEvent();
	[TabGroup("Events")]
	public UnityEvent onPlayerTooFarFromTribe = new UnityEvent();
	[TabGroup("Events")]
	public PlayerHasMovedEvent onPlayerHasMoved = new PlayerHasMovedEvent();

    Player _player;
    GameObject Tribe;
	float _MinDistForTribeAcceleration;
    Vector3 _move;
    Vector3 _velocity;


	protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        //Tribe = (GameObject)ObjectsManager.I["TribeGroundPosition"];
		Tribe = (GameObject)ObjectsManager.I["Tribe"];
		//_MinDistForTribeAcceleration = GameManager.I._data.TribeProperties.MinDistForAcceleration;
        _speed = GameManager.I._data.InitialPlayerSpeed;
        _player = GetComponent<Player>();
        //PlayerRunStamina = GameManager.I._data.PlayerRunStaminaMax;
        //onPlayerRunStaminaNullEnter.AddListener(() => { PlayerMayRun = false; });
        //onPlayerRunStaminaNullExit.AddListener(() => {  PlayerMayRun = true; });

        InputManager.I.onRunButtonPressed.AddListener(SwitchRun);
        InputManager.I.onMoveInputAxisEvent.AddListener(Move);
        InputManager.I.onStopMoveInputAxisEvent.AddListener(Wait);
        InputManager.I.onPlayerJumpPressed.AddListener(Jump);
        //UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { InGame = true; });
        //UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { InGame = false; });
    }

    private void Update()
    {
        //ComputeTribeDistance();
        //UpdateRunStamina();
        //RunStaminaCritical();
    }

    //private void FixedUpdate()
    //{
    //    Debug.DrawRay(CameraManager.I._MainCamera.transform.position + _move * 0.5f, Vector3.down * 5, Color.white);
    //    RaycastHit hit;
    //    if (Physics.Raycast(CameraManager.I._MainCamera.transform.position + _move * 0.5f, Vector3.down, out hit, 5, _groundMask))
    //    {
    //        float groundAngle = Vector3.Angle(Vector3.up, hit.normal);

    //        _isMoveAllowed  = groundAngle <= _controller.slopeLimit;
    //    }
    //}

    void SwitchRun()
    {
        if (!IsRunning && !IsWaiting && PlayerMayRun)
            IsRunning = true;
        else
            IsRunning = false;
    }

    #region MOVE version Renock

    //public void Move(InputAxisUnityEventArg axis)
    //{
    //	if (!InGame)
    //		return;

    //	#region Check Ground Position

    //	_isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
    //	if (_isGrounded && _velocity.y < 0)
    //	{
    //		_velocity.y = -2f;
    //	}

    //	#endregion

    //	#region Check Inputs

    //	_move = new Vector3(axis.XValue, 0, axis.YValue);

    //	#endregion

    //	#region Check Slopes

    //	Vector3.Normalize(HitAngle);

    //	// Set par défaut des angles à la valeur de SlopeLimit.
    //	HitForwardSlopeAngle = _controller.slopeLimit;
    //	HitBackwardSlopeAngle = _controller.slopeLimit;
    //	HitRightSlopeAngle = _controller.slopeLimit;
    //	HitLeftSlopeAngle = _controller.slopeLimit;

    //	// Debug des prochain rayons tirés.
    //	Debug.DrawRay(transform.position + OffsetStartRaycast, (transform.forward + HitAngle) * RayLenght, Color.blue, 0.1f);
    //	Debug.DrawRay(transform.position + OffsetStartRaycast, (-transform.forward + HitAngle) * RayLenght, Color.blue, 0.1f);
    //	Debug.DrawRay(transform.position + OffsetStartRaycast, (transform.right + HitAngle) * RayLenght, Color.blue, 0.1f);
    //	Debug.DrawRay(transform.position + OffsetStartRaycast, (-transform.right + HitAngle) * RayLenght, Color.blue, 0.1f);

    //	// Tir des 4 rayons + calcul de l'angle des pentes touchées
    //	if (Physics.Raycast(transform.position + OffsetStartRaycast, transform.forward + HitAngle, out HitForward, RayLenght, _groundMask))
    //		HitForwardSlopeAngle = Mathf.Abs(Vector3.Angle(HitForward.normal, transform.forward) - 90f);
    //	if (Physics.Raycast(transform.position + OffsetStartRaycast, -transform.forward + HitAngle, out HitBackward, RayLenght, _groundMask))
    //		HitBackwardSlopeAngle = Mathf.Abs(Vector3.Angle(HitBackward.normal, -transform.forward) - 90f);
    //	if (Physics.Raycast(transform.position + OffsetStartRaycast, transform.right + HitAngle, out HitRight, RayLenght, _groundMask))
    //		HitRightSlopeAngle = Mathf.Abs(Vector3.Angle(HitRight.normal, transform.right) - 90f);
    //	if (Physics.Raycast(transform.position + OffsetStartRaycast, -transform.right + HitAngle, out HitLeft, RayLenght, _groundMask))
    //		HitLeftSlopeAngle = Mathf.Abs(Vector3.Angle(HitLeft.normal, -transform.right) - 90f);

    //	// Si un des angles dépasse la valeur de SlopeLimit, annulation du mouvement dans la direction correspondante.
    //	if (HitForwardSlopeAngle >= _controller.slopeLimit)
    //		_move = new Vector3(_move.x, _move.y, Mathf.Clamp(_move.z, -1, 0));
    //	if (HitBackwardSlopeAngle >= _controller.slopeLimit)
    //		_move = new Vector3(_move.x, _move.y, Mathf.Clamp(_move.z, 0, 1));
    //	if (HitRightSlopeAngle >= _controller.slopeLimit)
    //		_move = new Vector3(Mathf.Clamp(_move.x, -1, 0), _move.y, _move.z);
    //	if (HitLeftSlopeAngle >= _controller.slopeLimit)
    //		_move = new Vector3(Mathf.Clamp(_move.x, 0, 1), _move.y, _move.z);

    //	#endregion

    //	#region Apply Sound

    //	if (_move == Vector3.zero)
    //	{
    //		SoundManager.I.StopPlayerSound();
    //	}
    //	else
    //	{
    //		//SoundManager.I.PlayPlayer("Walk");
    //		SoundManager.I.PlayerWalk();
    //	}

    //	#endregion

    //	#region Setup Run Multiplicator

    //	float runMultiply = 1;
    //	if (IsRunning && AmbiantManager.I.IsUsableNow(GameManager.I._data.PlayerRunUsable))
    //		runMultiply = GameManager.I._data.SpeedMultiplicator;

    //	#endregion

    //	#region Apply Movements

    //	if (_isMoveAllowed)
    //	{
    //		// Application du mouvement après transformation de la direction global (_move) en direction local.
    //		_controller.Move(transform.TransformDirection(_move) * _speed * runMultiply * Time.deltaTime);
    //	}

    //	#endregion

    //	#region Jump

    //	//if (Input.GetButtonDown("Jump") && _isGrounded)
    //	//{
    //	//	_velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
    //	//}

    //	#endregion

    //	#region Apply Gravity

    //		_velocity.y += _gravity * Time.deltaTime;
    //		_controller.Move(_velocity * Time.deltaTime);

    //	#endregion

    //	IsWaiting = false;
    //}
    //private void OnDrawGizmos()
    //{
    //	// Debug des points touchés par les rayons.
    //	Gizmos.DrawIcon(HitForward.point, "HitForward");
    //	Gizmos.DrawIcon(HitBackward.point, "HitBackward");
    //	Gizmos.DrawIcon(HitRight.point, "HitRight");
    //	Gizmos.DrawIcon(HitLeft.point, "HitLeft");
    //}

    #endregion

    #region MOVE Previous version

    public void Move(InputAxisUnityEventArg axis)
    {

        if (!InGame)
            return;

        #region Check Ground Position

        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        #endregion

        #region Check Inputs

        float x = axis.XValue;
        float z = axis.YValue;
        Vector3 right = transform.right * x;
        Vector3 forward = transform.forward * z;

        #endregion

        #region Check Slopes

        //// WITH MATHF.ATAN2 (voir Théo)

        Vector3 cameraPosition = CameraManager.I._MainCamera.transform.position;
        Vector3 rayCastOrigin = cameraPosition + right * _speed * Time.deltaTime;
        RaycastHit hit;
        _move = Vector3.zero;
        //if (Physics.Raycast(rayCastOrigin, Vector3.down, out hit, 5, _groundMask))
        //{

        //    if (Mathf.Atan2(hit.distance - Vector3.Distance(cameraPosition, _groundCheck.position), x * _speed * Time.deltaTime) <= _controller.slopeLimit)
        //        _move += right;
        //    else
        //    {
        //        Vector3 shiftVector = Vector3.Cross(Vector3.up, right);
        //        if (Vector3.Dot(hit.transform.position - transform.position, transform.forward) > 0)
        //        {
        //            _move += shiftVector;
        //        }
        //        else
        //            _move += shiftVector * -1;
        //    }
        //}
        //else
        _move += right;

        //rayCastOrigin = cameraPosition + forward * _speed * Time.deltaTime;

        //if (Physics.Raycast(rayCastOrigin, Vector3.down, out hit, 5, _groundMask))
        //{

        //    if (Mathf.Atan2(hit.distance - Vector3.Distance(cameraPosition, _groundCheck.position), z * _speed * Time.deltaTime) <= _controller.slopeLimit)
        //        _move += forward;
        //    else
        //    {
        //        Vector3 shiftVector = Vector3.Cross(Vector3.up, forward);
        //        if (Vector3.Dot(hit.transform.position - transform.position, transform.right) > 0)
        //        {
        //            _move += shiftVector;
        //        }
        //        else
        //            _move += shiftVector * -1;
        //    }
        //}
        //else
        _move += forward;



        // WITH VECTOR3.ANGLE

        //_move = Vector3.zero;
        ////RaycastHit hit;
        //if (Physics.Raycast(CameraManager.I._MainCamera.transform.position + right * 0.5f, Vector3.down, out hit, 5, _groundMask))
        //{
        //    float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
        //    if (groundAngle <= _controller.slopeLimit - 0.1f)
        //        _move += right;
        //    else
        //    {
        //        if (x > 0)
        //            _move += Vector3.Cross(right, Vector3.up) ;
        //        else
        //            _move += Vector3.Cross(Vector3.up, right);
        //    }
        //    //_isMoveAllowed = groundAngle <= _controller.slopeLimit;
        //}
        //else
        //    _move += right;

        //if (Physics.Raycast(CameraManager.I._MainCamera.transform.position + forward * 0.5f, Vector3.down, out hit, 5, _groundMask))
        //{
        //    float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
        //    if (groundAngle <= _controller.slopeLimit)
        //        _move += forward;
        //    else
        //    {
        //        float hitdist = 3f;
        //        Vector3 shiftVector = Vector3.Cross(Vector3.up, forward);
        //        if (Vector3.Dot(hit.transform.position - transform.position, transform.right) > 0)
        //        {
        //            _move += shiftVector;
        //        }
        //        else
        //            _move += shiftVector * -1;

        //        //if (Physics.Raycast(CameraManager.I._MainCamera.transform.position, shiftVector, out hit, hitdist, _groundMask))
        //        //{
        //        //    Debug.DrawRay(CameraManager.I._MainCamera.transform.position, shiftVector * hitdist * -1, Color.black);
        //        //    Debug.Log("hit");
        //        //}
        //        //else
        //        //{
        //        //    Debug.DrawRay(CameraManager.I._MainCamera.transform.position, shiftVector * hitdist, Color.white);
        //       // _move += shiftVector;
        //        //}
        //    }
        //    //_isMoveAllowed = groundAngle <= _controller.slopeLimit;
        //}
        //else
        //    _move += forward;

        #endregion

        #region Apply Sound

        if (_move == Vector3.zero)
        {
            SoundManager.I.IsWalking = false;
        }
        else
        {
            //SoundManager.I.PlayPlayer("Walk");
            SoundManager.I.IsWalking = true;
        }
        //_move = transform.right * x + transform.forward * z;

        #endregion

        #region Setup Run Multiplicator

        float runMultiply = 1;
        if (IsRunning && AmbiantManager.I.IsUsableNow(GameManager.I._data.PlayerRunUsable))
            runMultiply = GameManager.I._data.SpeedMultiplicator;

        #endregion

        #region Apply Movements

        //if (_isMoveAllowed)
        //{
        //    _controller.Move(_move * _speed * runMultiply * Time.deltaTime);
        //}
        _controller.Move(_move * _speed * runMultiply * Time.deltaTime);
        #endregion

        #region Jump

        // if (Input.GetButtonDown("Jump") && _isGrounded)
        // {
        //    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        // }

        #endregion

        #region Apply Gravity

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        #endregion

        IsWaiting = false;
    }

    #endregion

    public void Wait()
    {
        IsWaiting = true;
        IsRunning = false;
    }

    public void Jump()
    {
        if (!_isGrounded)
            return;

        _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
    }

    void ComputeTribeDistance()
    {
        Vector3 TribePositionProjected = Vector3.ProjectOnPlane(Tribe.transform.position, Vector3.up);
        Vector3 PlayerPositionProjected = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        TribeDistance = Vector3.Distance(PlayerPositionProjected, TribePositionProjected);
        IsTooFar = TribeDistance > GameManager.I._data.MaximumDistanceOfTribe;
        if(IsTooFar)
        {
            onPlayerTooFarFromTribe?.Invoke();
        }

		if (TribeDistance > _MinDistForTribeAcceleration)
			onPlayerHasMoved.Invoke(transform.position);
        //UIManager.I.SetTribeDistance();
    }

    void ResetSpeed()
    {
        _speed = GameManager.I._data.InitialPlayerSpeed;
    }

    public void UpdateRunStamina()
    {
        if(GameManager.I._data.PlayerRunStamina)
        {
            if (IsRunning)
                PlayerRunStamina -= GameManager.I._data.PlayerRunCostBySecond * Time.deltaTime;
            else
                PlayerRunStamina += GameManager.I._data.PlayerRunGainBySecond * Time.deltaTime;

            PlayerRunStamina = Mathf.Clamp(PlayerRunStamina, 0, GameManager.I._data.PlayerRunStaminaMax);

            //UIManager.I.SetRunStamina(PlayerRunStamina);
        }
    }

    void RunStaminaCritical()
	{
		if(IsRunStaminaNull)
		{
			onPlayerRunStaminaNullEnter.Invoke();
            IsRunning = false;
		}
		else
		{
            onPlayerRunStaminaNullExit.Invoke();
		}
	}
}

public class PlayerHasMovedEvent : UnityEvent<Vector3> { }