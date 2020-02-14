using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : BaseMonoBehaviour
{
    public CharacterController _controller;
    [Header("Current Speed in m/s of the player")]
    public float _speed = 12f;
    [Header("The gravity force")]
    public float _gravity = -9.81f;
    [Header("The maximum height of jump")]
    public float _jumpHeight = 3f;
    [Header("The game object bottom of the player to check he is grounded.")]
    public Transform _groundCheck;
    public float _groundDistance = 0.0f;
    [Header("The Layer of the ground or element on which we can walk.")]
    public LayerMask _groundMask;

    [Header("Player is running")]
    public bool IsRunning = false;
    public bool IsWaiting = true;
    public float PlayerRunStamina = 100f;
    bool IsRunStaminaNull => PlayerRunStamina == 0;
    bool PlayerMayRun = true;
    bool _isMoveAllowed = true;
    Vector3 _move;
    Vector3 _velocity;
    public bool _isGrounded;
    public bool IsTooFar = false;
    public float TribeDistance = 0.1f;
    GameObject Tribe;
    Player _player;
    public bool InGame = true;

    public UnityEvent onPlayerRunStaminaNullEnter = new UnityEvent();
    public UnityEvent onPlayerRunStaminaNullExit = new UnityEvent();
    public UnityEvent onPlayerTooFarFromTribe = new UnityEvent();

	// Event For Tribe
	public PlayerHasMovedEvent onPlayerHasMoved = new PlayerHasMovedEvent();
	float _MinDistForTribeAcceleration;

    // Start is called before the first frame update
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

    void SwitchRun()
    {
        if (!IsRunning && !IsWaiting && PlayerMayRun)
            IsRunning = true;
        else
            IsRunning = false;
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

    public void Move(InputAxisUnityEventArg axis)
    {
        if (!InGame)
            return;

        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float x = axis.XValue;
        float z = axis.YValue;

        Vector3 right = transform.right * x;
        Vector3 forward = transform.forward * z;
        RaycastHit hit;
        _move = Vector3.zero;
        if (Physics.Raycast(CameraManager.I._MainCamera.transform.position + right * 0.5f, Vector3.down, out hit, 5, _groundMask))
        {
            float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (groundAngle <= _controller.slopeLimit - 0.1f)
                _move += right;
            //_isMoveAllowed = groundAngle <= _controller.slopeLimit;
        }
        else
            _move += right;

        if (Physics.Raycast(CameraManager.I._MainCamera.transform.position + forward * 0.5f, Vector3.down, out hit, 5, _groundMask))
        {
            float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (groundAngle <= _controller.slopeLimit)
                _move += forward;
            //_isMoveAllowed = groundAngle <= _controller.slopeLimit;
        }
        else
            _move += forward;

        if (_move == Vector3.zero)
        {
            SoundManager.I.StopPlayerSound();
        }
        else
        {
            //SoundManager.I.PlayPlayer("Walk");
            SoundManager.I.PlayerWalk();
        }
        //_move = transform.right * x + transform.forward * z;

        float runMultiply = 1;
        if (IsRunning && AmbiantManager.I.IsUsableNow(GameManager.I._data.PlayerRunUsable))
            runMultiply = GameManager.I._data.SpeedMultiplicator;

        if(_isMoveAllowed)
        {
        _controller.Move(_move * _speed * runMultiply * Time.deltaTime);
        }

        // if (Input.GetButtonDown("Jump") && _isGrounded)
        // {
        //    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        // }

        _velocity.y += _gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        IsWaiting = false;
    }

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
