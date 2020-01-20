using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    Vector3 _velocity;
    bool _isGrounded;
    public bool IsTooFar = false;
    public float TribeDistance = 0.0f;
    GameObject Tribe;
    Player _player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        Tribe = (GameObject)ObjectsManager.I["TribeGroundPosition"];
        _speed = GameManager.I._data.InitialPlayerSpeed;
        _player = GetComponent<Player>();
        _player.onPlayerLifeEnterCritical.AddListener(DecreaseSpeed);
        _player.onPlayerLifeExitCritical.AddListener(IncreaseSpeed);
    }

    private void Update()
    {
        Move();
        ComputeTribeDistance();
    }

    public void Move()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        _controller.Move(move * _speed * Time.deltaTime);

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
    }

    void DecreaseSpeed()
    {
        _speed -= _speed * GameManager.I._data.PlayerSpeedDecreasePercentage / 100;
    }

    void IncreaseSpeed()
    {
        _speed = GameManager.I._data.InitialPlayerSpeed;
    }
}
