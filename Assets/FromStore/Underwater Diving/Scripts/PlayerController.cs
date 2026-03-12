using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Rush")]
    public bool rushing = false;
    public float rushSpeedBonus = 2f;
    public float rushDuration = 2f;

    [Header("Components")]
    public Rigidbody2D myRigidBody;
    public Animator myAnim;

    [Header("Effects")]
    public GameObject bubbles;

    [Header("Visual")]
    public float spriteScale = 1.4f;

    private float horizontalInput;
    private float verticalInput;
    private float rushTimeLeft = 0f;

    private void Start()
    {
        if (myRigidBody == null)
        {
            myRigidBody = GetComponent<Rigidbody2D>();
        }

        if (myAnim == null)
        {
            myAnim = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        ReadInput();
        HandleRushInput();
        ResetBoostTime();
        UpdateFacingDirection();
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void HandleRushInput()
    {
        if (Input.GetButtonDown("Jump") && !rushing)
        {
            rushing = true;
            rushTimeLeft = rushDuration;

            // if (bubbles != null)
            // {
            //     Instantiate(bubbles, transform.position, transform.rotation);
            // }
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = moveSpeed;

        if (rushing)
        {
            currentSpeed += rushSpeedBonus;
        }

        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 targetVelocity = moveDirection * currentSpeed;

        myRigidBody.linearVelocity = targetVelocity;
    }

    private void ResetBoostTime()
    {
        if (!rushing)
        {
            return;
        }

        rushTimeLeft -= Time.deltaTime;

        if (rushTimeLeft <= 0f)
        {
            rushing = false;
            rushTimeLeft = 0f;
        }
    }

    private void UpdateFacingDirection()
    {
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-spriteScale, spriteScale, spriteScale);
        }
    }

    private void UpdateAnimatorParameters()
    {
        if (myAnim == null || myRigidBody == null)
        {
            return;
        }

        myAnim.SetFloat("Speed", myRigidBody.linearVelocity.magnitude);
        myAnim.SetFloat("HorizontalSpeed", Mathf.Abs(myRigidBody.linearVelocity.x));
        myAnim.SetFloat("VerticalSpeed", myRigidBody.linearVelocity.y);
        myAnim.SetBool("IsRushing", rushing);
    }

    public void hurt()
    {
        if (myAnim == null)
        {
            return;
        }

        if (!rushing)
        {
            myAnim.Play("PlayerHurt");
        }
    }
}