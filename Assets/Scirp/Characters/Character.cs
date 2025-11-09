using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int _health;
    public int MaxHealth => maxHealth;
    public int Health => _health;

    // NEW: สถานะอมตะ
    protected bool invincible;                 // true = ไม่โดนดาเมจ
    public bool IsInvincible => invincible;    // เผื่อ UI/ดีบัก

    protected virtual void Awake()
    {
        _health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        if (invincible) return; // NEW: ถ้าอมตะ ไม่รับดาเมจ
        _health = Mathf.Max(0, _health - amount);
        if (_health <= 0) OnDeath();
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public abstract void Move();

    // NEW: ยูทิลิตี้เปิด i-frame ชั่วคราว
    protected System.Collections.IEnumerator InvincibleFor(float seconds)
    {
        invincible = true;
        yield return new WaitForSeconds(seconds);
        invincible = false;
    }
}
