using UnityEngine;

public class GroundPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5;

    [HeaderAttribute("Jump")]
    public float JumpForse = 10;
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.10f;
    public LayerMask GroundLayerMask;

    [Header("Wall")]
    public LayerMask WallLayerMask;
    public Transform WallCheck;
    public float WallCheckRadius;

    [Header("Wall Jump")]
    public float WallSlideSpeed = 2f;

    public Rigidbody2D Rigidbody;
    public Animator Animator;

    [Header("Unlockable Abilities")]
    public bool DoubleJumpUnlocked = false;
    public bool WallJumpUnlocked = false;
    public bool isGrounded {  get; private set; }

    private float horizontalInput;
    private float verticalInput;

    private bool isTouchingWall;
    private int wallSide; // ‗ךשמ -1 עמ סע³םא חכ³גא

    private int jumpUsed;


     void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void CheckEnviroment()
    {
        if (GroundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayerMask);
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            jumpUsed = 0;
        }

        isTouchingWall = false;
        wallSide = 0;

        if (WallCheck != null) 
        {
            Collider2D wallColider = Physics2D.OverlapCircle(WallCheck.position, WallCheckRadius, WallLayerMask);
            if (wallColider != null) 
            {
                isTouchingWall = true;

                float directionWall = wallColider.transform.position.x - transform.position.x;
                wallSide = (directionWall >= 0f) ? 1 : -1;
            }
        }

    }

    private void HandleHorizontalMovement()
    {
        Rigidbody.linearVelocity = new Vector2(horizontalInput * MoveSpeed, Rigidbody.linearVelocity.y);



    }
    private void HandleFacingDirection()
    {
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        }else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-2.2f, 2.2f, 2.2f);
        }
    }

    private void HandleJumpInput()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if(DoubleJumpUnlocked && jumpUsed < 2)
        {
            PerformDoubleJump();
        }
    }

    private void PerformNormalJump()
    {

    }
    private void PerformDoubleJump()
    {
        jumpUsed = 1;

        Rigidbody.linearVelocity = new Vector2(Rigidbody.linearVelocity.x, 0f);
    }
}

