using UnityEngine;

public class GroundPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5;

    [HeaderAttribute("Jump")]
    public float JumpForse = 10;
    public Transform GroundCheck;
    public float GroundCheckRadius = 0.10f;

    public Rigidbody2D Rigidbody;
    public Animator Animator;

    public bool isGrounded {  get; private set; }

    private void Awake()
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
}
