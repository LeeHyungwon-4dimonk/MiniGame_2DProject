using System.Collections;
using TMPro;
using UnityEngine;

public class BreakableObject_Tutorial : MonoBehaviour, IDamageable
{
    // ������ ������
    [SerializeField] private int m_durability;

    // ���ڰ� �μ��� �� ����� ������
    [SerializeField] private GameObject[] m_gameObjects;

    [SerializeField] private TMP_Text m_text1;
    [SerializeField] private TMP_Text m_text2;

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
            m_text1.enabled = false;
            m_text2.enabled = true;
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
