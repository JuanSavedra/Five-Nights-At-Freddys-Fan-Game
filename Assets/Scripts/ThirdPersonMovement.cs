using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float crouchSpeed;
    public float crouchYScale;
    public CapsuleCollider playerCollider;

    public float groundDrag;

    float _horizontalInput;
    float _verticalInput;
    float _movement;
    Vector3 _moveDirection;

    [Header("Keybinds")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Others")]
    public Transform orientation;
    public Animator animator;
    
    Rigidbody _rigidbody;

    public MovementState movementState;

    public enum MovementState 
    {
        Walking,
        Sprinting,
        Falling,
        Crouching
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;

        playerCollider.height = playerHeight;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SetAnimations();

        SpeedControl();
        StateHandler();
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

        if (Input.GetKeyDown(crouchKey)) {
            playerCollider.height = 0.6f;
            playerCollider.center = new Vector3(playerCollider.center.x, 0.3f, playerCollider.center.z);
        }

        if (Input.GetKeyUp(crouchKey)) {
            playerCollider.height = playerHeight;
            playerCollider.center = new Vector3(playerCollider.center.x, 0.5808896f, playerCollider.center.z);
        }
    }

    void StateHandler()
    {
        if (Input.GetKeyDown(crouchKey)) {
            movementState = MovementState.Crouching;
            moveSpeed = crouchSpeed;
        }

        if (_movement != 0 && Input.GetKey(sprintKey) && grounded) {
            movementState = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
        } else if (_movement != 0 && grounded) {
            movementState = MovementState.Walking;
            moveSpeed = walkSpeed;
        } else if (!grounded) {
            movementState = MovementState.Falling;
            moveSpeed = 0.5f;
        }
    }

    void Movement()
    {
        _movement = _moveDirection.x + _moveDirection.z;
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        _rigidbody.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    void SetAnimations()
    {
        
    }


    void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed) { 
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
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
}