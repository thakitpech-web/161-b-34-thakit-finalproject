using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int _health; // encapsulated (ไม่ serialize public)
    public int MaxHealth => maxHealth;
    public int Health => _health;

    protected virtual void Awake()
    {
        _health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        _health = Mathf.Max(0, _health - amount);
        if (_health <= 0) OnDeath();
    }

    protected virtual void OnDeath()
    {
        // ให้ลูกคลาส override ถ้าต้องการ
        Destroy(gameObject);
    }

    // ทำให้ลูกคลาสกำหนดพฤติกรรม Move เอง
    public abstract void Move();
}
