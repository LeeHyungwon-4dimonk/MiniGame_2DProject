using UnityEngine;

/// <summary>
/// ��� ������ ������ ��� ���� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonsterData : MonsterData
{
    // ���� Ž�� �þ�
    public float MonsterSight;

    // ���� ����
    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}