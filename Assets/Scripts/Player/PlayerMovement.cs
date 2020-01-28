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
    public float _groundDistance = 0.4f;
    [Header("The Layer of the ground or element on which we can walk.")]
    public LayerMask _groundMask;

    [Header("Player is running")]
    public bool IsRunning = false;
    bool PlayerMayRun = true;
    public float PlayerRunGauge = 100f;

    Vector3 _velocity;
    bool _isGrounded;
    public bool IsTooFar = false;
    public float TribeDistance = 0.0f;
    GameObject Tribe;
    Player _player;
    bool AllowMove = true;

	public PlayerHasMovedEvent onPlayerHasMoved = new PlayerHasMovedEvent();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        Tribe = (GameObject)ObjectsManager.I["TribeGroundPosition"];
        _speed = GameManager.I._data.InitialPlayerSpeed;
        _player = GetComponent<Player>();
        PlayerRunGauge = GameManager.I._data.PlayerRunGaugeMax;
        _player.onPlayerEnergyNullEnter.AddListener(() => { if (GameManager.I._data.PlayerRunEnergyLowUnusable) PlayerMayRun = false; });
        _player.onPlayerEnergyNullExit.AddListener(() => { if (GameManager.I._data.PlayerRunEnergyLowUnusable) PlayerMayRun = true; });

        InputManager.I.onRunButtonPressed.AddListener(EnableRun);
        InputManager.I.onRunButtonReleased.AddListener(DisableRun);
        InputManager.I.onMoveInputAxisEvent.AddListener(Move);
        UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { AllowMove = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { AllowMove = false; });
    }

    void EnableRun()
    {
        if (PlayerMayRun)
            IsRunning = true;
    }
    void DisableRun()
    {
        IsRunning = false;
    }

    private void Update()
    {
        ComputeTribeDistance();
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

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        _velocity.y += _gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    void ComputeTribeDistance()
    {
        Vector3 TribePositionProjected = Vector3.ProjectOnPlane(Tribe.transform.position, Vector3.up);
        Vector3 PlayerPositionProjected = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        TribeDistance = Vector3.Distance(TribePositionProjected, PlayerPositionProjected);
        IsTooFar = TribeDistance > GameManager.I._data.MaximumDistanceOfTribe;

		if (TribeDistance > 5)
			onPlayerHasMoved.Invoke(transform.position);
    }

    void ResetSpeed()
    {
        _speed = GameManager.I._data.InitialPlayerSpeed;
    }
}

public class PlayerHasMovedEvent : UnityEvent<Vector3> { }
