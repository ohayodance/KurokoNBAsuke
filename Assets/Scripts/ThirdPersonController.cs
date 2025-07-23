
using Assets.Scripts;

using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/


/// <summary>
/// Main script for third-person movement of the character in the game.
/// Make sure that the object that will receive this script (the player) 
/// has the Player tag and the Character Controller component.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    private float jumpElapsedTime = 0;
    private bool isJumping = false;

    // Inputs
    private float inputHorizontal;
    private float inputVertical;
    private bool inputJump;

    private CharacterController cc;
    private Animator animator;
    private BallController ballController;
    private Dunks dunksController;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ballController = GetComponent<BallController>();
        dunksController = GetComponent<Dunks>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimations();
        HandleBallInteraction();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleInput()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
    }

    private void HandleAnimations()
    {
        animator.SetBool("Run", inputHorizontal != 0 || inputVertical != 0);
        animator.SetBool("Air", !cc.isGrounded);
        animator.SetBool("HaveBall", ballController.IsBallInHands);
        animator.SetBool("AimingShoot", ballController.IsShooting);

        if (ballController.TriggerShoot)
        {
            animator.SetTrigger("ShootOver");
            ballController.ResetShootTrigger();
        }
    }

    private void HandleBallInteraction()
    {
        ballController.HandleBallInput(
            Input.GetKey(KeyCode.Mouse0),
            Input.GetKeyUp(KeyCode.Mouse0)
        );

        //if (ballController.IsBallInHands)
        //{
        //    dunksController.HandleDunks(
        //        inputJump,
        //        inputVertical,
        //        ballController.TargetPosition
        //    );
        //}
        //
    }

    private void HandleMovement()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * inputVertical + right * inputHorizontal) * velocity * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * (isJumping ?
            Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime :
            -gravity * Time.deltaTime);

        if (moveDirection.magnitude > 0)
        {
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angle, 0), 0.15f);
        }

        cc.Move(verticalMovement + moveDirection);
    }

    private void HandleJump()
    {
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
            jumpElapsedTime = 0;
        }

        if (isJumping)
        {
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !ballController.IsBallInHands && !ballController.IsBallFlying)
        {
            // Проверяем, что мяч не движется слишком быстро
            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            if (ballRb != null && ballRb.velocity.magnitude < 2f)
            {
                ballController.PickUpBall(other.transform);
            }
        }
    }

    public void NotifyBallInRange(Transform ball)
    {
        if (!ballController.IsBallInHands && !ballController.IsBallFlying)
        {
            ballController.PickUpBall(ball);
        }
    }
}