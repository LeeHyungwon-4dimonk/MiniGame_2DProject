using DesignPattern;
using UnityEngine;

/// <summary>
/// �÷��̾� ���� �߻� Muzzle
/// * ���� �ӵ� ������ Spell Speed �ν����ͷ� ����
/// </summary>
public class PlayerMuzzle : MonoBehaviour
{
    // ���� ������
    [SerializeField] Spell m_spellPrefab;

    // ���� �߻� �ӵ�(�뷱��)
    [SerializeField] float m_spellSpeed;
    
    // �÷��̾� ������Ʈ
    private PlayerController m_playerController;
    private SpriteRenderer m_playerSpriteRenderer;

    // ���� ������ ������Ʈ
    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    // ���� Ǯ ����
    private ObjectPool m_spellPool;

    private void Awake()
    {
        m_playerController = GetComponentInParent<PlayerController>();
        m_playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        m_spellPool = new ObjectPool(transform, m_spellPrefab);
    }

    public void Fire()
    {
        // �÷��̾ ��¡ �����̸�
        if (m_playerController.StateMach.CurState == m_playerController.StateMach.StateDic[EState.RangedAttack])
        {
            PooledObject spell = m_spellPool.PopPool() as Spell;
            m_spellRigid = spell.GetComponent<Rigidbody2D>();
            m_spellSpriteRenderer = spell.GetComponent<SpriteRenderer>();

            // �÷��̾ ���� ���� ����
            // ���� ���⿡ ���� Muzzle�� ��ġ�� �̵���Ű�� �߻� ���� ���� �� ���� �ִϸ��̼� ��� ���� ����
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