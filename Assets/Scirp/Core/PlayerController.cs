using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : Character
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    [Header("Dash (Shift)")]
    [SerializeField] private float dashSpeed = 14f;       // ความเร็วตอนแดช
    [SerializeField] private float dashDuration = 0.18f;  // นานกี่วินาที
    [SerializeField] private float dashCooldown = 0.60f;  // คูลดาวน์
    [SerializeField] private bool airDash = false;        // อนุญาตแดชกลางอากาศไหม
    [SerializeField] private float iframeExtra = 0.02f;   // เผื่อเวลาเปิด/ปิด hitbox (เลือกได้)

    private Rigidbody2D rb;
    private float inputX;
    private bool isDashing;
    private float cdTimer;
    private float defaultGravity;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;
    }

    private void Update()
    {
        // รับอินพุตแกน X เสมอ (แต่จะไม่ถูกนำไปใช้ตอนกำลังแดช)
        inputX = Input.GetAxisRaw("Horizontal");

        // กระโดด (ไม่ให้กดระหว่างแดช)
        if (Input.GetButtonDown("Jump") && IsGrounded() && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Dash (Left Shift)
        cdTimer -= Time.unscaledDeltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && cdTimer <= 0f)
        {
            if (airDash || IsGrounded())
            {
                StartCoroutine(DashRoutine());
            }
        }

        // พลิกสไปรต์ตามทิศ (ถ้าไม่ได้แดช)
        if (!isDashing && inputX != 0)
        {
            var sc = transform.localScale;
            sc.x = Mathf.Sign(inputX) * Mathf.Abs(sc.x == 0 ? 1 : sc.x);
            transform.localScale = sc;
        }

        // อัปเดตอนิเมชัน (เราใช้ bool IsRunning แบบที่เลือก)
        if (animator) animator.SetBool(IsRunningHash, Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
        // ถ้ากำลังแดช เราคุม rb ในคอร์รอตีนแล้ว
    }

    public override void Move()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    // ===== Dash + i-frame =====
    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        cdTimer = dashCooldown;

        // ทิศแดช: ถ้าไม่กดซ้าย/ขวา ใช้ทิศที่กำลังหันหน้าอยู่
        float dir = inputX != 0 ? Mathf.Sign(inputX) : Mathf.Sign(Mathf.Approximately(transform.localScale.x, 0f) ? 1f : transform.localScale.x);
        if (dir == 0) dir = 1;

        // เปิดอมตะ (ยืดเวลานิดหน่อย กันจังหวะชน)
        StartCoroutine(InvincibleFor(dashDuration + iframeExtra));

        // ปิดแรงโน้มถ่วงชั่วคราว ให้แดชเป็นเส้นตรง
        float prevGrav = rb.gravityScale;
        rb.gravityScale = 0f;

        // ควบคุมความเร็วคงที่ระหว่างแดช
        float t = 0f;
        while (t < dashDuration)
        {
            t += Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
            yield return new WaitForFixedUpdate();
        }

        // กลับสภาพเดิม
        rb.gravityScale = prevGrav;
        isDashing = false;
    }
}
