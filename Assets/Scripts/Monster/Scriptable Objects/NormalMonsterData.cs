using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonsterData : MonsterData
{
    public float MonsterSight;

    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}
