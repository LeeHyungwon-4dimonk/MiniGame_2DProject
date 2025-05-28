using System.Collections;
using UnityEngine;

public class PlayerMuzzle : MonoBehaviour
{
    private PlayerController m_playerController;
    private SpriteRenderer m_playerSpriteRenderer;

    private Coroutine m_spellCoolTimeCoroutine;

    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    [SerializeField] GameObject m_spellPrefab;
    [SerializeField] float m_spellSpeed;

    private void Awake()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        m_playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    public void Fire()
    {
        if (m_playerController.StateMach.CurState == m_playerController.StateMach.StateDic[EState.RangedAttack])
        {
            m_spellPrefab = Instantiate(m_spellPrefab);
            m_spellRigid = m_spellPrefab.GetComponent<Rigidbody2D>();
            m_spellSpriteRenderer = m_spellPrefab.GetComponent<SpriteRenderer>();
            if (m_playerSpriteRenderer.flipX == false)
            {
                gameObject.transform.localPosition = new Vector3(0.8f, 0.7f, 0);
                m_spellPrefab.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.right * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = false;
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(-0.8f, 0.7f, 0);
                m_spellPrefab.transform.position = gameObject.transform.position;
                m_spellRigid.AddForce(Vector2.left * m_spellSpeed, ForceMode2D.Impulse);
                m_spellSpriteRenderer.flipX = true;
            }            
        }
    }
}