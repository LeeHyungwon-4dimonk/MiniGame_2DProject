using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    private Animator m_animator;

    [SerializeField] private float m_spellSpeed;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_animator.SetTrigger("Burst");
        StartCoroutine(DestroyTerm());
    }

    IEnumerator DestroyTerm()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}