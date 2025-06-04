using DesignPattern;
using UnityEngine;

public class BossMonsterMuzzle : MonoBehaviour
{
    // ���� ������
    [SerializeField] private MonsterSpell m_spellPrefab;

    // ���� ���� ��ũ���ͺ� ������Ʈ
    [SerializeField] private BossMonsterData m_bossMobData;

    // ���� ���� ������Ʈ
    private SpriteRenderer m_bossSpriteRenderer;

    // ���� ������ ������Ʈ
    private Rigidbody2D m_spellRigid;
    private SpriteRenderer m_spellSpriteRenderer;

    // ���� Ǯ ����
    private ObjectPool m_spellPool;

    private void Awake()
    {
        m_bossSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        m_spellPool = new ObjectPool(transform, m_spellPrefab);
    }

    public void Fire()
    {
        PooledObject spell = m_spellPool.PopPool() as MonsterSpell;
        m_spellRigid = spell.GetComponent<Rigidbody2D>();
        m_spellSpriteRenderer = spell.GetComponent<SpriteRenderer>();

        // ���Ͱ� ���� ���� ����
        // ���� ���⿡ ���� Muzzle�� ��ġ�� �̵���Ű�� �߻� ���� ���� �� ���� �ִϸ��̼� ��� ���� ����
        if (m_bossSpriteRenderer.flipX == true)
        {
            gameObject.transform.localPosition = new Vector3(4f, 1.2f, 0);
            spell.transform.position = gameObject.transform.position;
            m_spellRigid.AddForce(Vector2.right * m_bossMobData.SpellSpeed, ForceMode2D.Impulse);
            m_spellSpriteRenderer.flipX = false;
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(-4f, 1.2f, 0);
            spell.transform.position = gameObject.transform.position;
            m_spellRigid.AddForce(Vector2.left * m_bossMobData.SpellSpeed, ForceMode2D.Impulse);
            m_spellSpriteRenderer.flipX = true;
        }
    }
}
