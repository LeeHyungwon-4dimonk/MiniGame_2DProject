using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;

public class Spell : PooledObject
{
    private Animator m_animator;
    private Rigidbody2D m_rigid;

    [SerializeField] private float m_spellSpeed;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_rigid.velocity = Vector2.zero;
        m_animator.SetTrigger("Burst");
        StartCoroutine(DestroyTerm());
    }

    IEnumerator DestroyTerm()
    {
        yield return new WaitForSeconds(1.5f);
        ReturnPool();
    }
}