using System.Collections;
using DesignPattern;
using UnityEngine;

public class Spell : PooledObject
{    
    private Animator m_animator;
    private Rigidbody2D m_rigid;
    private BoxCollider2D m_boxCollider;

    private Coroutine m_coroutine;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        m_boxCollider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 시 스펠에 몬스터가 많이 밀려나지 않도록 처리
        m_rigid.velocity = Vector2.zero;
        m_animator.SetTrigger("Burst");

        // 차지 시간에 따른 스펠 데미지 증가 반영
        // 1.5 초 차지할 때마다 데미지가 1 늘어나며, 최대 데미지 5 / 최대 차징 시간 6초로 지정함
        // 밸런싱이 필요할 경우 이 부분을 수정할 것
        // 충돌 버그로 인해 데미지가 중첩하여 들어가지 않도록 처리하기 위해 몬스터 충돌과 동시에 충돌체를 꺼 버림
        if (collision.gameObject.CompareTag("Enemy"))
        {
            int damage = 1 + (int)(GameObject.FindWithTag("Player").GetComponent<PlayerController>().ChargeTime / 1.5f);
            if (damage > 5) { damage = 5; }
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            m_boxCollider.enabled = false;
        }

        // 애니메이션 출력을 위한 딜레이 반영
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

    // 폭발 애니메이션 출력을 위한 딜레이
    IEnumerator DestroyTerm()
    {
        yield return new WaitForSeconds(0.5f);
        ReturnPool();
    }
}