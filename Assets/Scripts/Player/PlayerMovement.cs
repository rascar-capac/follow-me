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
    public float PlayerRunGauge = 100f;
    bool IsRunGaugeNull => PlayerRunGauge == 0;
    bool PlayerMayRun = true;

    Vector3 _velocity;
    public bool _isGrounded;
    public bool IsTooFar = false;
    public float TribeDistance = 0.1f;
    GameObject Tribe;
    Player _player;
    bool AllowMove = true;

    public UnityEvent onPlayerRunGaugeNullEnter = new UnityEvent();
    public UnityEvent onPlayerRunGaugeNullExit = new UnityEvent();

	// Event For Tribe
	public PlayerHasMovedEvent onPlayerHasMoved = new PlayerHasMovedEvent();
	float _MinDistForTribeAcceleration;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        Tribe = (GameObject)ObjectsManager.I["TribeGroundPosition"];
		_MinDistForTribeAcceleration = GameManager.I._data.TribeProperties.MinDistForAcceleration;
        _speed = GameManager.I._data.InitialPlayerSpeed;
        _player = GetComponent<Player>();
        PlayerRunGauge = GameManager.I._data.PlayerRunGaugeMax;
        onPlayerRunGaugeNullEnter.AddListener(() => { PlayerMayRun = false; });
        onPlayerRunGaugeNullExit.AddListener(() => {  PlayerMayRun = true; });

        InputManager.I.onRunButtonPressed.AddListener(SwitchRun);
        InputManager.I.onMoveInputAxisEvent.AddListener(Move);
        InputManager.I.onStopMoveInputAxisEvent.AddListener(Wait);
        InputManager.I.onPlayerJumpPressed.AddListener(Jump);
        UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { AllowMove = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { AllowMove = false; });


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
        ComputeTribeDistance();
        UpdateRunGauge();
        RunGaugeCritical();
    }

    public void Move(InputAxisUnityEventArg axis)
    {
        if (!AllowMove)
            return;

        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float x = axis.XValue;
        float z = axis.YValue;

        Vector3 move = transform.right * x + transform.forward * z;

        float runMultiply = 1;
        if (IsRunning && AmbiantManager.I.IsUsableNow(GameManager.I._data.PlayerRunUsable))
            runMultiply = GameManager.I._data.SpeedMultiplicator;

        _controller.Move(move * _speed * runMultiply * Time.deltaTime);

        //if (Input.GetButtonDown("Jump") && _isGrounded)
        //{
        //    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        //}

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
        TribeDistance = Vector3.Distance(TribePositionProjected, PlayerPositionProjected);
        IsTooFar = TribeDistance > GameManager.I._data.MaximumDistanceOfTribe;

		if (TribeDistance > _MinDistForTribeAcceleration)
			onPlayerHasMoved.Invoke(transform.position);
    }

    void ResetSpeed()
    {
        _speed = GameManager.I._data.InitialPlayerSpeed;
    }

    public void UpdateRunGauge()
    {
        if(GameManager.I._data.PlayerRunGauge)
        {
            if (IsRunning)
                PlayerRunGauge -= GameManager.I._data.PlayerRunCostBySecond * Time.deltaTime;
            else
                PlayerRunGauge += GameManager.I._data.PlayerRunGainBySecond * Time.deltaTime;

            PlayerRunGauge = Mathf.Clamp(PlayerRunGauge, 0, GameManager.I._data.PlayerRunGaugeMax);
        }
    }

    void RunGaugeCritical()
	{
		if(IsRunGaugeNull)
		{
			onPlayerRunGaugeNullEnter.Invoke();
            IsRunning = false;
		}
		else
		{
            onPlayerRunGaugeNullExit.Invoke();
		}
	}
}

public class PlayerHasMovedEvent : UnityEvent<Vector3> { }
