using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Character
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float inputX;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        // ���ⴴ
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // ��ԡ��õ�
        if (inputX != 0)
        {
            Vector3 sc = transform.localScale;
            sc.x = Mathf.Sign(inputX) * Mathf.Abs(sc.x == 0 ? 1 : sc.x);
            transform.localScale = sc;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
        return isGrounded;
    }

    // ������ҧ���ⴹ����� (���¡�ҡ Enemy)
    private void OnCollisionEnter2D(Collision2D other)
    {
        // ����ѵ�����ç ��ѡ��硹���
    }
}
