using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : BaseMonoBehaviour
{
    public CharacterController _controller;
    public float _speed = 12f;
    public float _gravity = -9.81f;
    public float _jumpHeight = 3f;
    Vector3 _velocity;

    public Transform _groundCheck;
    public float _groundDistance = 0.4f;
    public LayerMask _groundMask;

    bool _isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
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

        //Debug.DrawLine(weapon.position, Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)), Color.black);
    }
}
