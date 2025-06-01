using System.Collections;
using DesignPattern;
using UnityEngine;

public class Spell : PooledObject
{
    private Animator m_animator;
    private Rigidbody2D m_rigid;

    private Coroutine m_coroutine;

    [SerializeField] private float m_spellSpeed;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_rigid.velocity = Vector2.zero;
        m_animator.SetTrigger("Burst");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(1);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }
        if (m_coroutine == null)
        {
            m_coroutine = StartCoroutine(DestroyTerm());
        }
    }

    IEnumerator DestroyTerm()
    {
        yield return new WaitForSeconds(0.5f);
        ReturnPool();
    }
}