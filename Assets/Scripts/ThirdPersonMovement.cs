using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    float _horizontalInput;
    float _verticalInput;
    Vector3 _moveDirection;
    float _movement;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Others")]
    public Transform orientation;
    public Animator animator;

    bool _isWalking;
    bool _isRunning;

    Rigidbody _rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        VerifyPlayerIsWalkingOrRunning();
        SetAnimations();

        SpeedControl();
        VerifyGroundCollision();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void PlayerInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    void Movement()
    {
        _movement = _moveDirection.x + _moveDirection.z;
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    void SetAnimations()
    {
        animator.SetBool("isWalking", _isWalking);
        animator.SetBool("isRunning", _isRunning);
    }


    void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed) { 
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
        }

        if (_isRunning) {
            moveSpeed = 9f;
        } else {
            moveSpeed = 4f;
        }
    }

    void VerifyGroundCollision()
    {
        if (grounded) { 
            _rigidbody.linearDamping = groundDrag; 
        } else { 
            _rigidbody.linearDamping = 0; 
        }
    }

    void VerifyPlayerIsWalkingOrRunning()
    {
        if (_movement > 0 || _movement < 0) {
            if (Input.GetKey(KeyCode.LeftShift)) { 
                _isRunning = true;
                _isWalking = false;
            } else {
                _isRunning = false;
                _isWalking = true;
            }
        } else {
            _isWalking = false;
            _isRunning = false;
        }
    }
}