using System.Collections;
using DesignPattern;
using UnityEngine;

public class PlayerMuzzle : MonoBehaviour
{
    // 플레이어
    private PlayerController m_playerController;
    private SpriteRenderer m_playerSpriteRenderer;

    // 프리팹
    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    private ObjectPool m_spellPool;

    [SerializeField] Spell m_spellPrefab;
    [SerializeField] float m_spellSpeed;

    private void Awake()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        m_playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        m_spellPool = new ObjectPool(transform, m_spellPrefab);
    }

    public void Fire()
    {
        if (m_playerController.StateMach.CurState == m_playerController.StateMach.StateDic[EState.RangedAttack])
        {
            PooledObject spell = m_spellPool.PopPool() as Spell;            
            //m_spellPrefab = Instantiate(m_spellPrefab);
            m_spellRigid = spell.GetComponent<Rigidbody2D>();
            m_spellSpriteRenderer = spell.GetComponent<SpriteRenderer>();
            if (m_playerSpriteRenderer.flipX == false)
            {
                gameObject.transform.localPosition = new Vector3(0.8f, 0.7f, 0);
                spell.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.right * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = false;
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(-0.8f, 0.7f, 0);
                spell.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.left * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = true;
            }            
        }
    }
}