using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove { get; private set; } = true;

    [Header("Movement Parameters")]
    [SerializeField]
    private float moveSpeed = 6;
    [SerializeField]
    private float movementMultiplier = 10;
    [SerializeField]
    private float airMultiplier = 0.4f;

    [Header("Sprinting Parameters")]
    [SerializeField]
    private float walkSpeed = 4;
    [SerializeField]
    private float sprintSpeed = 6;
    [SerializeField]
    private float acceleration = 10;


    [Header("Drag")]
    [SerializeField]
    private float groundDrag = 6;

    [SerializeField]
    private float airDrag = 2;

    private float horizontalMovement;
    private float verticalMovement;

    [SerializeField] 
    private Vector3 movementDirection;

    [SerializeField]
    private Vector3 slopeMoveDirection;

    private Rigidbody rigidbody;

    [Header("Ground Detection")]
    private bool isGrounded;
    private float groundDistance = 0.5f;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField] 
    private Transform groundCheck;

    private float playerHeight = 2;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [SerializeField]
    private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Jumping"), SerializeField]
    private float jumpForce = 5;

    [SerializeField] private Transform orientation;

    private RaycastHit slopeHit;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    private void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        movementDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = airDrag;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rigidbody.AddForce(movementDirection.normalized * moveSpeed *
            movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rigidbody.AddForce(slopeMoveDirection.normalized * moveSpeed *
            movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rigidbody.AddForce(movementDirection.normalized * moveSpeed *
            movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }
    private void Jump()
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0,
        rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit,
        playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

}
