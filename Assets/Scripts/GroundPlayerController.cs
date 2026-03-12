using UnityEngine;

public class GroundPlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D Rigidbody;
    public Animator Animator;

    [Header("Movement")]
    public float MoveSpeed = 5f;

    [Header("Jump")]
    public float JumpForce = 10f;
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.10f;
    public LayerMask GroundLayerMask;

    [Header("Ladder")]
    public LayerMask LadderLayerMask;
    public Transform LadderCheck;
    public float LadderCheckRadius = 0.15f;
    public float ClimbSpeed = 4f;

    [Header("Wall")]
    public LayerMask WallLayerMask;
    public Transform WallCheck;
    public float WallCheckRadius = 0.12f;

    [Header("Wall Slide / Wall Jump (unlock later)")]
    public float WallSlideSpeed = 2f;
    public float WallJumpForceX = 8f;
    public float WallJumpForceY = 10f;
    public float WallJumpLockTime = 0.15f; 

    [Header("Unlockable Abilities")]
    [Tooltip("�� ���� ������������ ����� �������� (���������, ���� ������ ��������).")]
    public bool DoubleJumpUnlocked = false;

    [Tooltip("�� ���� ������������ ����� �������� (���������, ���� ��������).")]
    public bool WallJumpUnlocked = false;

    public bool IsGrounded { get; private set; }

    private float horizontalInput;
    private float verticalInput;

    private bool isClimbing;
    private bool isTouchingLadder;
    private bool isTouchingWall;

    private int jumpsUsed; 

    private float wallJumpLockTimer; 
    private int wallSide; 

    private void Awake()
    {
        if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody2D>();
        if (Animator == null) Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadInput();
        CheckEnvironment();
        HandleClimbingState();
        HandleJumpInput();
        UpdateAnimatorParameters();
        HandleFacingDirection();
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleClimbingMovement();
        HandleWallSlide();
        HandleWallJumpLockTimer();
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void CheckEnvironment()
    {
        if (GroundCheck != null)
        {
            IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayerMask);
        }
        else
        {
            IsGrounded = false;
        }

        if (IsGrounded && !isClimbing)
        {
            jumpsUsed = 0;
        }
        if (LadderCheck != null)
        {
            isTouchingLadder = Physics2D.OverlapCircle(LadderCheck.position, LadderCheckRadius, LadderLayerMask);
        }
        else
        {
            isTouchingLadder = false;
        }


        isTouchingWall = false;
        wallSide = 0;

        if (WallCheck != null)
        {
            Collider2D wallCollider = Physics2D.OverlapCircle(WallCheck.position, WallCheckRadius, WallLayerMask);
            if (wallCollider != null)
            {
                isTouchingWall = true;

                float directionToWall = wallCollider.transform.position.x - transform.position.x;
                wallSide = (directionToWall >= 0f) ? 1 : -1;
            }
        }
    }

    private void HandleClimbingState()
    {
        if (isTouchingLadder && Mathf.Abs(verticalInput) > 0.01f)
        {
            if (!isClimbing)
            {
                StartClimbing();
            }
        }

        if (isClimbing && !isTouchingLadder)
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        Rigidbody.gravityScale = 0f;

        Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0f);
        jumpsUsed = 0;
    }

    private void StopClimbing()
    {
        isClimbing = false;

        Rigidbody.gravityScale = 3f;
    }

    private void HandleClimbingMovement()
    {
        if (!isClimbing) return;
        float climbVelocityY = verticalInput * ClimbSpeed;
        float moveVelocityX = horizontalInput * (MoveSpeed * 0.6f);

        Rigidbody.linearVelocity = new Vector2(moveVelocityX, climbVelocityY);
    }

    private void HandleHorizontalMovement()
    {
        if (isClimbing) return;
        if (wallJumpLockTimer > 0f) return;

        Rigidbody.linearVelocity = new Vector2(horizontalInput * MoveSpeed, Rigidbody.linearVelocity.y);
    }

    private void HandleFacingDirection()
    {
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-2.2f, 2.2f, 2.2f);
        }
    }

    private void HandleJumpInput()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (isClimbing)
        {
            StopClimbing();
            PerformNormalJump();
            return;
        }

        if (WallJumpUnlocked && CanWallJumpNow())
        {
            PerformWallJump();
            return;
        }

        if (IsGrounded)
        {
            PerformNormalJump();
            return;
        }

        if (DoubleJumpUnlocked && jumpsUsed < 2)
        {
            PerformDoubleJump();
            return;
        }
    }

    private void PerformNormalJump()
    {
        jumpsUsed = 1;
        Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0f);
        Rigidbody.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);

        if (Animator != null) Animator.SetTrigger("Jump");
    }

    private void PerformDoubleJump()
    {
        jumpsUsed = 2;

        Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0f);
        Rigidbody.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);

        if (Animator != null) Animator.SetTrigger("Jump");
    }

    private bool CanWallJumpNow()
    {
        if (!isTouchingWall) return false;
        if (IsGrounded) return false;
        if (wallJumpLockTimer > 0f) return false;

        return Rigidbody.linearVelocity.y <= 0.5f;
    }

    private void PerformWallJump()
    {
        jumpsUsed = 1;

        float jumpDirectionX = -wallSide;

        Rigidbody.linearVelocity = Vector2.zero;
        Rigidbody.AddForce(new Vector2(jumpDirectionX * WallJumpForceX, WallJumpForceY), ForceMode2D.Impulse);

        wallJumpLockTimer = WallJumpLockTime;

        if (Animator != null) Animator.SetTrigger("Jump");
    }

    private void HandleWallJumpLockTimer()
    {
        if (wallJumpLockTimer > 0f)
        {
            wallJumpLockTimer -= Time.fixedDeltaTime;
            if (wallJumpLockTimer < 0f) wallJumpLockTimer = 0f;
        }
    }

    private void HandleWallSlide()
    {
        if (!WallJumpUnlocked) return;
        if (!isTouchingWall) return;
        if (IsGrounded) return;
        if (isClimbing) return;

        if (Rigidbody.linearVelocity.y < -0.01f)
        {
            float limitedFallSpeed = -WallSlideSpeed;
            Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, Mathf.Max(Rigidbody.linearVelocity.y, limitedFallSpeed));
        }
    }

    private void UpdateAnimatorParameters()
    {
        if (Animator == null) return;

        // ������� ���������
        Animator.SetFloat("Speed", Mathf.Abs(Rigidbody.linearVelocity.x));
        Animator.SetBool("IsGrounded", IsGrounded);
        Animator.SetFloat("VerticalSpeed", Rigidbody.linearVelocity.y);

        // �������
        Animator.SetBool("IsClimbing", isClimbing);
        Animator.SetFloat("ClimbSpeed", Mathf.Abs(Rigidbody.linearVelocity.y));

        // ����
        bool isWallSlidingNow = WallJumpUnlocked && isTouchingWall && !IsGrounded && !isClimbing && Rigidbody.linearVelocity.y < -0.01f;
        Animator.SetBool("IsWallSliding", isWallSlidingNow);
    }

    public void UnlockDoubleJump()
    {
        DoubleJumpUnlocked = true;
    }

    public void UnlockWallJump()
    {
        WallJumpUnlocked = true;
    }

    public void LockDoubleJump()
    {
        DoubleJumpUnlocked = false;
    }

    public void LockWallJump()
    {
        WallJumpUnlocked = false;
    }


}