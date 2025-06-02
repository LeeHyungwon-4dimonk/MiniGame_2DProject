using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    // ������ ������
    [SerializeField] private int m_durability;

    // ���ڰ� �μ��� �� ����� ������
    [SerializeField] private GameObject[] m_gameObjects;

    private SFXController m_sfxController;

    private void Awake() => Init();

    private void Init()
    {
        m_sfxController = GetComponent<SFXController>();
    }

    public void TakeDamage(int damage)
    {
        m_durability -= damage;
        m_sfxController.PlaySFX("Damage");
        if(m_durability < 0 )
        { 
            m_durability = 0;
            Break();
        }
    }

    public void Break()
    {
        for(int i = 0; i < m_gameObjects.Length; i++)
        {
            Debug.Log("����!");
            Instantiate(m_gameObjects[i], transform.position, Quaternion.identity);            
        }
        gameObject.SetActive(false);
    }
}
