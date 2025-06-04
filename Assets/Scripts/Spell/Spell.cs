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
        // �浹 �� ���翡 ���Ͱ� ���� �з����� �ʵ��� ó��
        m_rigid.velocity = Vector2.zero;
        m_animator.SetTrigger("Burst");

        // ���� �ð��� ���� ���� ������ ���� �ݿ�
        // 1.5 �� ������ ������ �������� 1 �þ��, �ִ� ������ 5 / �ִ� ��¡ �ð� 6�ʷ� ������
        // �뷱���� �ʿ��� ��� �� �κ��� ������ ��
        // �浹 ���׷� ���� �������� ��ø�Ͽ� ���� �ʵ��� ó���ϱ� ���� ���� �浹�� ���ÿ� �浹ü�� �� ����
        if (collision.gameObject.CompareTag("Enemy"))
        {
            int damage = 1 + (int)(GameObject.FindWithTag("Player").GetComponent<PlayerController>().ChargeTime / 1.5f);
            if (damage > 5) { damage = 5; }
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            m_boxCollider.enabled = false;
        }

        // �ִϸ��̼� ����� ���� ������ �ݿ�
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

    // ���� �ִϸ��̼� ����� ���� ������
    IEnumerator DestroyTerm()
    {
        yield return new WaitForSeconds(0.5f);
        ReturnPool();
    }
}