using UnityEngine;
using DesignPattern;
public class MonsterSpell : PooledObject
{
    private BoxCollider2D m_boxCollider;

    private void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        m_boxCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(2);
            m_boxCollider.enabled = false;            
        }
        ReturnPool();
    }
}
